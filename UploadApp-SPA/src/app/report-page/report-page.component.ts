import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { AlertifyService } from "../_services/alertify.service";
import { DocDataService } from "../_services/docData.service";
import { DocInfo } from "../models/docinfo";
import { PaginationResult, Pagination } from "../models/Pagination";
import { ActivatedRoute } from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";
import * as fileSaver from 'file-saver';


@Component({
  selector: "app-report-page",
  templateUrl: "./report-page.component.html",
  styleUrls: ["./report-page.component.css"],
})
export class ReportPageComponent implements OnInit {
  pagination: Pagination;
  filterDict = {};
  keyword : string = "";
  data2 = new Array();
  count = 0;

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
      console.log(response.docs);
      this.pagination = response.docs.pagination;
      this.count = response.docs.result.length;
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
      .getDocumentInfo(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.filterDict
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
  newSearch( data : any) {
    this.keyword = data.keyword;

    if(data.filter==null){
      for (var key in this.filterDict) {
          delete this.filterDict[key];
      }
      Object.assign(this.filterDict, {});
    }
    else{ Object.assign(this.filterDict, data.filter); }

    //validate filterDict 
    //edit this when adding date time to searchable fields 
     for (var key in this.filterDict) {
      if(!Boolean(this.filterDict[key])){
        delete this.filterDict[key];
      }
    }
    this.getDocInfo();
  }

  getDocInfo(){
    this.docService
      .getDocumentInfo(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.filterDict,
        this.keyword
      )
      .subscribe(
        (response: PaginationResult<DocInfo[]>) => {
          // debugger;
          console.log(response.result);
          this.addLastActionDate(response.result);
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

  downloadDocFile(fileId){
    this.docService.downloadFile(fileId).subscribe(
      (response)=>{
          console.log(response);
          let blob:any = new Blob([response], {type: 'text/xml'});
          const url = window.URL.createObjectURL(blob);
          // window.open(url)
          fileSaver.saveAs(blob, 'test.txt');
        },
        (error) => {
          this.alertify.error("Error during download link: " + error);
        }
      )
  }


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
          this.newSearch(this.filterDict);
        },
        (error) => {
          this.alertify.error("Error during resend link: " + error);
        }
      );
    });
  }
}
