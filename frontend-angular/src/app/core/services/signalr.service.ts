import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SignalrService {
  readonly hubUrl = environment.signalrHubUrl;

  connect(): void {
  }
}
