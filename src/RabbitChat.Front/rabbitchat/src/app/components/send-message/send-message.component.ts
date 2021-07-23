import { Component, OnInit } from '@angular/core';

import { SendMessageDto } from 'src/app/dto/send-message-dto';
import { MessageService } from 'src/app/services/message-service';

@Component({
  selector: 'send-message',
  templateUrl: './send-message.component.html',
  styleUrls: ['./send-message.component.css']
})
export class SendMessageComponent implements OnInit {

  text: string;

  constructor(private messageService: MessageService) { 
    this.text = '';
  }

  ngOnInit(): void { }

  send() {
    let sendMessageDto = new SendMessageDto();
    sendMessageDto.text = this.text;

    this.messageService.send(sendMessageDto).subscribe(dto => {
      this.text = '';
    })
  }

}
