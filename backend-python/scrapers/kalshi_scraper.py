"""Kalshi macro market scraper."""

from urllib.request import Request, urlopen
import json

from config.settings import settings


class KalshiScraper:
    def __init__(self, base_url: str | None = None) -> None:
        self.base_url = base_url or settings.kalshi_base_url

    def fetch_macro_markets(self) -> list[dict]:
        request = Request(f"{self.base_url}/markets?limit=20", headers={"Accept": "application/json"})

        with urlopen(request, timeout=15) as response:
            payload = json.loads(response.read().decode("utf-8"))

        return payload.get("markets", [])
