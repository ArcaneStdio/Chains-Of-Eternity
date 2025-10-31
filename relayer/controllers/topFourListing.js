import { pool } from "./db.js";

// Fetch latest NFTs
export async function getLatestNFTs(limit = 4) {
  try {
    // 1. Get latest tokenids from listed table
    const listedQuery = `
      SELECT tokenID
      FROM listed 
      ORDER BY created_at DESC 
      LIMIT $1;
    `;
    const listedResult = await pool.query(listedQuery, [limit]);
    const tokenIds = listedResult.rows.map(row => row.tokenid);

    if (tokenIds.length === 0) {
      return [];
    }

    // 2. Fetch details from nft_minted
    const nftQuery = `
      SELECT * 
      FROM nft_minted 
      WHERE nft_id = ANY($1);
    `;
    const nftResult = await pool.query(nftQuery, [tokenIds]);

    return nftResult.rows;
  } catch (err) {
    console.error("getLatestNFTs error:", err.message);
    throw err;
  }
}
