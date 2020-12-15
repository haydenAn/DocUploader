import { Component, OnInit, Output, EventEmitter } from "@angular/core";
import {
  trigger,
  state,
  style,
  animate,
  transition,
} from "@angular/animations";

export interface ActivatedFilter {
  name: string;
  value : any;
}

@Component({
  selector: "app-search",
  templateUrl: "./search.component.html",
  styleUrls: ["./search.component.css"],
  animations: [
    trigger("inOutAnimation", [
      transition(":enter", [
        style({ height: 0, opacity: 0 }),
        animate("0.5s ease-out", style({ height: 270, opacity: 1 })),
      ]),
      transition(":leave", [
        style({ height: 270, opacity: 1 }),
        animate("0.5s ease-in", style({ height: 0, opacity: 0 })),
      ]),
    ]),
  ],
})
export class SearchComponent implements OnInit {
  @Output() sendSearch = new EventEmitter<{ keyword: string; filter: any }>();
  activatedFilters: ActivatedFilter[] = [
  ];
  selectable = true;
  removable = true;
  panelOpenState = false;

  keyword: string = "";
  email: string = "";
  firstName: string = "";
  lastName: string = "";
  title: string = "";
  company: string = "";
  description: string = "";

  filterDict = {};

  sent = false;
  resent = false;
  viewed = false;
  agreed = false;

  constructor() {}

  ngOnInit() {}

  searchReport() {
    console.log("Searching for keyword ", this.keyword, this.email);
    this.filterDict["EmailAddress"] = this.email;
    this.filterDict["FirstName"] = this.firstName;
    this.filterDict["LastName"] = this.lastName;
    this.filterDict["Title"] = this.title;
    this.filterDict["Company"] = this.company;
    this.filterDict["Description"] = this.description;

    this.sendFilterData();
    this.panelOpenState = false;
  }
  sendFilterData(){
    let copy = Object.assign({}, this.filterDict);
    this.sendSearch.emit({ keyword: this.keyword, filter: copy });

    let arr = [];
    for(let key in copy){
      if(Boolean(copy[key])){
        arr.push({name : key , value : copy[key]});
      }
    }
    this.activatedFilters = arr;
  }

  remove(filter: ActivatedFilter): void {
    console.log(filter);
    this.filterDict[filter.name] = null;
    this.sendFilterData();
    const index = this.activatedFilters.indexOf(filter);
    if (index >= 0) {
      this.activatedFilters.splice(index, 1);
    }
  }

  onCheckboxChange(value: string, ischecked: boolean) {
    console.log("radio button", value, " is checked=", ischecked);
    switch (value) {
      case "Sent":
        this.sent = ischecked;
        break;
      case "Resent":
        this.resent = ischecked;
        break;
      case "Viewd":
        this.viewed = ischecked;
        break;
      case "Agreed":
        this.agreed = ischecked;
        break;
      default:
        break;
    }
    this.filterDict["Status"] = value.toString();
  }

  clearFilters() {
    this.keyword = "";
    this.email = "";
    this.firstName = "";
    this.lastName = "";
    this.title = "";
    this.company = "";
    this.description = "";

    this.sent = false;
    this.resent = false;
    this.viewed = false;
    this.agreed = false;

    this.panelOpenState = false;

    for (var key in this.filterDict) {
      delete this.filterDict[key];
    }
    Object.assign(this.filterDict, {});
    //don't change this line
    this.sendSearch.emit({ keyword: this.keyword, filter: null });
  }

  toggleOpenState() {
    this.panelOpenState = !this.panelOpenState;
    console.log(this.filterDict);
  }
}
