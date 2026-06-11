"""Polymarket macro market scraper."""

from urllib.parse import urlencode
from urllib.request import Request, urlopen
import json

from config.settings import settings


class PolymarketScraper:
    def __init__(self, base_url: str | None = None) -> None:
        self.base_url = base_url or settings.polymarket_base_url

    def fetch_macro_markets(self, limit: int = 20) -> list[dict]:
        query = urlencode({"active": "true", "closed": "false", "limit": limit})
        request = Request(f"{self.base_url}?{query}", headers={"Accept": "application/json"})

        with urlopen(request, timeout=15) as response:
            payload = json.loads(response.read().decode("utf-8"))

        if isinstance(payload, list):
            return payload

        return payload.get("markets", [])
