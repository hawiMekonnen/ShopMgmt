# AMOS-Style Demo Runbook (Ethiopian Airlines)

## Prerequisites

```bash
# Backend (Git Bash)
cd ShopMgmt
source scripts/setup-dotnet-path.sh
./scripts/db-update.sh
./scripts/run-api.sh
```

API: `https://localhost:7120` (or per launchSettings)

```bash
# Frontend
cd ShopMgmtFront
npm install
npm run dev
```

Open `http://localhost:5173` (or port shown in terminal).

## Demo users (Development seed)

| Email | Role | Password |
|-------|------|----------|
| technician@demo.et | Technician | Demo@123 |
| shopmanager@demo.et | ShopManager | Demo@123 |
| procurement@demo.et | Procurement | Demo@123 |
| admin@demo.et | Admin | Demo@123 |

Seed runs automatically on first API start when the database has no users.

## Role-based UI (after login)

Each user sees a **different sidebar** and **home screen**:

| Role | Home screen | Main menu items |
|------|-------------|-----------------|
| **Technician** | Search & Request | Search & Request, Request queue, Alerts (pickup only) |
| **ShopManager** | Request queue | Request queue, My shop stock (per-shop qty), Alerts, Search (optional) |
| **Procurement** | Procurement inbox | Stock by shop, Procurement inbox, Alerts |
| **Admin** | Dashboard | All items including Categories (admin) |
| **Finance** | Dashboard | Dashboard only (read-only ops) |

Use the quick-login tiles on the sign-in page to switch roles during the demo.

## 5-minute presenter script

1. **Technician** (`technician@demo.et`) — **Search & Request**
   - Open **Search & Request**
   - Search part `ET-AVN-WIRE` — show **Available** vs **On hand** (pending batches blocked)
   - Click **Request** for 1 spool — **work order required** (e.g. `ET-AUE / WO-4401`)

2. **Shop manager** (`shopmanager@demo.et`) — **Release for issue**
   - **Requests** — **Release for issue** on submitted request (reserves stock + **PickupReady** alert in one step)

3. **Technician** — **Pickup**
   - **Requests** — **Confirm pickup** on `ReadyForPickup` row (pre-seeded request also works)
   - Stock issues with sign-off fields on usage

4. **Procurement** (`procurement@demo.et`) — **Stock & inbox**
   - **Stock by shop** — on hand, available, min, **on order** (reorder flag)
   - **Procurement inbox** — low stock, quarantine, open requests (filter by shop)
   - **Mark reorder** on a line

5. **Optional** — Receive → Serviceability (API or existing material flows)
   - New batch starts **Pending**; pass inspection → **Serviceable** → counts toward **Available**

## Inventory rules (talk track)

- **On hand** = (Pending + Serviceable received) − usages  
- **Available** = Serviceable received − usages − **reserved** (approved/ready requests)  
- **Quarantined** stock excluded from available  
- Full Swiss-AS AMOS remains the system of record; this tool models the **shop counter slice**

## Key API endpoints

| Flow | Method |
|------|--------|
| Search | `GET /api/materials/search?partNumber=&aircraft=&shopId=` |
| Submit request | `POST /api/materialrequests` |
| Release / Issue | `PATCH /api/materialrequests/{id}/release` · `.../issue` |
| Defect return | `POST /api/materialreturns` |
| Procurement | `GET /api/procurement/actions` |

All write endpoints except login require `Authorization: Bearer {token}`.
