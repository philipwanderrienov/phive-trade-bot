# Trading Bot Platform

Starter project untuk Phive Trade Bot berdasarkan diagram arsitektur di
`assets/arsitektur aplikasi phive bot.png`.

- `backend-python`: scraper, AI engine, scheduler, dan Kafka producer.
- `backend-dotnet`: gateway, shared core, worker, engine, dan Web API.
- `frontend-angular`: dashboard Angular untuk rekomendasi, backtesting, dan laporan.
- `docs/architecture.md`: ringkasan layer dan alur data.

## Local Infrastructure

```bash
docker compose up -d
```

Service lokal:

- PostgreSQL: `localhost:5432`
- Kafka: `localhost:9092`
- Grafana: `localhost:3000`

## Development Commands

```bash
cp .env.example .env
make infra-up
make dotnet-build
make python-run
make angular-start
```

## Development Flow

1. Python mengambil OHLCV dan macro event, lalu membangun sinyal.
2. Sinyal dipublish ke Kafka topic `trading.signals`.
3. Worker .NET membaca event dan memicu engine teknikal, strategi, risk, dan rekomendasi.
4. API dan Gateway menampilkan CRUD, backtesting, reporting, dan realtime notification.
5. Angular dashboard menampilkan sinyal untuk user dan dapat diteruskan ke aplikasi broker.
