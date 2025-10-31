
import { pool } from "./db.js"



export async function fetchAndStoreEventsforMinting(id, name, description, itemType, rarity, armour, weapon, minter,) {

  const inserted = []
  
    try {
      await pool.query(
        `INSERT INTO nft_minted
          (nft_id, name, description, kind, rarity, armour, armour_resistances, weapon, general, minted_by)
         VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,$10)
         ON CONFLICT (nft_id) DO NOTHING`,
        [
          id,
          name,
          description,
          itemType,
          rarity,
          armour ? JSON.stringify(armour) : "{}",
          "{}",
          weapon ? JSON.stringify(weapon) : "{}",
          "{}",
          minter,
        ]
      )
      inserted.push(id)
      console.log("Added to db")
    } catch (err) {
      console.error(`Failed to insert NFT ${id}:`, err.message)
    }
  }

  


