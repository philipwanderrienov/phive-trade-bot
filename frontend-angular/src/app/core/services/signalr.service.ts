import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Recommendation } from './api.service';

@Injectable({ providedIn: 'root' })
export class SignalrService {
  readonly hubUrl = environment.signalrHubUrl;
  readonly signalCreated$ = new Subject<Recommendation>();
  private connection?: signalR.HubConnection;

  async connect(): Promise<void> {
    if (this.connection) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .withAutomaticReconnect()
      .build();

    this.connection.on('SignalCreated', (signal: Recommendation) => {
      this.signalCreated$.next(signal);
    });

    await this.connection.start();
  }

  async disconnect(): Promise<void> {
    if (!this.connection) {
      return;
    }

    await this.connection.stop();
    this.connection = undefined;
  }
}
