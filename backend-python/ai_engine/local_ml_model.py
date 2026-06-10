"""Local ML model placeholder for XGBoost/LSTM signals."""


class LocalMlModel:
    def predict(self, features: dict) -> dict:
        score = float(features.get("momentum", 0.0))
        label = "buy" if score > 0.65 else "sell" if score < -0.65 else "hold"

        return {"score": score, "label": label, "features": features}
