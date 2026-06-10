# Phive Bot Architecture

Dokumen ini mengikuti diagram `assets/arsitektur aplikasi phive bot.png`.

## Layer 1 - Data Source

- Yahoo Finance via `yfinance` untuk harga, OHLCV, dan chart.
- Polymarket dan Kalshi untuk data probabilitas event dan sinyal makro.

## Layer 2 - Python Scraper and AI Engine

- `scrapers/`: adapter data source.
- `ai_engine/local_ml_model.py`: prediksi tren lokal.
- `ai_engine/deepseek_client.py`: analisis NLP, makro, dan sentimen.
- `pipeline.py`: menyatukan data, model lokal, dan analisis AI menjadi sinyal matang.
- `kafka_services/producer.py`: publish sinyal ke Kafka topic `trading.signals`.

## Layer 3 - .NET Core Microservices

- `TradingBot.MarketWorker`: consumer Kafka dan pemicu engine.
- `TradingBot.Engine`: indicator engine, strategy engine, risk manager, signal service, order recommender.
- `TradingBot.Api`: CRUD, recommendation, reporting, backtesting, housekeeping.
- `TradingBot.Gateway`: pintu masuk API dan WebSocket/SignalR.
- `TradingBot.Core`: entity, DTO, dan shared contract.

## Layer 4 - Observability

- Logging memakai `Microsoft.Extensions.Logging`.
- Grafana disediakan di `docker-compose.yml` untuk metrics dan logs dashboard.

## Layer 5 - Frontend

- Angular dashboard untuk display sinyal.
- SignalR service disiapkan untuk push notification real-time.
- Feature folders: dashboard, backtesting, reports.
