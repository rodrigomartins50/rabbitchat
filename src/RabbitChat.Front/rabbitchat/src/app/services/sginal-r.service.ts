import { ThisReceiver } from '@angular/compiler';
import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { environment } from 'src/environments/environment';
import { StateConnectionEnum } from '../shared/enum/state-connection.enum';
import { GlobalStore } from '../shared/store/global-store';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private hubConnection: signalR.HubConnection

  constructor(private globalStore: GlobalStore) { 

    let urlService = environment.urlService;

    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(urlService + '/rabbitchathub')
    .withAutomaticReconnect([0, 2000, 50000, 2000])
    .build();
  }

  public startConnection(): Promise<void> {

    let conexao = this.hubConnection.start();

    conexao.then(() => {
      this.infoConnectionIdStore();
      this.globalStore.setConnectionState(StateConnectionEnum.conectado);
    })
    .catch(err => console.log('Error while setConnectionState connection: ' + err));

    this.hubConnection.onreconnecting(error => {
      this.globalStore.setConnectionState(StateConnectionEnum.reconectando);
    });

    this.hubConnection.onreconnected(error => {
      this.infoConnectionIdStore();
      this.globalStore.setConnectionState(StateConnectionEnum.conectado);
    });

    this.hubConnection.onclose(error => {
      this.globalStore.setConnectionState(StateConnectionEnum.fechado);
    });

    return conexao;
  }

  public stopConnection(): void{
    this.hubConnection.stop();
  }

  public receiveNewMessessage(methodReturn: any): void {
    this.hubConnection.on("ReceiveNewMessage", (dto) => {
        methodReturn(dto);
    });
  }

  public loadMessessages(methodReturn: any): void {
    this.hubConnection.on("LoadMessages", (dto) => {
        methodReturn(dto);
    });
  }

  private infoConnectionIdStore() {
    if(this.hubConnection.connectionId) {
      this.globalStore.setConnectionId(this.hubConnection.connectionId);
    }
  }

}