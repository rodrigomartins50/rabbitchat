import { Component, Input, OnInit } from '@angular/core';
import { MessageChat } from 'src/app/dto/messageChat';

@Component({
  selector: 'message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {

  @Input()
  messageChat: MessageChat;

  constructor() {
    this.messageChat = new MessageChat();
  }

  ngOnInit(): void { }

}
