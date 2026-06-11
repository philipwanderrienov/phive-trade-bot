import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export type Recommendation = {
  symbol: string;
  market: string;
  recommendation: string;
  confidence: number;
  entryPrice: number;
  stopLoss: number;
  targetPrice: number;
  riskRewardRatio: number;
  rationale: string;
  generatedAt: string;
};

export type ReportSummary = {
  pnl: number;
  winRate: number;
  trades: number;
  equity: number;
  maxDrawdown: number;
  activeSignals: number;
  generatedAt: string;
};

@Injectable({ providedIn: 'root' })
export class ApiService {
  readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {
  }

  getRecommendations(): Observable<Recommendation[]> {
    return this.http.get<Recommendation[]>(`${this.baseUrl}/recommendation`);
  }

  getReportSummary(): Observable<ReportSummary> {
    return this.http.get<ReportSummary>(`${this.baseUrl}/report`);
  }
}
