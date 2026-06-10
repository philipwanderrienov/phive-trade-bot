"""Price data scraper using yfinance."""


class YFinanceScraper:
    def fetch_prices(self, symbol: str, period: str = "1mo", interval: str = "1d") -> dict:
        try:
            import yfinance as yf
        except ImportError as exc:
            raise RuntimeError("Install Python dependencies with `pip install -r requirements.txt`.") from exc

        ticker = yf.Ticker(symbol)
        history = ticker.history(period=period, interval=interval)

        rows = [
            {
                "timestamp": str(index),
                "open": float(row["Open"]),
                "high": float(row["High"]),
                "low": float(row["Low"]),
                "close": float(row["Close"]),
                "volume": int(row["Volume"]),
            }
            for index, row in history.iterrows()
        ]

        return {"symbol": symbol, "period": period, "interval": interval, "prices": rows}
