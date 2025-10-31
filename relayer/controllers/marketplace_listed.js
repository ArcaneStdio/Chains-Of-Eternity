import { pool } from "./db.js"
//import {block_height} from "./MintNFT.js"
// Configure Flow (testnet)


//const EVENT = "A.0095f13a82f1a835.MarketPLace2.Listed"

export async function fetchAndStoreEventsforListing(itemID, seller, price, tokenID) {

  const inserted = []


    try {
      await pool.query(
        `INSERT INTO listed
          (itemID, seller, price, tokenID)
         VALUES ($1,$2,$3,$4)
         ON CONFLICT (itemID) DO NOTHING`,
        [
          itemID,
          seller,
          price,
          tokenID
        ]
      )
      inserted.push(itemID)
    } catch (err) {
      console.error(`Failed to insert listed item ${itemID}:`, err.message)
    }
  }


//export async function getLatestBlockHeight() {
//  const latestBlock = await fcl.send([fcl.getBlock(true)]).then(fcl.decode)
//  return latestBlock.height
//}
