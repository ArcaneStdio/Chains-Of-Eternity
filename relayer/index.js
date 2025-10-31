import express from "express";
import flowRoutes from "./routes/Route.js";
import cors from "cors"
import {testDatabaseConnection} from "./controllers/db.js"
const app = express();
const port = 3000;

app.use(cors())
app.use(express.json());

app.use("", flowRoutes);
await testDatabaseConnection()
app.listen(port, () => {
  console.log(`Relayer running at http://localhost:${port}`);
});
