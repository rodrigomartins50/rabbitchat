import { Component } from '@angular/core';
import { LoadMessagesDto } from './dto/load-messages-dto';
import { MessageChat } from './dto/messageChat';
import { MessageService } from './services/message-service';
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
    private globalStore: GlobalStore,
    private messageService: MessageService
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
        this.loadInitialMessages();
        this.watchLoadMessages();
      }
    });
  }

  private watchEventNewMessage() {
    this.signalRService.receiveNewMessessage((dto: MessageChat) => {
      this.messages.push(dto);
    });
  }  

  private loadInitialMessages() {
    let dto = new LoadMessagesDto();

    this.messageService.load(dto).subscribe(dto => true);
  }

  private watchLoadMessages() {
    this.signalRService.loadMessessages((dto: MessageChat) => {
      this.messages.push(dto);
    });
  }
}
