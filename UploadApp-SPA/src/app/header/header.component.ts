import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
@Output() sendDocInfo = new EventEmitter();
@Output() initializeUploader = new EventEmitter();
  constructor() { }

  ngOnInit() {
  }

  saveAndConfirm() {
    this.sendDocInfo.emit('confirm action from user requested!');
  }

  reset() {
    this.initializeUploader.emit('initializing action from user requested!');
  }
}
