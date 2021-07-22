import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GlobalStore } from './shared/store/global-store';
import { SignalRService } from './services/sginal-r.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [
    GlobalStore,
    SignalRService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
