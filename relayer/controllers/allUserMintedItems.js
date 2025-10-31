import { pool } from "./db.js";

// Fetch latest NFTs
export async function getUserNFTs(address) {
  try {
    // 1. Get latest tokenids from listed table
    const mintedQuery = `
      SELECT *
      FROM nft_minted
      WHERE minted_by = $1;
    `;
    const mintedResult = await pool.query(mintedQuery, [address]);

    return mintedResult.rows;
  } catch (err) {
    console.error("getNFTs error:", err.message);
    throw err;
  }
}
