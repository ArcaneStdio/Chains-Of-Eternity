import * as fcl from "@onflow/fcl"
import pkg from 'elliptic';
import { SHA3 } from "sha3";   // instead of crypto.sha256
import 'dotenv/config'; 
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

    transaction(
        nftID: UInt64,

        // Offensive
        damage: UInt32,
        attackSpeed: UInt32,
        criticalRate: UInt32,
        criticalDamage: UInt32,

        // Defensive
        maxHealth: UInt32,
        defense: UInt32,
        healthRegeneration: UInt32,
        resistances: [UInt32],

        // Special
        maxEnergy: UInt32,
        energyRegeneration: UInt32,
        maxMana: UInt32,
        manaRegeneration: UInt32,

        // Stat Points
        constitution: UInt32,
        strength: UInt32,
        dexterity: UInt32,
        intelligence: UInt32,
        stamina: UInt32,
        agility: UInt32,
        remainingPoints: UInt32
    ) {
        let nftRef: &HeroNFT.NFT

        prepare(signer: auth(BorrowValue) &Account) {
            let collectionRef = signer.storage.borrow<&HeroNFT.Collection>(
                from: HeroNFT.CollectionStoragePath
            ) ?? panic("No HeroNFT.Collection found in signer’s account storage")

            self.nftRef = collectionRef.ownedNFTs[nftID] as! &HeroNFT.NFT
        }

        execute {
            let newOffensive = HeroNFT.OffensiveStats(
                damage: damage,
                attackSpeed: attackSpeed,
                criticalRate: criticalRate,
                criticalDamage: criticalDamage
            )
            let newDefensive = HeroNFT.DefensiveStats(
                maxHealth: maxHealth,
                defense: defense,
                healthRegeneration: healthRegeneration,
                resistances: resistances
            )
            let newSpecial = HeroNFT.SpecialStats(
                maxEnergy: maxEnergy,
                energyRegeneration: energyRegeneration,
                maxMana: maxMana,
                manaRegeneration: manaRegeneration
            )
            let newStatPoints = HeroNFT.StatPointsAssigned(
                constitution: constitution,
                strength: strength,
                dexterity: dexterity,
                intelligence: intelligence,
                stamina: stamina,
                agility: agility,
                remainingPoints: remainingPoints
            )
            let newStats = HeroNFT.Stats(
                offensiveStats: newOffensive,
                defensiveStats: newDefensive,
                specialStats: newSpecial,
                statPointsAssigned: newStatPoints
            )

            self.nftRef.updateHeroStats(newStats: newStats)

            log("✅ Hero stats updated successfully for NFT ".concat(nftID.toString()))
        }
    }

`;

export const updateHero = async (req, res) => {
  const recipientAddr = req.body.recipientAddr;
  try {
    //let resulteAcc = await serverAuth();
    //console.log(resulteAcc)
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [
        arg(req.body.nftID, t.UInt64),
        arg(req.body.damage, t.UInt32),
        arg(req.body.attackSpeed, t.UInt32),
        arg(req.body.criticalRate, t.UInt32),
        arg(req.body.criticalDamage, t.UInt32),
        
        arg(req.body.maxHealth, t.UInt32),
        arg(req.body.defense, t.UInt32),
        arg(req.body.healthRegeneration, t.UInt32),
        arg(req.body.resistances, t.Array(t.UInt32)),
        
        arg(req.body.maxEnergy, t.UInt32),
        arg(req.body.energyRegeneration, t.UInt32),
        arg(req.body.maxMana, t.UInt32),
        arg(req.body.manaRegeneration, t.UInt32),
        
        arg(req.body.constitution, t.UInt32),
        arg(req.body.strength, t.UInt32),
        arg(req.body.dexterity, t.UInt32),
        arg(req.body.intelligence, t.UInt32),
        arg(req.body.stamina, t.UInt32),
        arg(req.body.agility, t.UInt32),
        arg(req.body.remainingPoints, t.UInt32)],
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
    console.error('Updating Hero Data Failed:', err);
    throw err;
  }
};
