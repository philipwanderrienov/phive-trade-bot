import { Component } from '@angular/core';

type SignalRow = {
  symbol: string;
  market: string;
  recommendation: 'Buy' | 'Hold' | 'Watch';
  confidence: number;
  status: 'Live' | 'Review';
};

@Component({
  standalone: true,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  readonly signals: SignalRow[] = [
    { symbol: 'AAPL', market: 'NASDAQ', recommendation: 'Hold', confidence: 62, status: 'Live' },
    { symbol: 'TSLA', market: 'NASDAQ', recommendation: 'Watch', confidence: 54, status: 'Review' },
    { symbol: 'BTC-USD', market: 'Crypto', recommendation: 'Buy', confidence: 71, status: 'Live' }
  ];
}
