import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GlobalStore } from './shared/store/global-store';
import { SignalRService } from './services/sginal-r.service';
import { PrimeNGModule } from './shared/primeng/primeng.module';
import { HeaderComponent } from './components/header/header.component';
import { ContentComponent } from './components/content/content.component';
import { UsersComponent } from './components/users/users.component';
import { FormsModule } from '@angular/forms';
import { MessageService } from './services/message-service';
import { HttpClientModule } from '@angular/common/http';
import { SendMessageComponent } from './components/send-message/send-message.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    ContentComponent,
    UsersComponent,
    SendMessageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    PrimeNGModule,
    HttpClientModule
  ],
  providers: [
    GlobalStore,
    SignalRService,
    MessageService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
