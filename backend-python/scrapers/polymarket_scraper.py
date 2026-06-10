"""Polymarket macro market scraper."""

from urllib.parse import urlencode
from urllib.request import Request, urlopen
import json


class PolymarketScraper:
    base_url = "https://gamma-api.polymarket.com/markets"

    def fetch_macro_markets(self, limit: int = 20) -> list[dict]:
        query = urlencode({"active": "true", "closed": "false", "limit": limit})
        request = Request(f"{self.base_url}?{query}", headers={"Accept": "application/json"})

        with urlopen(request, timeout=15) as response:
            payload = json.loads(response.read().decode("utf-8"))

        if isinstance(payload, list):
            return payload

        return payload.get("markets", [])
