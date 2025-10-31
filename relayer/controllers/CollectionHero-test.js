import * as fcl from "@onflow/fcl"

// ---- Configure FCL for Testnet (Cadence 1.0 ready) ----
fcl.config()
  .put("app.detail.title", "Hero Game")
  .put("flow.network", "testnet")
  .put("accessNode.api", "https://rest-testnet.onflow.org")
  .put("discovery.wallet", "https://fcl-discovery.onflow.org/testnet/authn")

//let unityInstanceRef = null;
//
//function setUnityInstance(instance) {
//  unityInstanceRef = instance;
//}
//
//function sendToUnity(method, payload = "") {
//  try {
//    if (unityInstanceRef) {
//      unityInstanceRef.SendMessage("AuthCanvas", method, String(payload ?? ""));
//    } else {
//      console.log("[FlowBridge->Unity]", method, payload);
//    }
//  } catch (e) {
//    console.warn("SendMessage failed:", e);
//  }
//}
//
//async function authn() {
//  // Opens wallet; resolves when user has an authenticated session
//  await fcl.authenticate()
//  const user = await fcl.currentUser().snapshot()
//  return user
//}
//
//// ---- Public API (exposed on window.FlowBridge) ----
//async function connectFlow() {
//  try {
//    const user = await authn()
//    sendToUnity("OnFlowWalletConnected", user?.addr ?? "")
//  } catch (e) {
//    sendToUnity("OnFlowError", e?.message ?? String(e))
//  }
//}
//
//async function disconnectFlow() {
//  try {
//    await fcl.unauthenticate()
//    sendToUnity("OnWalletDisconnected", "")
//  } catch (e) {
//    sendToUnity("OnFlowError", e?.message ?? String(e))
//  }
//}

// Example transaction: create empty Hero collection (no args)
export const heroCollection = async (req, res) => {
  const cadence = `
    import HeroNFT from 0x0095f13a82f1a835

    transaction {
      prepare(signer: auth(BorrowValue, IssueStorageCapabilityController, PublishCapability, SaveValue, UnpublishCapability) &Account) {
        if signer.storage.borrow<&HeroNFT.Collection>(from: HeroNFT.CollectionStoragePath) != nil {
          return
        }
        let collection <- HeroNFT.createEmptyCollection(nftType: Type<@HeroNFT.NFT>())
        signer.storage.save(<-collection, to: HeroNFT.CollectionStoragePath)
        let collectionCap = signer.capabilities.storage.issue<&HeroNFT.Collection>(HeroNFT.CollectionStoragePath)
        signer.capabilities.publish(collectionCap, at: HeroNFT.CollectionPublicPath)
      }
    }
  `
  try {
    // Use the current wallet user as proposer/payer/authorizations
    const authz = fcl.authz // NOTE: with FCL, proposer/payer/authz default to current user; explicit is fine too
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [],
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })
    console.log(txId)
    //sendToUnity("OnFlowTxSubmitted", txId)

    // Wait until sealed
    await fcl.tx(txId).onceSealed()
    //sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    //sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

// Keep a simple getter for current user
async function getFlowUser() {
  try {
    const user = await fcl.currentUser().snapshot()
    //sendToUnity("OnFlowUser", JSON.stringify(user || {}))
  } catch (e) {
    //sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

// Expose to window for Unity .jslib to access
if (typeof window !== "undefined") {
  window.FlowBridge = {
    setUnityInstance,
    connectFlow,
    disconnectFlow,
    createHeroCollection,
    getFlowUser,
  }
}