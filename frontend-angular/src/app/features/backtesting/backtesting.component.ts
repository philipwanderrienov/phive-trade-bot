import { Component } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-backtesting',
  template: `
    <section class="panel">
      <p>Strategy lab</p>
      <h1>Backtesting</h1>
      <span>Backtest runner siap dihubungkan ke API .NET.</span>
    </section>
  `,
  styles: [`
    .panel {
      border: 1px solid rgba(255, 255, 255, 0.1);
      border-radius: 8px;
      padding: 24px;
      background: rgba(13, 18, 28, 0.76);
    }

    p {
      margin: 0 0 6px;
      color: #2dd4bf;
      font-size: 13px;
      font-weight: 700;
      text-transform: uppercase;
    }

    h1 {
      margin: 0 0 12px;
    }

    span {
      color: #b9c5d6;
    }
  `]
})
export class BacktestingComponent {
}
