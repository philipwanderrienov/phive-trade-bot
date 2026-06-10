.PHONY: infra-up infra-down python-run dotnet-build angular-start

infra-up:
	docker compose up -d

infra-down:
	docker compose down

python-run:
	cd backend-python && python3 main.py

dotnet-build:
	dotnet build backend-dotnet/TradingBot.sln

angular-start:
	cd frontend-angular && npm start
