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
    import ItemManager from 0x0095f13a82f1a835
    import MarketPlace2 from 0x0095f13a82f1a835
    import FungibleToken from 0x631e88ae7f1d7c20
    
    transaction( price: UFix64) {
        let vaultRef: auth(FungibleToken.Withdraw) &{FungibleToken.Vault}
        prepare(signer: auth(Storage , BorrowValue) &Account) {
            self.vaultRef = signer.storage.borrow<auth(FungibleToken.Withdraw) &{FungibleToken.Vault}>(from: /storage/ArcaneVault)
                ?? panic("Missing FlowToken vault in buyer account. Please create & link one.")
    
            // Withdraw NFT from seller's collection (this requires signer's withdraw auth)
            let payment <- self.vaultRef.withdraw(amount: price)
    
            // Pass the NFT resource and signer.address into contract
            MarketPlace2.depositFees(from: <- payment)
        }
    }


`;

export const AddTokenToMarketPlace = async (req, res) => {
  const price = req.body.price;
  //console.log(recipientAddr)
  try {
  
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [arg(price, t.UFix64)
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
