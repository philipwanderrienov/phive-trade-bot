CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE IF NOT EXISTS market_data (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    symbol varchar(32) NOT NULL,
    market varchar(64) NOT NULL DEFAULT '',
    open numeric(18, 6) NOT NULL DEFAULT 0,
    high numeric(18, 6) NOT NULL DEFAULT 0,
    low numeric(18, 6) NOT NULL DEFAULT 0,
    close numeric(18, 6) NOT NULL DEFAULT 0,
    price numeric(18, 6) NOT NULL DEFAULT 0,
    volume numeric(20, 4) NOT NULL DEFAULT 0,
    source varchar(64) NOT NULL DEFAULT 'manual',
    timestamp timestamptz NOT NULL DEFAULT now(),
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_market_data_symbol_timestamp
    ON market_data (symbol, timestamp DESC);

CREATE TABLE IF NOT EXISTS signals (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    symbol varchar(32) NOT NULL,
    market varchar(64) NOT NULL DEFAULT '',
    recommendation varchar(16) NOT NULL,
    confidence numeric(8, 4) NOT NULL DEFAULT 0,
    entry_price numeric(18, 6) NOT NULL DEFAULT 0,
    stop_loss numeric(18, 6) NOT NULL DEFAULT 0,
    target_price numeric(18, 6) NOT NULL DEFAULT 0,
    risk_reward_ratio numeric(8, 4) NOT NULL DEFAULT 0,
    rationale varchar(1024) NOT NULL DEFAULT '',
    source varchar(64) NOT NULL DEFAULT 'engine',
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_signals_symbol_created_at
    ON signals (symbol, created_at DESC);

INSERT INTO market_data (
    symbol,
    market,
    open,
    high,
    low,
    close,
    price,
    volume,
    source,
    timestamp
) 
SELECT
    seed.symbol,
    seed.market,
    seed.open,
    seed.high,
    seed.low,
    seed.close,
    seed.price,
    seed.volume,
    seed.source,
    seed.timestamp
FROM (
    VALUES
    ('AAPL', 'NASDAQ', 190.800000, 198.000000, 189.200000, 196.200000, 196.200000, 1625000, 'seed-sql', now() - interval '3 days'),
    ('TSLA', 'NASDAQ', 181.200000, 183.000000, 174.900000, 175.300000, 175.300000, 1485000, 'seed-sql', now() - interval '2 days'),
    ('BTC-USD', 'Crypto', 69080.000000, 71500.000000, 68400.000000, 71120.000000, 71120.000000, 1250000, 'seed-sql', now() - interval '1 day')
) AS seed(symbol, market, open, high, low, close, price, volume, source, timestamp)
WHERE NOT EXISTS (
    SELECT 1
    FROM market_data existing
    WHERE existing.symbol = seed.symbol
      AND existing.source = seed.source
);

INSERT INTO signals (
    symbol,
    market,
    recommendation,
    confidence,
    entry_price,
    stop_loss,
    target_price,
    risk_reward_ratio,
    rationale,
    source
) 
SELECT
    seed.symbol,
    seed.market,
    seed.recommendation,
    seed.confidence,
    seed.entry_price,
    seed.stop_loss,
    seed.target_price,
    seed.risk_reward_ratio,
    seed.rationale,
    seed.source
FROM (
    VALUES
    ('AAPL', 'NASDAQ', 'Buy', 71.0000, 196.200000, 190.800000, 209.100000, 2.3500, 'Initial SQL seed signal for local dashboard testing.', 'seed-sql'),
    ('TSLA', 'NASDAQ', 'Watch', 58.0000, 175.300000, 183.100000, 158.200000, 2.1800, 'Initial SQL seed signal for local dashboard testing.', 'seed-sql'),
    ('BTC-USD', 'Crypto', 'Buy', 76.0000, 71120.000000, 68110.000000, 78340.000000, 2.4000, 'Initial SQL seed signal for local dashboard testing.', 'seed-sql')
) AS seed(symbol, market, recommendation, confidence, entry_price, stop_loss, target_price, risk_reward_ratio, rationale, source)
WHERE NOT EXISTS (
    SELECT 1
    FROM signals existing
    WHERE existing.symbol = seed.symbol
      AND existing.source = seed.source
);
