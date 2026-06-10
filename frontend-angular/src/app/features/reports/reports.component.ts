import { Component } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-reports',
  template: `
    <section class="panel">
      <p>Performance</p>
      <h1>Reports</h1>
      <span>Grafik PnL dan winrate siap dihubungkan ke reporting API.</span>
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
export class ReportsComponent {
}
