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

        try:
            from openai import OpenAI
        except ImportError as exc:
            raise RuntimeError("Install Python dependencies with `pip install -r requirements.txt`.") from exc

        client = OpenAI(api_key=settings.deepseek_api_key, base_url=settings.deepseek_base_url)

        try:
            response = client.chat.completions.create(
                model=settings.deepseek_model,
                messages=[
                    {
                        "role": "system",
                        "content": (
                            "You are a concise market analyst. Summarize macro and technical "
                            "context for an automated trading signal in one short paragraph."
                        ),
                    },
                    {"role": "user", "content": prompt},
                ],
                temperature=0.2,
                max_tokens=180,
            )
            summary = response.choices[0].message.content or ""
        except Exception as exc:
            return {
                "provider": "deepseek",
                "configured": True,
                "prompt": prompt,
                "summary": f"DeepSeek request failed: {exc}",
            }

        return {
            "provider": "deepseek",
            "configured": True,
            "prompt": prompt,
            "summary": summary,
        }
