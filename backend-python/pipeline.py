"""Layer 2 orchestration: data fetcher, local model, AI analysis, and signal payload."""

from dataclasses import dataclass, asdict
from datetime import datetime, timezone

from ai_engine.deepseek_client import DeepSeekClient
from ai_engine.local_ml_model import LocalMlModel
from scrapers.kalshi_scraper import KalshiScraper
from scrapers.polymarket_scraper import PolymarketScraper
from scrapers.yfinance_scraper import YFinanceScraper


@dataclass(frozen=True)
class TradingSignal:
    symbol: str
    market: str
    recommendation: str
    confidence: float
    model_score: float
    macro_events: int
    entry_price: float
    last_close: float
    momentum: float
    source: str
    rationale: str
    generated_at: str

    def to_dict(self) -> dict:
        return asdict(self)


class SignalPipeline:
    def __init__(self) -> None:
        self._prices = YFinanceScraper()
        self._kalshi = KalshiScraper()
        self._polymarket = PolymarketScraper()
        self._model = LocalMlModel()
        self._deepseek = DeepSeekClient()

    def build_signal(self, symbol: str) -> TradingSignal:
        price_data = self._prices.fetch_prices(symbol, period="5d", interval="1d")
        macro_events = self._safe_macro_count()
        momentum = self._calculate_momentum(price_data["prices"])
        model_result = self._model.predict({"momentum": momentum, "macro_events": macro_events})
        ai_result = self._deepseek.analyze(f"Analyze macro and technical context for {symbol}.")

        confidence = self._confidence(model_result["score"], macro_events, ai_result["configured"])

        return TradingSignal(
            symbol=symbol,
            market=self._infer_market(symbol),
            recommendation=model_result["label"],
            confidence=confidence,
            model_score=model_result["score"],
            macro_events=macro_events,
            entry_price=self._last_close(price_data["prices"]),
            last_close=self._last_close(price_data["prices"]),
            momentum=momentum,
            source="python-scheduler",
            rationale=(
                f"Python pipeline signal with momentum {momentum}, "
                f"{macro_events} macro events, and DeepSeek configured={ai_result['configured']}. "
                f"{str(ai_result['summary'])[:240]}"
            ),
            generated_at=datetime.now(timezone.utc).isoformat(),
        )

    def _safe_macro_count(self) -> int:
        try:
            return len(self._kalshi.fetch_macro_markets()) + len(self._polymarket.fetch_macro_markets())
        except Exception:
            return 0

    @staticmethod
    def _calculate_momentum(prices: list[dict]) -> float:
        if len(prices) < 2:
            return 0.0

        first = prices[0]["close"]
        last = prices[-1]["close"]

        if first == 0:
            return 0.0

        return round((last - first) / first, 4)

    @staticmethod
    def _last_close(prices: list[dict]) -> float:
        if not prices:
            return 0.0

        return round(float(prices[-1]["close"]), 6)

    @staticmethod
    def _infer_market(symbol: str) -> str:
        if symbol.endswith("-USD"):
            return "Crypto"

        if symbol.endswith("=X"):
            return "Forex"

        return "NASDAQ"

    @staticmethod
    def _confidence(score: float, macro_events: int, ai_configured: bool) -> float:
        base = min(abs(score) * 100, 80)
        macro_boost = min(macro_events * 0.5, 10)
        ai_boost = 10 if ai_configured else 0

        return round(min(base + macro_boost + ai_boost, 99), 2)
