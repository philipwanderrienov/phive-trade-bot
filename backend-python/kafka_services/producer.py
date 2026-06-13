"""Kafka producer for finalized trading signals."""

import json
from typing import Any

from config.settings import settings


class SignalProducer:
    def __init__(self) -> None:
        try:
            from kafka import KafkaProducer
            from kafka.errors import KafkaError
            from kafka.serializer import Serializer
        except ImportError as exc:
            raise RuntimeError("Install Python dependencies with `pip install -r requirements.txt`.") from exc

        class JsonValueSerializer(Serializer):
            def serialize(self, topic: str, headers: list[tuple[str, bytes]], data: Any) -> bytes:
                return json.dumps(data).encode("utf-8")

        try:
            self._producer = KafkaProducer(
                bootstrap_servers=settings.kafka_bootstrap_servers,
                bootstrap_timeout_ms=5000,
                max_block_ms=5000,
                request_timeout_ms=10000,
                retries=3,
                value_serializer=JsonValueSerializer(),
            )
        except KafkaError as exc:
            raise RuntimeError(
                "Kafka broker is not reachable. Start it with `make kafka-up` "
                f"and verify KAFKA_BOOTSTRAP_SERVERS={settings.kafka_bootstrap_servers}."
            ) from exc

    def publish(self, signal: dict) -> None:
        self._producer.send(settings.signal_topic, signal)
        self._producer.flush()

    def close(self) -> None:
        self._producer.close()
