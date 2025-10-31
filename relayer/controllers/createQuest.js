import * as fcl from "@onflow/fcl"
import pkg from 'elliptic';
import { SHA3 } from "sha3";   // instead of crypto.sha256
import 'dotenv/config'; 
import { pool } from "./db.js"
// Load environment variables
const ACCOUNT_ADDRESS = process.env.ACCOUNT_ADDRESS;
const PRIVATE_KEY = process.env.PRIVATE_KEY; // Hex string
const KEY_ID = Number(process.env.KEY_ID || 0); // Key index in Flow account


fcl.config()
  .put("accessNode.api", "https://rest-testnet.onflow.org")
  .put("app.detail.title", "Hero Game")
  .put("flow.network", "testnet")
  .put("discovery.wallet", "https://fcl-discovery.onflow.org/testnet/authn")

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

 // console.log("----------------------");
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








export const createQuest = async (req, res) => {
  const level = req.body.level;
  const rarity = req.body.rarity;


  var cadence = `
  import FungibleToken from 0x9a0766d93b6608b7
  import FlowToken from 0x7e60df042a9c0868
  import RandomPicker from 0x2f52190177ec174b
  import QuestManager from 0x0095f13a82f1a835
  /// Commits the defined amount of Flow as a bet to the RandomPicker contract, saving the returned Receipt to storage
  ///
  transaction(level: UInt8, rarity: String) {

      prepare(signer: auth(BorrowValue, SaveValue) &Account) {
          // Withdraw my bet amount from my FlowToken vault
          //let flowVault = signer.storage.borrow<auth(FungibleToken.Withdraw) &FlowToken.Vault>(from: /storage/flowTokenVault)!
          //let bet <- flowVault.withdraw(amount: betAmount)
          let numEnemyTypes = QuestManager.RARITY_ENEMY_COUNT[rarity] ?? panic("Unknown rarity")
          let rarityFactor = QuestManager.RARITY_MULTIPLIER[rarity] ?? panic("Unknown rarity multiplier")
          let totalWeight: UFix64 = UFix64(level) * UFix64(rarityFactor) * 20.0
          let enemy_1 = QuestManager.ENEMIES[numEnemyTypes[0]]
          let weight_enemy1 = QuestManager.ENEMY_WEIGHTS[enemy_1]!
          var maxCount: UFix64 = 1.0

          maxCount = totalWeight / UFix64(weight_enemy1)

          let range1: [UInt64] = []
          var i: UFix64 = 0.0
          while i <= UFix64(maxCount) {
              range1.append(UInt64(i))
              i = i + 1.0
          }
          //let count1 = UFix64(QuestManager.pickRandomValue(values: range1))

          let receipt <- RandomPicker.commit(values: range1)

          // Check that I don't already have a receipt stored
          if signer.storage.type(at: RandomPicker.ReceiptStoragePath) != nil {
              panic("Storage collision at path=".concat(RandomPicker.ReceiptStoragePath.toString()).concat(" a Receipt is already stored!"))
          }

          // Save that receipt to my storage
          // Note: production systems would consider handling path collisions
          signer.storage.save(<-receipt, to: RandomPicker.ReceiptStoragePath)
      }
  }

`;
  try {
    const txId_rand = await fcl.mutate({
      cadence,
      args: (arg, t) => [arg(level, t.UInt8), arg(rarity, t.String)],
      proposer: authz,       // Must be the account that has NFTMinter
      payer: authz,          // Pays gas
      authorizations: [authz], // Signer must authorize (has NFTMinter)
      limit: 9999,
    });



    
    console.log('Transaction for generating random enemies submitted with ID:', txId_rand);

    const status_random = await fcl.tx(txId_rand).onceSealed();

    //const response_random = status_random
//
    //const form_random = status_random.events[0].data

    console.log("Randomness commited")


    var cadence = `
  import FlowTransactionScheduler from 0x11a3131ddbaa0917
  import FlowTransactionSchedulerUtils from 0x11a3131ddbaa0917
  import FlowToken from 0x7e60df042a9c0868
  import FungibleToken from 0x9a0766d93b6608b7
  import QuestManager from 0x0095f13a82f1a835
  import QuestTransactionQuestHandler from 0x0095f13a82f1a835
  import RandomPicker from 0x2f52190177ec174b
  /// Create a quest and schedule its cleanup after 2 days
  transaction(
      level: UInt8,
      rarity: String,
      priority: UInt8,
      executionEffort: UInt64
  ) {

      prepare(signer: auth(Storage, Capabilities) &Account) {


          //VRF
          let receipt <- signer.storage.load<@RandomPicker.Receipt>(from: RandomPicker.ReceiptStoragePath)
              ?? panic("No Receipt found in storage at path=".concat(RandomPicker.ReceiptStoragePath.toString()))

          // Reveal by redeeming my receipt - fingers crossed!
          let winnings = RandomPicker.reveal(receipt: <-receipt)

          //Enemies Logic

          let numEnemyTypes = QuestManager.RARITY_ENEMY_COUNT[rarity] ?? panic("Unknown rarity")
          let rarityFactor = QuestManager.RARITY_MULTIPLIER[rarity] ?? panic("Unknown rarity multiplier")
          let totalWeight: UFix64 = UFix64(level) * UFix64(rarityFactor) * 100.0

          let enemy1 = winnings
          let enemy_1 = QuestManager.ENEMIES[numEnemyTypes[0]]
          let enemy_2 = QuestManager.ENEMIES[numEnemyTypes[1]]

          let weight_enemy1 = QuestManager.ENEMY_WEIGHTS[enemy_1]!

          let enemiesForQuest = [
              QuestManager.ENEMIES[numEnemyTypes[0]],
              QuestManager.ENEMIES[numEnemyTypes[1]]
          ]

          let remainingWeight = totalWeight - (UFix64(weight_enemy1) * UFix64(enemy1))

          //let enemy_2 = enemiesForQuest[1]
          let weight_enemy2 = QuestManager.ENEMY_WEIGHTS[enemy_2]!
          let enemy2: UFix64 = remainingWeight / UFix64(weight_enemy2)

          var finalEnemies: {String: UInt64} = {}
          finalEnemies[enemiesForQuest[0]] = UInt64(enemy1)
          finalEnemies[enemiesForQuest[1]] = UInt64(enemy2)

          //QuestManager

          // Borrow the Manager from QuestManager contract account
          let managerRef = signer.storage.borrow<&QuestManager.Manager>(from: /storage/QuestManager)
              ?? panic("Could not borrow Manager reference from QuestManager contract")

          let questID: UInt64 = managerRef.createQuest(level: level, rarity: rarity, enemies: finalEnemies)
          let transactionData = QuestTransactionQuestHandler.questinput(questID: questID)

          // Schedule cleanup for 2 days from now (172800 seconds)
          let cleanupTimestamp: UFix64 = getCurrentBlock().timestamp + 172800.0

          // Convert priority
          let pr = priority == 0
              ? FlowTransactionScheduler.Priority.High
              : priority == 1
                  ? FlowTransactionScheduler.Priority.Medium
                  : FlowTransactionScheduler.Priority.Low

          // Estimate fees
          let est = FlowTransactionScheduler.estimate(
              data: transactionData,
              timestamp: cleanupTimestamp,
              priority: pr,
              executionEffort: executionEffort
          )

          assert(
              est.timestamp != nil || pr == FlowTransactionScheduler.Priority.Low,
              message: est.error ?? "estimation failed"
          )

          // Withdraw fees from signer's Flow vault
          let vaultRef = signer.storage.borrow<auth(FungibleToken.Withdraw) &FlowToken.Vault>(from: /storage/flowTokenVault)
              ?? panic("missing FlowToken vault")
          let fees <- vaultRef.withdraw(amount: est.flowFee ?? 0.0) as! @FlowToken.Vault

          // Create FlowTransactionSchedulerUtils Manager if not exists
          if !signer.storage.check<@{FlowTransactionSchedulerUtils.Manager}>(from: FlowTransactionSchedulerUtils.managerStoragePath) {
              let manager <- FlowTransactionSchedulerUtils.createManager()
              signer.storage.save(<-manager, to: FlowTransactionSchedulerUtils.managerStoragePath)

              // Create a public capability to the scheduled transaction manager
              let managerRef = signer.capabilities.storage.issue<&{FlowTransactionSchedulerUtils.Manager}>(FlowTransactionSchedulerUtils.managerStoragePath)
              signer.capabilities.publish(managerRef, at: FlowTransactionSchedulerUtils.managerPublicPath)
          }

          // Get or create the Handler capability
          var handlerCap: Capability<auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}>? = nil

          // Check if handler already exists
          if !signer.storage.check<@QuestTransactionQuestHandler.Handler>(from: /storage/QuestTransactionQuestHandler) {
              let handler <- QuestTransactionQuestHandler.createHandler()
              signer.storage.save(<-handler, to: /storage/QuestTransactionQuestHandler)
          }

          // Get the capability (try to get existing controllers first)
          let controllers = signer.capabilities.storage.getControllers(forPath: /storage/QuestTransactionQuestHandler)

          if controllers.length > 0 {
              if let cap = controllers[0].capability as? Capability<auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}> {
                  handlerCap = cap
              } else if controllers.length > 1 {
                  handlerCap = controllers[1].capability as! Capability<auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}>
              }
          }

          // If no valid capability found, issue a new one
          if handlerCap == nil || !handlerCap!.check() {
              handlerCap = signer.capabilities.storage.issue<auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}>(/storage/QuestTransactionQuestHandler)
          }

          assert(handlerCap != nil && handlerCap!.check(), message: "Handler capability is invalid")

          // Borrow the FlowTransactionSchedulerUtils Manager to schedule
          let schedulerManager = signer.storage.borrow<auth(FlowTransactionSchedulerUtils.Owner) &{FlowTransactionSchedulerUtils.Manager}>(from: FlowTransactionSchedulerUtils.managerStoragePath)
              ?? panic("Could not borrow a Manager reference from FlowTransactionSchedulerUtils")

          // Schedule the cleanup transaction
          schedulerManager.schedule(
              handlerCap: handlerCap!,
              data: transactionData,
              timestamp: cleanupTimestamp,
              priority: pr,
              executionEffort: executionEffort,
              fees: <-fees
          )

          log("Quest created with ID: ".concat(questID.toString()))
          log("Cleanup scheduled for timestamp: ".concat(cleanupTimestamp.toString()))
      }
  }
  `;

    const txId_quest = await fcl.mutate({
      cadence,
      args: (arg, t) => [arg(level, t.UInt8), arg(rarity, t.String), arg(1, t.UInt8), arg(1000, t.UInt64)],
      proposer: authz,       
      payer: authz,          
      authorizations: [authz], 
      limit: 9999,
    });

    console.log('Transaction for creating quest submitted with ID:', txId_quest);

    const status_quest = await fcl.tx(txId_quest).onceSealed();

    const form = status_quest.events[3].data

    //const response_quest = status_quest
    console.log(form)
   // console.log(ourdata)

    
    try {
      await pool.query(
        `INSERT INTO quests
          (id, level, rarity, assigned_to, enemies, status, expires_at)
         VALUES ($1, $2, $3, $4, $5, $6, $7)
         ON CONFLICT (id) DO NOTHING`,
        [
          form.id,
          form.level,
          form.rarity,
          '[]',              // rewards (as in your original)
          form.enemies,
          "ACTIVE",
          form.expiresAt
        ]
      )
      console.log(`Inserted quest with ID: ${form.id}`)
    } catch (err) {
      console.error(`Failed to insert quest ${form.id}:`, err.message)
    }
    console.log("Sending response")
    res.send(form)
    console.log("response sent")
  } catch (err) {
    console.error('Creating Quest failed:', err);
  }
};
