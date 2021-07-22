import { Component } from '@angular/core';
import { SignalRService } from './services/sginal-r.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'rabbitchat';

constructor(private signalRService: SignalRService) {
  this.signalRService.startConnection();

  this.signalRService.receiveNewMessessage((dto: string) => {
    this.title = dto;
  })
}
  
}
