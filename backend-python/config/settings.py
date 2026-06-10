"""Application settings for the Python backend."""

from dataclasses import dataclass
import os

try:
    from dotenv import load_dotenv
except ImportError:  # pragma: no cover - dependency is listed in requirements.
    load_dotenv = None

if load_dotenv:
    load_dotenv()


@dataclass(frozen=True)
class Settings:
    deepseek_api_key: str = os.getenv("DEEPSEEK_API_KEY", "")
    kalshi_api_key: str = os.getenv("KALSHI_API_KEY", "")
    polymarket_api_key: str = os.getenv("POLYMARKET_API_KEY", "")
    kafka_bootstrap_servers: str = os.getenv("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092")
    signal_topic: str = os.getenv("SIGNAL_TOPIC", "trading.signals")


settings = Settings()
