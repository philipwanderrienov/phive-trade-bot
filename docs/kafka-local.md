# Local Kafka Development

Panduan ini untuk memahami dan menjalankan alur:

```text
Python producer -> Kafka topic trading.signals -> .NET MarketWorker -> PostgreSQL -> API/Angular
```

## Konsep singkat

- **Broker** adalah server Kafka. Di lokal, broker berjalan di `localhost:9092`.
- **Topic** adalah nama jalur pesan. Proyek ini memakai `trading.signals`.
- **Producer** adalah aplikasi yang mengirim pesan. Di proyek ini: `backend-python/kafka_services/producer.py`.
- **Consumer** adalah aplikasi yang membaca pesan. Di proyek ini: `TradingBot.MarketWorker`.
- **Consumer group** adalah nama grup pembaca. Offset disimpan per group, jadi worker tahu pesan mana yang sudah pernah dibaca.
- **Offset** adalah nomor urut pesan di topic. Kalau worker mati lalu hidup lagi, ia lanjut dari offset terakhir yang sudah di-commit.

Kafka cocok di sini karena Python tidak perlu tahu detail database .NET, dan .NET worker tidak perlu memanggil Python langsung. Mereka hanya sepakat pada topic dan bentuk JSON.

## 1. Siapkan environment

Pastikan `.env` punya nilai ini:

```env
KAFKA_BOOTSTRAP_SERVERS=localhost:9092
KAFKA_CONSUMER_GROUP=tradingbot-market-worker
SIGNAL_TOPIC=trading.signals
POSTGRES_CONNECTION=Host=localhost;Port=5432;Database=phive-trade-bot;Username=postgres;Password=...
```

Install dependency Python:

```bash
make python-install
```

## 2. Jalankan Kafka lokal

Nyalakan Docker Desktop dulu, lalu:

```bash
make kafka-up
```

Target ini hanya menyalakan `zookeeper`, `kafka`, dan `kafka-init`. Service `kafka-init` membuat topic `trading.signals` bila belum ada.

Cek topic:

```bash
make kafka-topic
```

Kalau berhasil, Anda akan melihat detail topic `trading.signals`.

## 3. Jalankan worker .NET

Di terminal kedua:

```bash
make dotnet-worker
```

Output yang sehat biasanya memuat:

```text
Kafka consumer subscribed to trading.signals at localhost:9092.
```

Kalau muncul `Connection refused`, broker Kafka belum hidup atau port `9092` belum terbuka.

## 4. Publish pesan test dari Python

Di terminal ketiga:

```bash
make python-publish-test SYMBOL=AAPL
```

Script ini memakai `backend-python/dev_publish_signal.py` dan mengirim satu JSON dummy tanpa memanggil Yahoo Finance, Kalshi, Polymarket, atau DeepSeek. Ini latihan terbaik untuk membuktikan koneksi Kafka dulu.

Worker .NET harus menampilkan log seperti:

```text
Persisted Kafka signal Buy AAPL with confidence 72.5.
```

## 5. Lihat pesan mentah di Kafka

Untuk belajar, Anda bisa membuka console consumer:

```bash
make kafka-console
```

Lalu jalankan lagi:

```bash
make python-publish-test SYMBOL=TSLA
```

Console consumer akan menampilkan JSON mentah yang dikirim Python.

## 6. Jalankan alur aplikasi

Setelah Kafka dan database siap:

```bash
make dotnet-api
make dotnet-worker
make angular-start
```

Untuk scheduler asli:

```bash
make python-run
```

`python-run` menjalankan `backend-python/scheduler.py`, mengambil sinyal dari pipeline, lalu publish ke Kafka setiap 5 menit. Gunakan ini setelah jalur test dummy sudah berhasil.

## Bentuk JSON yang disepakati

Python mengirim field snake_case seperti ini:

```json
{
  "symbol": "AAPL",
  "market": "NASDAQ",
  "recommendation": "Buy",
  "confidence": 72.5,
  "model_score": 0.73,
  "macro_events": 1,
  "entry_price": 123.45,
  "last_close": 123.45,
  "momentum": 0.018,
  "source": "python-dev-publisher",
  "rationale": "Local Kafka development test",
  "generated_at": "2026-06-13T00:00:00+00:00"
}
```

.NET membaca JSON ini lewat `KafkaTradingSignal`, lalu worker mengubahnya menjadi entity `Signal` dan menyimpan ke PostgreSQL.

## Troubleshooting cepat

- `Cannot connect to Docker daemon`: Docker Desktop belum jalan.
- `localhost:9092 Connection refused`: Kafka belum running, atau healthcheck belum selesai.
- `No module named kafka`: jalankan `make python-install`.
- Worker hidup tapi tidak insert data: cek `.env` `POSTGRES_CONNECTION`, topic `SIGNAL_TOPIC`, dan log worker.
- Pesan tidak muncul lagi saat memakai `kafka-console --from-beginning`: consumer group yang sama bisa sudah menyimpan offset. Untuk latihan mentah, console consumer bawaan tanpa group eksplisit biasanya cukup.

## Pola develop yang nyaman

1. Mulai dari `make kafka-up`.
2. Jalankan `make dotnet-worker`.
3. Kirim pesan dengan `make python-publish-test SYMBOL=AAPL`.
4. Pastikan worker menyimpan data ke PostgreSQL.
5. Baru lanjut ke `make python-run` untuk pipeline asli.

Dengan pola ini, kalau ada error, sumbernya lebih mudah dipisahkan: Kafka, Python producer, .NET consumer, atau database.
