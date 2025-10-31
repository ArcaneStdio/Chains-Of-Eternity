import { pool } from "./db.js";

// Fetch latest NFTs
export async function getSpellData(address) {
  try {
   
    const getQuery = `
      SELECT *
      FROM spells
      WHERE created_by != $1
      ORDER BY created_at DESC
      LIMIT 4`;
    const getResult = await pool.query(getQuery, [address]);
    console.log(getResult.rows);

    return getResult.rows;
  } catch (err) {
    console.error("getSpells error:", err.message);
    throw err;
  }
}
