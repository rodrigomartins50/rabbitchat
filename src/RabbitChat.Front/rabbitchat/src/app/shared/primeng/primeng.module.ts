import { NgModule } from '@angular/core';

import {ButtonModule} from 'primeng/button';
import {EditorModule} from 'primeng/editor';
import {InputTextModule} from 'primeng/inputtext';

@NgModule({
  imports: [
    ButtonModule,
    EditorModule,
    InputTextModule
  ],
  exports: [
    ButtonModule,
    EditorModule,
    InputTextModule
  ]
})
export class PrimeNGModule { }
