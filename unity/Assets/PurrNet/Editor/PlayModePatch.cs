using System.Collections.Generic;
using PurrNet.Modules;
using UnityEditor;
using UnityEngine;

namespace PurrNet.Editor
{
    [UsedByIL]
    public static class PlayModePatch
    {
        static readonly HashSet<EditorWindow> _windows = new ();

        public static void Repaint()
        {
            foreach (var window in _windows)
            {
                if (window)
                    window.Repaint();
            }
        }

        [UsedByIL]
        public static void OnGUI(EditorWindow window)
        {
            _windows.Add(window);

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            PurrNetToolBarStatus.OnToolbarGUI();
            GUILayout.Space(100f);
            GUILayout.EndHorizontal();
        }
    }
}
