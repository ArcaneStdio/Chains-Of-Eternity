
import { pool } from "./db.js"

// Configure Flow (testnet)


const EVENT = "A.0095f13a82f1a835.MarketPLace2.Cancelled"

export async function fetchAndStoreEventsforCancelling(startHeight, endHeight) {


  const inserted = []


    try {
      await pool.query(
        `DELETE FROM listed WHERE itemID = $1`,
        [
          itemID
        ]
      )
      inserted.push(itemID)
    } catch (err) {
      console.error(`Failed to delete item ${itemID}:`, err.message)
    }
  }


