import { Component, OnInit, Output, EventEmitter } from "@angular/core";
import {
  trigger,
  state,
  style,
  animate,
  transition,
} from "@angular/animations";

@Component({
  selector: "app-search",
  templateUrl: "./search.component.html",
  styleUrls: ["./search.component.css"],
  animations: [
    trigger("inOutAnimation", [
      transition(":enter", [
        style({ height: 0, opacity: 0 }),
        animate("0.5s ease-out", style({ height: 260, opacity: 1 })),
      ]),
      transition(":leave", [
        style({ height: 260, opacity: 1 }),
        animate("0.5s ease-in", style({ height: 0, opacity: 0 })),
      ]),
    ]),
  ],
})
export class SearchComponent implements OnInit {
  @Output() sendSearch = new EventEmitter<{ keyword: string; filter: any }>();
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

    let copy = Object.assign({}, this.filterDict);
    this.sendSearch.emit({ keyword: this.keyword, filter: copy });
    this.panelOpenState = false;
  }

  onCheckboxChange(value: string, ischecked: boolean) {
    console.log("radio button", value, " is checked=", ischecked);
    this.filterDict["Status"] = value.toString();
  }

  clearFilters() {
    this.keyword = "";
    this.email= "";
    this.firstName= "";
    this.lastName= "";
    this.title= "";
    this.company= "";
    this.description= "";

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
  }
}
