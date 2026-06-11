# Local PostgreSQL Setup

The .NET API and MarketWorker read the database connection in this order:

1. `POSTGRES_CONNECTION` environment variable.
2. `ConnectionStrings:Postgres` in `appsettings.json` / `appsettings.Development.json`.

Default local connection:

```powershell
Host=localhost;Port=5432;Database=tradingbot;Username=tradingbot;Password=tradingbot
```

Run the schema and seed script:

```powershell
$env:PGPASSWORD = "your_postgres_password"
psql -h localhost -p 5432 -U your_postgres_user -d your_database -f docs/postgres-init.sql
```

Point the .NET apps at that database:

```powershell
$env:POSTGRES_CONNECTION = "Host=localhost;Port=5432;Database=your_database;Username=your_postgres_user;Password=your_postgres_password"
dotnet run --project backend-dotnet\TradingBot.Api\TradingBot.Api.csproj --urls http://127.0.0.1:5000
```

To use the repository defaults, create this database/user first:

```sql
CREATE USER tradingbot WITH PASSWORD 'tradingbot';
CREATE DATABASE tradingbot OWNER tradingbot;
GRANT ALL PRIVILEGES ON DATABASE tradingbot TO tradingbot;
```
