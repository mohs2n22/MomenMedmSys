# Deployment Files for MomenMedmSys

## Files Overview

| File | Purpose |
|------|---------|
| `Dockerfile` | Container image build for the ASP.NET Core app |
| `docker-compose.yml` | Run app locally with containers |
| `render.yaml` | Render.com infrastructure-as-code config |
| `.dockerignore` | Excludes unnecessary files from Docker builds |
| `web.config` | IIS deployment configuration (Windows servers) |
| `nginx.conf` | Nginx reverse proxy config (Linux servers) |

---

## Database

This project uses **SQLite** (file-based database). No external database server needed.

The database file `medmsys.db` is created automatically in the app's directory on first run.
All seed data (admin user + sample devices) is also created automatically.

---

## Deployment Methods

### Option 1: Render.com (Recommended — Free)

1. Push your project to a GitHub repository
2. Go to [Render Dashboard](https://dashboard.render.com) → New Web Service
3. Connect your GitHub repo
4. **Runtime:** `Docker`
5. **Dockerfile Path:** `deploy/Dockerfile`
6. **Branch:** `master`
7. **Instance:** `Free`
8. **No environment variables needed** (SQLite is file-based)
9. Click **Create Web Service**

Render builds and deploys automatically. The app will be available at `https://your-app.onrender.com`

### Option 2: Docker (Any VPS or Cloud)
```bash
cd deploy
docker-compose up -d
```

### Option 3: Manual on Linux
```bash
dotnet publish MomenMedmSys.Web -c Release -o /app/publish
dotnet MomenMedmSys.Web.dll --urls http://localhost:5000
```
Then configure Nginx using `deploy/nginx.conf`.

### Option 4: IIS (Windows)
1. Install .NET 8 Hosting Bundle on the server
2. Publish: `dotnet publish -c Release -o C:\inetpub\apps\mems`
3. Copy `web.config` to the published folder
4. Create IIS site pointing to the folder with an app pool using "No Managed Code"
