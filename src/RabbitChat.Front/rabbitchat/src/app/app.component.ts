import { Component } from '@angular/core';
import { MessageChat } from './dto/messageChat';
import { SignalRService } from './services/sginal-r.service';
import { GlobalStore } from './shared/store/global-store';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {

  messages: MessageChat[];
  username: string;

  constructor(
    private signalRService: SignalRService,
    private globalStore: GlobalStore
  ) {
    this.username = "";
    this.messages = this.globalStore.getMessages();

    this.watchChangeUsername();
  }

  private watchChangeUsername() {
    this.globalStore.username$.subscribe(username => {
      this.username = username

      if(this.username && this.username != '') {
        this.signalRService.startConnection();
        this.watchEventNewMessage();
      }
    });
  }
  
  private watchEventNewMessage() {
    this.signalRService.receiveNewMessessage((dto: string) => {

      let message = new MessageChat();
      message.text = dto;

      this.messages.push(message);
    });
  }  
}
