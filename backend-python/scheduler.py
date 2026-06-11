"""APScheduler orchestration for periodic scraping."""

from apscheduler.schedulers.blocking import BlockingScheduler

from kafka_services.producer import SignalProducer
from pipeline import SignalPipeline


def collect_signals() -> None:
    pipeline = SignalPipeline()
    producer = SignalProducer()

    for symbol in ["AAPL", "TSLA", "BTC-USD", "EURUSD=X"]:
        signal = pipeline.build_signal(symbol)
        payload = signal.to_dict()
        producer.publish(payload)
        print(f"Published signal: {payload}")


def start_scheduler() -> None:
    scheduler = BlockingScheduler()
    scheduler.add_job(collect_signals, "interval", minutes=5, id="collect-signals")
    collect_signals()
    scheduler.start()
