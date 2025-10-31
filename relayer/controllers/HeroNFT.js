import * as fcl from "@onflow/fcl"
import pkg from 'elliptic';
import { SHA3 } from "sha3";   // instead of crypto.sha256
import 'dotenv/config'; 
import { receiveMessageOnPort } from "worker_threads";
// Load environment variables
const ACCOUNT_ADDRESS = process.env.ACCOUNT_ADDRESS;
const PRIVATE_KEY = process.env.PRIVATE_KEY; // Hex string
const KEY_ID = Number(process.env.KEY_ID || 0); // Key index in Flow account

fcl.config()
  .put("accessNode.api", "https://rest-testnet.onflow.org")
  .put("app.detail.title", "Hero Game")
  .put("flow.network", "testnet")
  .put("accessNode.api", "https://rest-testnet.onflow.org")
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
  import HeroNFT from 0x0095f13a82f1a835
  import NonFungibleToken from 0x631e88ae7f1d7c20

  transaction(recipient: Address) {
      let minter: &HeroNFT.NFTMinter
      let recipientCollectionRef: &{NonFungibleToken.Receiver}

      prepare(signer: auth(BorrowValue) &Account) {
          self.minter = signer.storage.borrow<&HeroNFT.NFTMinter>(
              from: HeroNFT.MinterStoragePath
          ) ?? panic("Signer does not have the minter resource!")

          self.recipientCollectionRef = getAccount(recipient)
              .capabilities
              .borrow<&{NonFungibleToken.Receiver}>(HeroNFT.CollectionPublicPath)
              ?? panic("Recipient does not have a collection!")
      }

      execute {
          let id = HeroNFT.totalSupply
          let offensiveStats = HeroNFT.OffensiveStats(
              damage: 10,
              attackSpeed: 1,
              criticalRate: 5,
              criticalDamage: 50
          )
          let defensiveStats = HeroNFT.DefensiveStats(
              maxHealth: 100,
              defense: 10,
              healthRegeneration: 5,
              resistances: [1, 2, 3]
          )
          let specialStats = HeroNFT.SpecialStats(
              maxEnergy: 50,
              energyRegeneration: 5,
              maxMana: 50,
              manaRegeneration: 5
          )
          let statPointsAssigned = HeroNFT.StatPointsAssigned(
              constitution: 10,
              strength: 10,
              dexterity: 10,
              intelligence: 10,
              stamina: 10,
              agility: 10,
              remainingPoints: 0
          )
          let stats = HeroNFT.Stats(
              offensiveStats: offensiveStats,
              defensiveStats: defensiveStats,
              specialStats: specialStats,
              statPointsAssigned: statPointsAssigned
          )

          let mintedNFT <- self.minter.createNFT(
              id: id,
              playerName: "Test Hero",
              playerID: "test123",
              level: 1,
              isBanned: false,
              raceName: "Human",
              equippedItems: [],
              stats: stats,
              imgURL: nil
          )
          self.recipientCollectionRef.deposit(token: <-mintedNFT)
      }
  }
`;

export const mintHero = async (req, res) => {
  const recipientAddr = req.body.recipientAddr;
  console.log(recipientAddr)
  try {
    //let resulteAcc = await serverAuth();
    //console.log(resulteAcc)
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [arg(recipientAddr, t.Address)],
      proposer: authz,       // Must be the account that has NFTMinter
      payer: authz,          // Pays gas
      authorizations: [authz], // Signer must authorize (has NFTMinter)
      limit: 9999,
    });

    console.log('Transaction submitted with ID:', txId);

    const status = await fcl.tx(txId).onceSealed();
    //console.log('Transaction sealed:', status);

    const response = {
      "tokenID" : (status.events)[0].data.id
    }

    console.log("Sending response")
    res.send(response)
    console.log("response sent")
    //console.log("-----------------------------")
    //
    //res.send(status)
  } catch (err) {
    console.error('Minting failed:', err);
    throw err;
  }
};
