import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  @Output() sendSearch = new EventEmitter();
  search: string = '';
  sent = false;
  resent = false;
  viewed = false;
  agreed = false;
  buttonValue = '';

  constructor() { }

  ngOnInit() {
  }

  searchReport() {
    console.log('Searching for ', this.search);
    this.sendSearch.emit(this.search);
  }

  onCheckboxChange(value: string, ischecked: boolean) {
    console.log('radio button', value, ' is checked=', ischecked);
    // this.sendFilter.emit(value);
    this.buttonValue = value;
    this.sendSearch.emit(value);
  }

  // clearSearch() {
  //   console.log('Cleared search w buttonvalue=', this.buttonValue);
  //   this.search = '';
  //   this.sendSearch.emit(this.buttonValue);
  // }

  clearFilters() {
    this.search = '';
    this.sent = false;
    this.resent = false;
    this.viewed = false;
    this.agreed = false;
    this.sendSearch.emit('');
  }

  anythingOn() {
    // console.log('Checking', this.search.length);
    if (this.search.length > 0) {
      return true;
    }
    return this.sent || this.resent || this.viewed || this.agreed;
  }

}
