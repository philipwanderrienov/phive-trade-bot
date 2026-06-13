.PHONY: infra-up infra-down kafka-up kafka-topic kafka-console python-install python-run python-publish-test dotnet-build dotnet-api dotnet-worker angular-start

SYMBOL ?= AAPL

infra-up:
	docker compose up -d

infra-down:
	docker compose down

kafka-up:
	docker compose up -d zookeeper kafka kafka-init

kafka-topic:
	docker exec tradingbot-kafka kafka-topics --bootstrap-server localhost:9092 --describe --topic trading.signals

kafka-console:
	docker exec -it tradingbot-kafka kafka-console-consumer --bootstrap-server localhost:9092 --topic trading.signals --from-beginning

python-install:
	cd backend-python && python3 -m pip install -r requirements.txt

python-run:
	cd backend-python && python3 main.py

python-publish-test:
	cd backend-python && python3 dev_publish_signal.py $(SYMBOL)

dotnet-build:
	dotnet build backend-dotnet/TradingBot.sln

dotnet-api:
	dotnet run --project backend-dotnet/TradingBot.Api/TradingBot.Api.csproj

dotnet-worker:
	dotnet run --project backend-dotnet/TradingBot.MarketWorker/TradingBot.MarketWorker.csproj

angular-start:
	cd frontend-angular && npm start
