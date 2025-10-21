#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileColorDatabaseGenerator
{
    private const string defaultAssetPath = "Assets/TileColorDatabase.asset";

    [MenuItem("Tools/Tile Colors/Generate Tile Color Database")]
    public static void GenerateDatabaseMenu()
    {
        Generate(defaultAssetPath, sampleStep: 4);
    }

    // sampleStep: 1 = every pixel (slow), 2 = every 2nd pixel, 4 = much faster (good quality)
    public static void Generate(string assetPath = defaultAssetPath, int sampleStep = 4)
    {
        try
        {
            var guids = new HashSet<string>();
            AddFindResults("t:Tile", guids);
            AddFindResults("t:RuleTile", guids);       // 2D extras
            AddFindResults("t:AnimatedTile", guids);   // 2D extras
            // You can add other custom tile types by name if you use them.

            var tiles = new List<TileBase>();
            foreach (var g in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(g);
                var tile = AssetDatabase.LoadAssetAtPath<TileBase>(p);
                if (tile != null) tiles.Add(tile);
            }

            if (tiles.Count == 0)
            {
                Debug.LogWarning("No Tile assets found in project.");
                return;
            }

            var textureOriginalReadable = new Dictionary<string, bool>();
            var results = new Dictionary<TileBase, Color>();

            int tIndex = 0;
            foreach (var tile in tiles)
            {
                EditorUtility.DisplayProgressBar("Generating Tile Colors", $"Processing {tile.name}", (float)tIndex / tiles.Count);
                tIndex++;

                List<Sprite> sprites = GetSpritesFromTileAsset(tile);
                if (sprites.Count == 0)
                {
                    // fallback: skip tile
                    continue;
                }

                Color sum = Color.black;
                int count = 0;

                foreach (var sprite in sprites)
                {
                    if (sprite == null) continue;

                    // Use AssetDatabase.GetAssetPath(sprite) to get the source texture asset
                    string spriteAssetPath = AssetDatabase.GetAssetPath(sprite);
                    if (string.IsNullOrEmpty(spriteAssetPath))
                        continue;

                    // Load texture at that path (source texture, not runtime atlas)
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(spriteAssetPath);
                    if (tex == null)
                        continue;

                    // Ensure readability temporarily
                    string texPath = spriteAssetPath;
                    if (!textureOriginalReadable.ContainsKey(texPath))
                    {
                        var importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
                        if (importer != null)
                            textureOriginalReadable[texPath] = importer.isReadable;
                        else
                            textureOriginalReadable[texPath] = true; // assume readable if no importer
                    }

                    var importer2 = AssetImporter.GetAtPath(texPath) as TextureImporter;
                    if (importer2 != null && !importer2.isReadable)
                    {
                        importer2.isReadable = true;
                        importer2.SaveAndReimport(); // reimport so we can read pixels
                    }

                    // Compute average with sampling (fast)
                    Rect r = sprite.textureRect;
                    Color avg = ComputeAverageOfRect(tex, r, sampleStep);
                    sum += avg;
                    count++;
                }

                if (count > 0)
                    results[tile] = sum / count;
            }

            // restore texture import settings
            foreach (var kv in textureOriginalReadable)
            {
                var path = kv.Key;
                var original = kv.Value;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null && importer.isReadable != original)
                {
                    importer.isReadable = original;
                    importer.SaveAndReimport();
                }
            }

            // create or update the ScriptableObject
            TileColorDatabase db = AssetDatabase.LoadAssetAtPath<TileColorDatabase>(assetPath);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<TileColorDatabase>();
                AssetDatabase.CreateAsset(db, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            db.SetAll(results);
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"TileColorDatabase generated with {results.Count} entries -> {assetPath}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error generating TileColorDatabase: " + e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void AddFindResults(string query, HashSet<string> guids)
    {
        try
        {
            string[] found = AssetDatabase.FindAssets(query);
            if (found != null)
                foreach (var g in found) guids.Add(g);
        }
        catch { }
    }

    // Try to extract sprite(s) used by this tile asset (handles Tile, AnimatedTile, RuleTile-ish)
    private static List<Sprite> GetSpritesFromTileAsset(TileBase tileAsset)
    {
        var list = new List<Sprite>();

        if (tileAsset is Tile simpleTile)
        {
            if (simpleTile.sprite != null) list.Add(simpleTile.sprite);
            return list;
        }

        // Use SerializedObject to attempt extracting common sprite arrays:
        var so = new SerializedObject(tileAsset);
        // common properties:
        TryAddSpriteArray(so, "m_AnimatedSprites", list); // AnimatedTile
        TryAddSpriteArray(so, "m_Sprites", list);
        TryAddSpriteArray(so, "m_TilingRules", list, nestedSpriteArrayName: "m_Sprites"); // RuleTile -> each rule has m_Sprites
        // Also try to find any object reference property that points to a Sprite
        var iter = so.GetIterator();
        while (iter.NextVisible(true))
        {
            if (iter.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (iter.objectReferenceValue is Sprite s)
                {
                    if (!list.Contains(s)) list.Add(s);
                }
            }
        }

        return list;
    }

    // helper: look for a property that is an array of sprites OR a nested rule array with nestedSpriteArrayName
    private static void TryAddSpriteArray(SerializedObject so, string propName, List<Sprite> collector, string nestedSpriteArrayName = null)
    {
        var prop = so.FindProperty(propName);
        if (prop == null) return;

        if (prop.isArray)
        {
            if (string.IsNullOrEmpty(nestedSpriteArrayName))
            {
                for (int i = 0; i < prop.arraySize; i++)
                {
                    var el = prop.GetArrayElementAtIndex(i);
                    if (el.propertyType == SerializedPropertyType.ObjectReference && el.objectReferenceValue is Sprite s)
                        if (!collector.Contains(s) && s != null) collector.Add(s);
                }
            }
            else
            {
                // nested array (e.g., m_TilingRules -> each element has m_Sprites array)
                for (int i = 0; i < prop.arraySize; i++)
                {
                    var el = prop.GetArrayElementAtIndex(i);
                    var rule = el;
                    var child = rule.FindPropertyRelative(nestedSpriteArrayName);
                    if (child != null && child.isArray)
                    {
                        for (int j = 0; j < child.arraySize; j++)
                        {
                            var cel = child.GetArrayElementAtIndex(j);
                            if (cel.propertyType == SerializedPropertyType.ObjectReference && cel.objectReferenceValue is Sprite s)
                                if (!collector.Contains(s) && s != null) collector.Add(s);
                        }
                    }
                }
            }
        }
    }

    // Compute average color of a sprite rect using sampling
    private static Color ComputeAverageOfRect(Texture2D tex, Rect rect, int sampleStep)
    {
        if (tex == null) return Color.gray;

        int x0 = Mathf.FloorToInt(rect.x);
        int y0 = Mathf.FloorToInt(rect.y);
        int w = Mathf.RoundToInt(rect.width);
        int h = Mathf.RoundToInt(rect.height);

        // clamp
        if (w <= 0 || h <= 0) return Color.gray;
        sampleStep = Mathf.Max(1, sampleStep);

        double r = 0, g = 0, b = 0, a = 0;
        int count = 0;

        for (int y = y0; y < y0 + h; y += sampleStep)
        {
            for (int x = x0; x < x0 + w; x += sampleStep)
            {
                try
                {
                    Color c = tex.GetPixel(x, y);
                    r += c.r; g += c.g; b += c.b; a += c.a;
                    count++;
                }
                catch
                {
                    // if something goes wrong, skip
                }
            }
        }

        if (count == 0) return Color.gray;
        return new Color((float)(r / count), (float)(g / count), (float)(b / count), (float)(a / count));
    }
}
#endif
