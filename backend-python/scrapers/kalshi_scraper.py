"""Kalshi macro market scraper."""

from urllib.request import Request, urlopen
import json


class KalshiScraper:
    base_url = "https://api.elections.kalshi.com/trade-api/v2"

    def fetch_macro_markets(self) -> list[dict]:
        request = Request(f"{self.base_url}/markets?limit=20", headers={"Accept": "application/json"})

        with urlopen(request, timeout=15) as response:
            payload = json.loads(response.read().decode("utf-8"))

        return payload.get("markets", [])
