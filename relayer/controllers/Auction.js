import { pool } from "./db.js"
export async function ListedAuction(req, res) {
    const itemID = req.body.itemID
    const seller = req.body.seller
    const basePrice = req.body.basePrice
    const endTime = req.body.endTime

  const inserted = []
  
    try {
      await pool.query(
        `INSERT INTO auction_house
          (itemID, seller, basePrice, endTime)
         VALUES ($1,$2,$3,$4)
         `,
        [
          itemID,
          seller,
          basePrice,
          endTime
        ]
      )
      inserted.push(itemID)
      console.log("Added to db")
      res.send("yyay")
    } catch (err) {
      console.error(`Failed to insert NFT ${itemID}:`, err.message)
    }
  }


// Handle BidPlaced event
export async function BidPlaced(req, res) {
    const itemID = req.body.itemID
    const bidder = req.body.bidder
    const amount = req.body.amount

    try {
        await pool.query(
            `UPDATE auction_house 
             SET currentbidder = $1, currentbidding = $2, status = $3
             WHERE itemid = $4`,
            [bidder, amount, "bidding",itemID]
        )
        console.log(`Bid placed for item ${itemID}`)
        res.send("Bid placed successfully")
    } catch (err) {
        console.error(`Failed to update bid for item ${itemID}:`, err.message)
        res.status(500).send("Failed to place bid")
    }
}

// Handle BidReplaced event
export async function BidReplaced(req, res) {
    const itemID = req.body.itemID
    const oldBidder = req.body.oldBidder
    const newBidder = req.body.newBidder
    const amount = req.body.amount

    try {
        await pool.query(
            `UPDATE auction_house 
             SET currentbidder = $1, currentbidding = $2
             WHERE itemid = $3 AND currentbidder = $4`,
            [newBidder, amount, itemID, oldBidder]
        )
        console.log(`Bid replaced for item ${itemID} - Old bidder: ${oldBidder}, New bidder: ${newBidder}`)
        res.send("Bid replaced successfully")
    } catch (err) {
        console.error(`Failed to replace bid for item ${itemID}:`, err.message)
        res.status(500).send("Failed to replace bid")
    }
}

// Handle AuctionCompleted event
export async function AuctionCompleted(req, res) {
    const itemID = req.body.itemID
    const winner = req.body.winner
    const finalPrice = req.body.finalPrice

    try {
        await pool.query(
            `UPDATE auction_house 
             SET winner = $1, finalprice = $2, status = 'completed'
             WHERE itemid = $3`,
            [winner, finalPrice, itemID]
        )
        console.log(`Auction completed for item ${itemID} - Winner: ${winner}, Final price: ${finalPrice}`)
        res.send("Auction completed successfully")
    } catch (err) {
        console.error(`Failed to complete auction for item ${itemID}:`, err.message)
        res.status(500).send("Failed to complete auction")
    }
}

// Handle Cancelled event
export async function AuctionCancelled(req, res) {
    const itemID = req.body.itemID

    try {
        await pool.query(
            `DELETE FROM auction_house WHERE itemID = $1`,
            [itemID]
        )
        console.log(`Auction cancelled for item ${itemID} by seller: ${seller}`)
        res.send("Auction cancelled successfully")
    } catch (err) {
        console.error(`Failed to cancel auction for item ${itemID}:`, err.message)
        res.status(500).send("Failed to cancel auction")
    }
}

  


