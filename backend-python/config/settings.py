"""Application settings for the Python backend."""

from dataclasses import dataclass
import os
from pathlib import Path

try:
    from dotenv import load_dotenv
except ImportError:  # pragma: no cover - dependency is listed in requirements.
    load_dotenv = None

if load_dotenv:
    load_dotenv(Path(__file__).resolve().parents[2] / ".env")


@dataclass(frozen=True)
class Settings:
    deepseek_api_key: str = os.getenv("DEEPSEEK_API_KEY", "")
    deepseek_base_url: str = os.getenv("DEEPSEEK_BASE_URL", "")
    deepseek_model: str = os.getenv("DEEPSEEK_MODEL", "deepseek-chat")
    kalshi_api_key: str = os.getenv("KALSHI_API_KEY", "")
    kalshi_base_url: str = os.getenv("KALSHI_BASE_URL", "")
    polymarket_api_key: str = os.getenv("POLYMARKET_API_KEY", "")
    polymarket_base_url: str = os.getenv("POLYMARKET_BASE_URL", "")
    kafka_bootstrap_servers: str = os.getenv("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092")
    signal_topic: str = os.getenv("SIGNAL_TOPIC", "trading.signals")
    postgres_connection: str = os.getenv("POSTGRES_CONNECTION", "")


settings = Settings()
