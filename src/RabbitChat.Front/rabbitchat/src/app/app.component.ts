import { Component } from '@angular/core';
import { LoadMessagesDto } from './dto/load-messages-dto';
import { MessageChat } from './dto/messageChat';
import { MessageService } from './services/message-service';
import { SignalRService } from './services/sginal-r.service';
import { StateConnectionEnum } from './shared/enum/state-connection.enum';
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

    this.globalStore.connectionState$.subscribe(state => {
      if(state == StateConnectionEnum.conectado) {
        
        let dto = new LoadMessagesDto();
        dto.connectionId = this.globalStore.getConnectionId();
        this.messageService.load(dto).subscribe(dto => true);
      }
    });
  }

  private watchLoadMessages() {
      this.signalRService.loadMessessages((listDto: MessageChat[]) => {

        for (let index = 0; index < listDto.length; index++) {
          const element = listDto[index];
          
          this.messages.unshift(element);
        }
      });
  }
}
