import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { BaseService } from '../core/base.service';
import { SendMessageDto } from '../dto/send-message-dto';
import { LoadMessagesDto } from '../dto/load-messages-dto';

@Injectable({
  providedIn: 'root'
})
export class MessageService extends BaseService {

  constructor(httpClient: HttpClient) { super(httpClient); }

  send(sendObject : SendMessageDto): Observable<any> {
    return this.post("/message", sendObject);
  }

  load(sendObject : LoadMessagesDto): Observable<any> {
    return this.post("/message/loadMessages", sendObject);
  }

}
