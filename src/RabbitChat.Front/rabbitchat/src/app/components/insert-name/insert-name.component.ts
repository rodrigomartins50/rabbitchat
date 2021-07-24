import { Component, OnInit } from '@angular/core';
import { GlobalStore } from 'src/app/shared/store/global-store';

@Component({
  selector: 'insert-name',
  templateUrl: './insert-name.component.html',
  styleUrls: ['./insert-name.component.css']
})
export class InsertNameComponent implements OnInit {

  username: string;

  constructor(private globalStore:GlobalStore) { 
    this.username = "";
  }

  ngOnInit(): void { }

  insertUsername() {
    this.globalStore.setUsername(this.username);
  }

}
