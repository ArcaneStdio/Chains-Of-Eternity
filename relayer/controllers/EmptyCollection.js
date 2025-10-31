import * as fcl from "@onflow/fcl"
import pkg from 'elliptic';
import { SHA3 } from "sha3";   // instead of crypto.sha256
import 'dotenv/config'; 
// Load environment variables
const ACCOUNT_ADDRESS = process.env.USER_ACCOUNT_ADDRESS;
const PRIVATE_KEY = process.env.USER_PRIVATE_KEY; // Hex string
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
  //  Hash with SHA3-256
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


const cadence = `
    import ItemManager from 0x0095f13a82f1a835
    import NonFungibleToken from 0x631e88ae7f1d7c20

    transaction {

        prepare(signer: auth(BorrowValue, IssueStorageCapabilityController, PublishCapability, SaveValue, UnpublishCapability) &Account) {

            // Return early if the account already has a collection
            if signer.storage.borrow<&ItemManager.Collection>(from: ItemManager.CollectionStoragePath) != nil {
                return
            }

            // Create a new empty collection
            let collection <- ItemManager.createEmptyCollection(nftType: Type<@ItemManager.NFT>())

            // save it to the account
            signer.storage.save(<-collection, to: ItemManager.CollectionStoragePath)

            let collectionCap = signer.capabilities.storage.issue<&ItemManager.Collection>(ItemManager.CollectionStoragePath)
            signer.capabilities.publish(collectionCap, at: ItemManager.CollectionPublicPath)
        }
    }

`;

export const emptyCollection = async (req, res) => {
  //const recipientAddr = req.body.recipientAddr;
  try {
    const txId = await fcl.mutate({
      cadence,
      proposer: authz,       // Must be the account that has NFTMinter
      payer: authz,          // Pays gas
      authorizations: [authz], // Signer must authorize (has NFTMinter)
      limit: 9999,
    });

    console.log('Transaction submitted with ID:', txId);

    const status = await fcl.tx(txId).onceSealed();
    console.log('Transaction sealed:', status);
    console.log("Sending response")
    res.send(status)
  } catch (err) {
    console.error('Minting failed:', err);
    throw err;
  }
};
