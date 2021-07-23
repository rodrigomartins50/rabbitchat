import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { MessageChat } from 'src/app/dto/messageChat';
import { GlobalStore } from 'src/app/shared/store/global-store';

@Component({
  selector: 'content',
  templateUrl: './content.component.html',
  styleUrls: ['./content.component.css']
})
export class ContentComponent implements OnInit {

  messages: MessageChat[]

  constructor(private globalStore: GlobalStore) { 
    this.messages = this.globalStore.getMessages();
  }

  ngOnInit(): void { }

}
