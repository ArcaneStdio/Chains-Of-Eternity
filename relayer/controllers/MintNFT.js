import * as fcl from "@onflow/fcl"
import pkg from 'elliptic';
import { SHA3 } from "sha3";   // instead of crypto.sha256
import 'dotenv/config'; 
import { type } from "os";
import { fetchAndStoreEventsforMinting } from "./nftminted_store.js";
// Load environment variables
const ACCOUNT_ADDRESS = process.env.ACCOUNT_ADDRESS;
const PRIVATE_KEY = process.env.PRIVATE_KEY; // Hex string
const KEY_ID = Number(process.env.KEY_ID || 0); // Key index in Flow account

var block_height;

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



const cadence = `
  import NonFungibleToken from 0x631e88ae7f1d7c20
  import ItemManager from 0x0095f13a82f1a835
  // import RandomConsumer from 0x0095f13a82f1a835

  transaction(
      recipient: Address
  ) {

      /// local variable for storing the minter reference
      let minter: &ItemManager.NFTMinter

      /// Reference to the receiver's collection
      let recipientCollectionRef: &{NonFungibleToken.Receiver}

      prepare(signer: auth(BorrowValue) &Account) {

          // borrow a reference to the NFTMinter resource in storage
          self.minter = signer.storage.borrow<&ItemManager.NFTMinter>(from: ItemManager.MinterStoragePath)
              ?? panic("The signer does not store a ItemManager Collection object at the path "
                          .concat(ItemManager.CollectionStoragePath.toString())
                          .concat("The signer must initialize their account with this collection first!"))

          // Borrow the recipient's public NFT collection reference
          self.recipientCollectionRef = getAccount(recipient).capabilities.borrow<&{NonFungibleToken.Receiver}>(
                  ItemManager.CollectionPublicPath
          ) ?? panic("The account ".concat(recipient.toString()).concat(" does not have a NonFungibleToken Receiver at ")
                  .concat(ItemManager.CollectionPublicPath.toString())
                  .concat(". The account must initialize their account with this collection first!"))
      }

      execute {

          let id: UInt64 = 1
          // Mint the NFT and deposit it to the recipient's collection
          let mintedNFT <- self.minter.createNFT(
              name: "Sword of Testing",
              description: "A test sword",
              itemType: ItemManager.ItemType.Weapon,
              rarity: ItemManager.Rarity.Common,
              stackable: false,
              weapon: ItemManager.WeaponData(
                  damage: 10,
                  attackSpeed: 1,
                  criticalRate: 5,
                  criticalDamage: 50
              ),
              armour: nil,
              consumable: nil,
              accessory: nil
          )
          self.recipientCollectionRef.deposit(token: <-mintedNFT)
      }
  }


`;
//export var block_height;
export const mintNFT = async (req, res) => {
  const recipientAddr = req.body.recipientAddr;
  try {

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

    const response = status

    const form = status.events[0].data
    console.log("Sending response")
    res.send(response)
    console.log("response sent")
    try {
        console.log("Sending to db")
        const result = await fetchAndStoreEventsforMinting(form.id, form.name, form.description, form.itemType, form.rarity, form.armour, form.weapon, form.minter)
      
      } catch (err) {
        console.error("Error in /listen-events-nft:", err)
      }
  } catch (err) {
    console.error('Minting failed:', err);

  }
};
