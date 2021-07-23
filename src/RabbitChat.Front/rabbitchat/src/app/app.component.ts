import { Component } from '@angular/core';
import { MessageChat } from './dto/messageChat';
import { SignalRService } from './services/sginal-r.service';
import { GlobalStore } from './shared/store/global-store';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

messages: MessageChat[];

constructor(
  private signalRService: SignalRService,
  private globalStore: GlobalStore
) {
  this.signalRService.startConnection();

  this.signalRService.receiveNewMessessage((dto: string) => {

    let message = new MessageChat();
    message.text = dto;

    this.messages.push(message);
  });

  this.messages = this.globalStore.getMessages();
}
  
}
