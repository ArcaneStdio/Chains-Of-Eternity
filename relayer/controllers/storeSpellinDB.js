import { pool } from "./db.js"
import crypto from 'crypto';


export async function addSpell(req, res) {
  try {
    const {
      address,
      json_data
    } = req.body

    function hashSHA256(input) {
        return crypto.createHash('sha256').update(input, 'utf8').digest('hex');
    }
    const hashed = hashSHA256(json_data);

    try {
        
        const { rows } = await pool.query(
            `SELECT EXISTS (
                SELECT 1 FROM spells WHERE hashed_data = $1
            ) AS hash_exists`,
            [hashed]
        );

        if (rows[0].hash_exists) {
            
            return { success: false, message: 'This hash already exists!' };
        } else {
            const insertResult = await pool.query(
                `INSERT INTO spells (address, hashed_data)
                 VALUES ($1, $2)
                 RETURNING *`,
                [address, hashed]
            );
            return { success: true, data: insertResult.rows[0], data: hashed };
        }
      } catch(err){
        console.error("Error inserting spell:", err)
        res.status(500).json({ success: false, error: err.message })

      }
    } catch (err) {
    console.error("Error inserting spell:", err)
    res.status(500).json({ success: false, error: err.message })
  }
}
  
