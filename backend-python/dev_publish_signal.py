"""Publish one local test signal to Kafka without calling external market APIs."""

from datetime import datetime, timezone
import sys

from kafka_services.producer import SignalProducer


def build_payload(symbol: str) -> dict:
    now = datetime.now(timezone.utc).isoformat()

    return {
        "symbol": symbol,
        "market": "NASDAQ",
        "recommendation": "Buy",
        "confidence": 72.5,
        "model_score": 0.73,
        "macro_events": 1,
        "entry_price": 123.45,
        "last_close": 123.45,
        "momentum": 0.018,
        "source": "python-dev-publisher",
        "rationale": f"Local Kafka development test for {symbol} at {now}.",
        "generated_at": now,
    }


def main() -> None:
    symbol = sys.argv[1] if len(sys.argv) > 1 else "AAPL"
    payload = build_payload(symbol.upper())
    producer = None

    try:
        producer = SignalProducer()
        producer.publish(payload)
    except RuntimeError as exc:
        print(f"Failed to publish Kafka test signal: {exc}")
        raise SystemExit(1) from exc
    finally:
        if producer is not None:
            producer.close()

    print(f"Published Kafka test signal: {payload}")


if __name__ == "__main__":
    main()
