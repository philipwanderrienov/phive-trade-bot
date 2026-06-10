"""APScheduler orchestration for periodic scraping."""

from apscheduler.schedulers.blocking import BlockingScheduler

from pipeline import SignalPipeline


def collect_signals() -> None:
    pipeline = SignalPipeline()
    signal = pipeline.build_signal("AAPL")
    print(f"Generated signal: {signal.to_dict()}")


def start_scheduler() -> None:
    scheduler = BlockingScheduler()
    scheduler.add_job(collect_signals, "interval", minutes=5, id="collect-signals")
    collect_signals()
    scheduler.start()
