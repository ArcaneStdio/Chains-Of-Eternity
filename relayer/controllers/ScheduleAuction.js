import * as fcl from "@onflow/fcl"
import pkg from 'elliptic';
import { SHA3 } from "sha3";   // instead of crypto.sha256
import 'dotenv/config'; 
import { receiveMessageOnPort } from "worker_threads";
// Load environment variables
const ACCOUNT_ADDRESS = process.env.USER_ACCOUNT_ADDRESS;
const PRIVATE_KEY = process.env.USER_PRIVATE_KEY; // Hex string
const KEY_ID = Number(process.env.KEY_ID || 0); // Key index in Flow account

fcl.config()
  .put("accessNode.api", "http://127.0.0.1:3569")
  .put("app.detail.title", "Hero Game")
  .put("flow.network", "emulator")
  .put("discovery.wallet", "http://localhost:8701/fcl/authn")

// Helper: Create signature
const { ec: EC } = pkg;
const ec = new EC("p256");



export const signWithKey = async (privateKeyHex, msgHex) => {
  // Hash with SHA3-256
  const sha = new SHA3(256);
  sha.update(Buffer.from(msgHex, "hex"));
  const msgHash = sha.digest();

  // Load private key
  if (privateKeyHex.startsWith("0x")) {
    privateKeyHex = privateKeyHex.slice(2);
  }
  if (privateKeyHex.length !== 64) {
    throw new Error(`Invalid private key length: ${privateKeyHex.length}. Expected 64 hex chars (32 bytes).`);
  }

  const key = ec.keyFromPrivate(Buffer.from(privateKeyHex, "hex"));

  // Derive Flow-compatible public key (X+Y, no 04 prefix)
  const pubPoint = key.getPublic();
  const x = pubPoint.getX().toArrayLike(Buffer, "be", 32);
  const y = pubPoint.getY().toArrayLike(Buffer, "be", 32);
  const derivedPubKey = Buffer.concat([x, y]).toString("hex");

  console.log("----------------------");
 // console.log("Derived Flow Public Key:", derivedPubKey);

  // Sign hash
  const sig = key.sign(msgHash);

  const r = sig.r.toArrayLike(Buffer, "be", 32);
  const s = sig.s.toArrayLike(Buffer, "be", 32);

  return Buffer.concat([r, s]).toString("hex"); // 64-byte hex signature
};


// --- Authorization function ---
const signingFunction = async ({ message }) => {
  return {
    addr: fcl.withPrefix(ACCOUNT_ADDRESS),
    keyId: KEY_ID,
    signature: await signWithKey(PRIVATE_KEY, message),
  };
};

const authz = async (account) => {
  return {
    ...account,
    tempId: `${ACCOUNT_ADDRESS}-${KEY_ID}`,
    addr: fcl.withPrefix(ACCOUNT_ADDRESS),
    keyId: KEY_ID,
    signingFunction,

  };
};


// ---- Configure FCL for Testnet (Cadence 1.0 ready) ----



const cadence = `
    import FlowTransactionScheduler from 0x8c5303eaa26202d6
  import FlowToken from 0x7e60df042a9c0868
  import FungibleToken from 0x9a0766d93b6608b7
  import AuctionHouse from 0x0095f13a82f1a835
  import AuctionCallbackHandler from 0x0095f13a82f1a835
  import NonFungibleToken from 0x631e88ae7f1d7c20
  import ItemManager from 0x0095f13a82f1a835
  /// Schedule an increment of the Counter with a relative delay in seconds
  transaction(
      delaySeconds: UFix64,
      priority: UInt8,
      executionEffort: UInt64,
      tokenID: UInt64,
      price: UFix64
  ) {
      let withdrawRef: auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}
      prepare(signer: auth(Storage, Capabilities) &Account) {
          self.withdrawRef = signer.storage.borrow<auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}>(from: ItemManager.CollectionStoragePath)
              ?? panic("Missing ItemManager collection")

          // Withdraw NFT from seller's collection (this requires signer's withdraw auth)
          let nft <- self.withdrawRef.withdraw(withdrawID: tokenID)

          // Pass the NFT resource and signer.address into contract
          let future = getCurrentBlock().timestamp + delaySeconds
          let listId: UInt64 = AuctionHouse.listItem(nft: <- nft, basePrice: price, seller: signer.address, endTime: future)
          let transactionData = AuctionCallbackHandler.loradata(listingId: listId)

          let pr = priority == 0
              ? FlowTransactionScheduler.Priority.High
              : priority == 1
                  ? FlowTransactionScheduler.Priority.Medium
                  : FlowTransactionScheduler.Priority.Low

          let est = FlowTransactionScheduler.estimate(
              data: transactionData,
              timestamp: future,
              priority: pr,
              executionEffort: executionEffort
          )

          assert(
              est.timestamp != nil || pr == FlowTransactionScheduler.Priority.Low,
              message: est.error ?? "estimation failed"
          )

          let vaultRef = signer.storage
              .borrow<auth(FungibleToken.Withdraw) &FlowToken.Vault>(from: /storage/flowTokenVault)
              ?? panic("missing FlowToken vault")
          let fees <- vaultRef.withdraw(amount: est.flowFee ?? 0.0) as! @FlowToken.Vault

          let handlerCap = signer.capabilities.storage
              .issue<auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}>(/storage/AuctionCallbackHandler)

          let receipt <- FlowTransactionScheduler.schedule(
              handlerCap: handlerCap,
              data: transactionData,
              timestamp: future,
              priority: pr,
              executionEffort: executionEffort,
              fees: <-fees
          )

          log("Scheduled transaction id: ".concat(receipt.id.toString()).concat(" at ").concat(receipt.timestamp.toString()))

          destroy receipt
      }
  }

`;

export const ScheduleAuction = async (req, res) => {
  const delaySeconds = req.body.delaySeconds;
  const priority = req.body.priority;
  const executionEffort = req.body.executionEffort;
  const tokenID = req.body.tokenID;
  const price = req.body.price;
  //console.log(recipientAddr)
  try {
  
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [
        arg(delaySeconds, t.UFix64),
        arg(priority, t.UInt8),
        arg(executionEffort, t.UInt64),
        arg(tokenID, t.UInt64),
        arg(price, t.UFix64)
      ],
      proposer: authz,       // Must be the account that has NFTMinter
      payer: authz,          // Pays gas
      authorizations: [authz], // Signer must authorize (has NFTMinter)
      limit: 9999,
    });

    console.log('Transaction submitted with ID:', txId);

    const status = await fcl.tx(txId).onceSealed();
    console.log('Transaction sealed:', status);
    console.log("-----------------------------")
    console.log("Sending response")
    res.send(status)
  } catch (err) {
    console.error('Scheduling failed:', err);
    throw err;
  }
};
