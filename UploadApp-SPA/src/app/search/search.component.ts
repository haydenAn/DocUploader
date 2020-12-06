import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  @Output() sendSearch = new EventEmitter<{keyword: string, filter: any}>();
  // @Output addModel = new EventEmitter<{make: string, name: string}>();


  keyword: string = '';
  sent = false;
  resent = false;
  viewed = false;
  agreed = false;
  filterDict = {};

  constructor() { }

  ngOnInit() {
  }

  searchReport() {
    console.log('Searching for keyword ', this.keyword);

    let copy = Object.assign({}, this.filterDict);
    this.sendSearch.emit({keyword:this.keyword, filter :copy});
  }

  onCheckboxChange(value: string, ischecked: boolean) {
    console.log('radio button', value, ' is checked=', ischecked);

    this.filterDict["Status"] = value.toString();
    let copy = Object.assign({}, this.filterDict);
    this.sendSearch.emit({keyword:this.keyword, filter:copy});
  }

  clearFilters() {
    this.keyword = '';
    this.sent = false;
    this.resent = false;
    this.viewed = false;
    this.agreed = false;

    for (var key in this.filterDict) {
      delete this.filterDict[key];
    }
    Object.assign(this.filterDict, {});
    //don't change this line
    this.sendSearch.emit({keyword:this.keyword, filter:null});
  }

  anythingOn() {
    if (this.keyword.length > 0) {
      return true;
    }
    return this.sent || this.resent || this.viewed || this.agreed;
  }

}
