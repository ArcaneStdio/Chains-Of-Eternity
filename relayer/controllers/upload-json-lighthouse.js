// lighthouseController.js
import dotenv from "dotenv";
import axios from "axios";
import lighthouse from "@lighthouse-web3/sdk";
import { addSpell } from "./storeSpellinDB.js";
dotenv.config();

/**
 * Upload encrypted JSON to Lighthouse
 */
export const uploadJsonToLighthouse = async (req, res) => {
  try {
    const { jsonData, publicKey, signedMessage } = req.body;
    console.log("Received upload request:", { jsonData, publicKey, signedMessage });
    const checkSpell = await addSpell()
    if (checkSpell.success){
      const response = await lighthouse.textUploadEncrypted(
        checkSpell.data,
        process.env.API_KEY, // must be set in .env
        publicKey,
        signedMessage
      );
      console.log("Lighthouse upload response:", response);
      return res.status(200).json({
        success: true,
        data: response,
      });
    } else {
      return res.json({
        success: false,
        message: "Spell already exits!"
      })
    }
  } catch (error) {
    return res.status(500).json({
      success: false,
      message: error.message,
    });
  }
};

/**
 * Apply zkTLS conditions for a CID
 */
export const applyZkConditions = async (req, res) => {
  try {
    const { cid, publicKey, signedMessage, conditions } = req.body;

    const nodeId = [1, 2, 3, 4, 5];
    const nodeUrl = nodeId.map(
      (id) => `https://encryption.lighthouse.storage/api/setZkConditions/${id}`
    );

    const config = {
      method: "post",
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${signedMessage}`,
      },
    };

    const apidata = { address: publicKey, cid, conditions };

    const results = [];
    for (const url of nodeUrl) {
      const resp = await axios({ url, data: apidata, ...config });
      results.push(resp.data);
    }

    return res.json({ success: true, data: results });
  } catch (err) {
    return res.status(500).json({ success: false, message: err.message });
  }
};

/**
 * Verify zkTLS proof + decrypt JSON file
 */
export const verifyAndDecrypt = async (req, res) => {
  try {
    const { cid, publicKey, signedMessage, proof } = req.body;

    const nodeId = [1, 2, 3, 4, 5];
    const nodeUrl = nodeId.map(
      (id) =>
        `https://encryption.lighthouse.storage/api/verifyZkConditions/${id}`
    );

    const config = {
      method: "post",
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${signedMessage}`,
      },
    };

    const apidata = { address: publicKey, cid, proof };

    const shards = [];
    for (const url of nodeUrl) {
      const resp = await axios({ url, data: apidata, ...config });
      shards.push(resp.data.payload);
    }

    const { masterKey, error } = await lighthouse.recoverKey(shards);
    if (error) throw new Error(error);

    const decrypted = await lighthouse.decryptFile(cid, masterKey);

    const jsonData = JSON.parse(Buffer.from(decrypted).toString());

    return res.json({ success: true, data: jsonData });
  } catch (err) {
    return res.status(500).json({ success: false, message: err.message });
  }
};

/**
 * Fetch zkTLS conditions for a CID
 */
export const getZkConditions = async (req, res) => {
  try {
    const { cid, signedMessage } = req.body;

    const response = await axios({
      url: `https://encryption.lighthouse.storage/api/getZkConditions/${cid}`,
      method: "get",
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${signedMessage}`,
      },
    });

    return res.json({ success: true, data: response.data });
  } catch (err) {
    return res.status(500).json({ success: false, message: err.message });
  }
};
