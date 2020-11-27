import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from '../_services/alertify.service';
import { DocInfo } from '../models/docinfo';
import { DocDataService } from '../_services/docData.service';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-destination',
  templateUrl: './destination.component.html',
  styleUrls: ['./destination.component.css']
})
export class DestinationComponent implements OnInit {
  baseUrl = environment.apiUrl;
  pdfSrc = 'https://vadimdez.github.io/ng2-pdf-viewer/assets/pdf-test.pdf';
  isCollapsed = false;
  emaildoc: DocInfo;
  emailLink = '';
  firstname: string;
  lastname = '';
  documentName: string;
  viewed = false;
  received = false;

  constructor(
    private http: HttpClient,
    private docService: DocDataService,
    private spinner: NgxSpinnerService,
    private router: Router,
    private route: ActivatedRoute,
    private notify: AlertifyService
  ) {}

  ngOnInit() {
    this.emailLink = this.route.snapshot.params.emaillink;
    console.log('emaillink=' + this.emailLink.trim());
    this.route.data.subscribe(response => {
      // debugger;
      this.emaildoc = response.doc;
      console.log('resolver subscribe response=', response.doc);
    });
    // Without this, you get some strange console errors sometimes...arghh!! 
    // cannot read property of 'canhaz' of undefined
    this.documentName = this.emaildoc['documentFullName'];
    console.log('Doc id=', this.emaildoc.id, ' with doc ', this.documentName);
    this.received = false;
  }

  // getDocumentInfo(emaillink: string) {
  //   const url = this.baseUrl + 'api/licensing/' + emaillink;
  //   console.log(url);
  //   this.docService.getDocumentInfo(emaillink).subscribe(
  //     (response: DocInfo) => {
  //       // debugger;
  //       console.log('Get data call successful');
  //       console.log(response);
  //       this.emaildoc = response;
  //       // this.firstname = response["firstName"];
  //       // this.lastname = response["lastName"];
  //     },
  //     error => {
  //       console.log('Error during getDocumentInfo GET op', error);
  //     }
  //   );
  // }

  view() {
    this.docService.updateViewDate(this.emaildoc.id)
      .subscribe(
        () => {
          this.notify.success('Date Viewed updated successfully');
          this.viewed = true;
        },
        error => {
          this.notify.error('Error updating date viewed.' + error);
        }
      );
  }

  confirm() {
    if (!this.viewed) { return; }
    this.docService.updateAgreeDate(this.emaildoc.id)
      .subscribe(
        () => {
          this.notify.success('Date Agreed updated successfully');
          this.router.navigate(['/confirmation-page']);
        },
        error => {
          this.notify.error('Error updating date agreed.' + error);
        }
      );

  }

  onCheckboxChange(isChecked: boolean) {
    this.received = isChecked;
  }

}
