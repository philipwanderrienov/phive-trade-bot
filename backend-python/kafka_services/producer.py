"""Kafka producer for finalized trading signals."""

import json

from config.settings import settings


class SignalProducer:
    def __init__(self) -> None:
        try:
            from kafka import KafkaProducer
        except ImportError as exc:
            raise RuntimeError("Install Python dependencies with `pip install -r requirements.txt`.") from exc

        self._producer = KafkaProducer(
            bootstrap_servers=settings.kafka_bootstrap_servers,
            value_serializer=lambda value: json.dumps(value).encode("utf-8"),
        )

    def publish(self, signal: dict) -> None:
        self._producer.send(settings.signal_topic, signal)
        self._producer.flush()
