import { NgModule } from '@angular/core';

import {ButtonModule} from 'primeng/button';
import {EditorModule} from 'primeng/editor';

@NgModule({
  imports: [
    ButtonModule,
    EditorModule
  ],
  exports: [
    ButtonModule,
    EditorModule
  ]
})
export class PrimeNGModule { }
