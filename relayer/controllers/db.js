import pkg from "pg"
import dotenv from "dotenv"
dotenv.config()

const { Pool } = pkg

export const pool = new Pool({
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  host: process.env.DB_HOST,
  port: process.env.DB_PORT || 5432,
  database: process.env.DB_NAME,
  ssl: { rejectUnauthorized: false }
})

// Optional: test connection at startup
export async function testDatabaseConnection() {
  try {
    const result = await pool.query("SELECT NOW()")
    console.log("Database connected at:", result.rows[0].now)
  } catch (err) {
    console.error("Database connection failed:", err.message)
    process.exit(1)
  }
}
