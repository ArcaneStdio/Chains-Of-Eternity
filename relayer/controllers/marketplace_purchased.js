import * as fcl from "@onflow/fcl"
import { pool } from "./db.js"

// Configure Flow (testnet)
fcl.config()
  .put("accessNode.api", "https://rest-testnet.onflow.org")
  .put("flow.network", "testnet")

const EVENT = "A.0095f13a82f1a835.MarketPLace2.Purchased"

export async function fetchAndStoreEventsforPurchasing(startHeight, endHeight) {
  const events = await fcl.send([
    fcl.getEventsAtBlockHeightRange(
      EVENT,
      startHeight,
      endHeight
    ),
  ]).then(fcl.decode)

  const inserted = []
  for (const ev of events) {
    const {
      itemID,
    } = ev.data

    try {
        await pool.query(
        `DELETE FROM listed WHERE itemID = $1`,
        [
          itemID
        ]
      )
      inserted.push(itemID)
    } catch (err) {
      console.error(`Failed to insert purchased item ${itemID}:`, err.message)
    }
  }

  return { found: events.length, inserted }
}


