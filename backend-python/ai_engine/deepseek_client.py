"""DeepSeek client placeholder."""

from config.settings import settings


class DeepSeekClient:
    def analyze(self, prompt: str) -> dict:
        if not settings.deepseek_api_key:
            return {
                "provider": "deepseek",
                "configured": False,
                "summary": "DEEPSEEK_API_KEY is not configured.",
            }

        return {
            "provider": "deepseek",
            "configured": True,
            "prompt": prompt,
            "summary": "Client wiring is ready for the API call implementation.",
        }
