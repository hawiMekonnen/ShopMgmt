# Shop Management — Ethiopian Airlines

## Member 1 — Material Management

### On-hand and stock value

- **On-hand** = sum of `StockBatch.QuantityReceived` − sum of `MaterialUsage.QuantityUsed`
- **Stock value** = on-hand × `Material.UnitPrice`

Usage recording is owned by Member 2; this module reads usages for inventory only.

### Run locally

#### Git Bash on Windows (fix “No .NET SDKs were found”)

Git Bash often uses the **32-bit** `dotnet` under `Program Files (x86)`, which has **no SDKs**. Use the 64-bit SDK instead:

```bash
source scripts/setup-dotnet-path.sh
```

Or run the helper scripts (they source that automatically):

```bash
./scripts/db-update.sh    # migrations
./scripts/run-api.sh      # API https://localhost:7120
```

**Permanent fix:** In Windows Environment Variables, move `C:\Program Files\dotnet` **above** `C:\Program Files (x86)\dotnet` in `Path`, or remove the x86 entry. Restart the terminal.

**PowerShell / CMD** usually work without this; the issue is specific to Git Bash PATH order.

#### Commands (after `source scripts/setup-dotnet-path.sh`)

1. **Database** (SQL Server LocalDB):

   ```bash
   dotnet ef database update --project ShopMgmt.Infrastructure --startup-project ShopMgmt.WebAPI
   ```

   First time only, if `dotnet ef` is missing:

   ```bash
   dotnet tool install --global dotnet-ef
   export PATH="$HOME/.dotnet/tools:$PATH"
   ```

2. **API** (`https://localhost:7120`):

   ```bash
   dotnet run --project ShopMgmt.WebAPI
   ```

UI is built separately in React (another folder). CORS allows `http://localhost:3000` and `http://localhost:5173` by default; add more origins in `ShopMgmt.WebAPI/Program.cs` if needed.

### Tests

```bash
dotnet test ShopMgmt.Tests
```

### API endpoints

| Resource | Routes |
|----------|--------|
| Categories | `GET/POST /api/categories`, `GET/PUT/DELETE /api/categories/{id}` |
| Materials | `GET/POST /api/materials`, `GET/PUT/DELETE /api/materials/{id}`, `GET /api/materials/{id}/inventory` |
| Stock batches | `GET/POST /api/materials/{materialId}/batches`, `DELETE /api/materials/{materialId}/batches/{batchId}` |
