import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Subscription, catchError, forkJoin, of } from 'rxjs';
import { ApiService, Recommendation, ReportSummary } from '../../core/services/api.service';
import { SignalrService } from '../../core/services/signalr.service';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  imports: [DecimalPipe]
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly api = inject(ApiService);
  private readonly signalr = inject(SignalrService);
  private readonly subscriptions = new Subscription();

  signals: Recommendation[] = fallbackSignals;
  report: ReportSummary = fallbackReport;
  isLoading = false;
  source: 'api' | 'fallback' = 'fallback';

  get averageConfidence(): number {
    if (this.signals.length === 0) {
      return 0;
    }

    return this.signals.reduce((total, signal) => total + signal.confidence, 0) / this.signals.length;
  }

  ngOnInit(): void {
    this.refresh();
    this.signalr.connect().catch(() => undefined);
    this.subscriptions.add(
      this.signalr.signalCreated$.subscribe((signal) => {
        this.signals = [
          signal,
          ...this.signals.filter((existing) => existing.symbol !== signal.symbol)
        ].slice(0, 25);
        this.report = {
          ...this.report,
          activeSignals: this.signals.length,
          generatedAt: new Date().toISOString()
        };
        this.source = 'api';
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    this.signalr.disconnect().catch(() => undefined);
  }

  refresh(): void {
    this.isLoading = true;

    forkJoin({
      signals: this.api.getRecommendations(),
      report: this.api.getReportSummary()
    })
      .pipe(
        catchError(() => of({ signals: fallbackSignals, report: fallbackReport }))
      )
      .subscribe(({ signals, report }) => {
        this.signals = signals;
        this.report = report;
        this.source = signals === fallbackSignals ? 'fallback' : 'api';
        this.isLoading = false;
      });
  }
}

const fallbackSignals: Recommendation[] = [
  {
    symbol: 'AAPL',
    market: 'NASDAQ',
    recommendation: 'Buy',
    confidence: 71,
    entryPrice: 196.2,
    stopLoss: 190.8,
    targetPrice: 209.1,
    riskRewardRatio: 2.35,
    rationale: 'Seeded dashboard row while API is offline.',
    generatedAt: new Date().toISOString()
  },
  {
    symbol: 'TSLA',
    market: 'NASDAQ',
    recommendation: 'Watch',
    confidence: 58,
    entryPrice: 175.3,
    stopLoss: 183.1,
    targetPrice: 158.2,
    riskRewardRatio: 2.18,
    rationale: 'Momentum is weak; risk manager keeps this under review.',
    generatedAt: new Date().toISOString()
  },
  {
    symbol: 'BTC-USD',
    market: 'Crypto',
    recommendation: 'Buy',
    confidence: 76,
    entryPrice: 71120,
    stopLoss: 68110,
    targetPrice: 78340,
    riskRewardRatio: 2.4,
    rationale: 'Positive momentum with macro support.',
    generatedAt: new Date().toISOString()
  }
];

const fallbackReport: ReportSummary = {
  pnl: 742.35,
  winRate: 58.8,
  trades: 34,
  equity: 10742.35,
  maxDrawdown: 4.7,
  activeSignals: fallbackSignals.length,
  generatedAt: new Date().toISOString()
};
