import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { AlertifyService } from "../_services/alertify.service";
import { DocDataService } from "../_services/docData.service";
import { DocInfo } from "../models/docinfo";
import { PaginationResult, Pagination } from "../models/Pagination";
import { ActivatedRoute } from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: "app-report-page",
  templateUrl: "./report-page.component.html",
  styleUrls: ["./report-page.component.css"],
})
export class ReportPageComponent implements OnInit {
  pagination: Pagination;
  search = "";
  filter = "";
  searchFilter = "";

  data2 = new Array();

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private spinner: NgxSpinnerService,
    private docService: DocDataService,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit() {
    this.spinner.show();
    setTimeout(() => {
      this.spinner.hide();
    }, 200);
    this.getReportData();
  }

  getReportData() {
    this.activatedRoute.data.subscribe((response) => {
      this.pagination = response.docs.pagination;
      console.log("Results=", response.docs.result);
      // debugger;
      this.addLastActionDate(response.docs.result);
    });
  }

  //add date by status
  addLastActionDate(result) {
    this.data2 = new Array();
    result.map((doc) => {
      switch (doc.status) {
        case "Viewed":
          doc.lastActionDate = doc.dateViewed;
          break;
        case "Agreed":
          doc.lastActionDate = doc.dateAgreed;
          break;
        case "Resent":
          doc.lastActionDate = doc.dateResent;
          break;
        default:
          doc.lastActionDate = doc.dateSent;
      }
      this.data2.push(doc);
    });
  }

  pageChanged(e: any): void {
    this.pagination.currentPage = e.page;
    this.loadNewPage();
  }

  loadNewPage() {
    console.log("hit in loadNewpage report-page");
    this.docService
      .getReportInfo(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.searchFilter
      )
      .subscribe(
        (response: PaginationResult<DocInfo[]>) => {
          this.addLastActionDate(response.result);
          this.pagination = response.pagination;
        },
        (error) => {
          console.log("Error during loadNewPage() getReportData GET op", error);
        }
      );
  }

  // This can be: Sent, south+Sent, or south
  newSearch(filter: string) {
    this.spinner.show();
    if (filter === "") {
      this.searchFilter = "";
      this.search = this.filter = "";
    } else if (!this.searchFilter.includes(filter)) {
      // let parts = filter.split('+');
      if (
        filter.includes("Sent") ||
        filter.includes("Resent") ||
        filter.includes("Viewed") ||
        filter.includes("Agreed")
      ) {
        this.filter = filter;
      } else {
        // is it not a filter term then it is a search
        this.search = filter;
      }
      this.searchFilter = this.search + "+" + this.filter;
    } //  this.addFilter(filter);
    console.log("Filter in", filter, "Received new search", this.searchFilter);
    this.docService
      .getReportInfo(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.searchFilter
      )
      .subscribe(
        (response: PaginationResult<DocInfo[]>) => {
          // debugger;
          console.log(response.result);
          // this.data2 = response.result;
          this.addLastActionDate(response.result);
          // this.data2.sort( (a, b) => { console.log(a.lastActionDate);  return a.lastActionDate - b.lastActionDate ; } );
          this.pagination = response.pagination;
        },
        (error) => {
          console.log("Error during newSearch() getReportInfo GET op", error);
        }
      );
    setTimeout(() => {
      this.spinner.hide();
    }, 300);
  }

  // newFilter(filter: string) {
  //   this.spinner.show();
  //   this.searchFilter = filter;
  //   console.log('Received new filter action', filter);
  //   this.docService.getReportInfo(this.pagination.currentPage, this.pagination.itemsPerPage, filter).subscribe(
  //     (response: PaginationResult<DocInfo[]>) => {
  //       // debugger;
  //       // console.log(response);
  //       // this.data2 = response.result;
  //       this.addLastActionDate(response.result);
  //       this.pagination = response.pagination;
  //     },
  //     error => {
  //       console.log('Error during loadNewPage() getReportData GET op', error);
  //     }
  //   );
  //   setTimeout(() => {
  //     this.spinner.hide();
  //   }, 1500);
  // }

  resendLink(id: number) {
    console.log("Resending link for #", id);
    const msg = "Please confim that you want to resend the link";
    this.alertify.confirm(msg, () => {
      console.log("Calling post to resend link #", id);
      this.spinner.show();
      setTimeout(() => {
        this.spinner.hide();
      }, 500);
      this.docService.resendLink(id).subscribe(
        (resp) => {
          this.alertify.success("Resent email link");
          this.newSearch(this.searchFilter);
        },
        (error) => {
          this.alertify.error("Error during resend link: " + error);
        }
      );
    });
  }
}
