import { Component, OnInit } from '@angular/core';
import { SentEmailService } from '../_services/sentEmail.service';
import { DocInfo } from '../models/docinfo';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-success-page',
  templateUrl: './success-page.component.html',
  styleUrls: ['./success-page.component.css']
})
export class SuccessPageComponent implements OnInit {
  docInfo: DocInfo;
  emaillink: string;
  emaillinkid: string;
  statelink: string;
  baseUrl = 'localhost:4200/';

  constructor(private data: SentEmailService, private router: Router) {
    this.data.currentDocInfo.subscribe(di => this.docInfo = di);
    this.emaillink = this.baseUrl + 'sign-doc/' + this.docInfo.emaillinkid;
    this.emaillinkid = this.docInfo.emaillinkid;

    // debugger;
    const nav = this.router.getCurrentNavigation();
    // const state = nav.extras.state as {
    //   emaillink: string
    // };
    // this.statelink = state.emaillink;
   }

  ngOnInit() {
    // if (this.emaillink.length === 0) {
    //   this.emaillink = this.baseUrl + 'sign-doc/' + this.statelink;
    //   this.emaillinkid = this.statelink;
    //   console.log('emaillink=' + this.emaillink, ' statelink=', this.statelink);
    // }

  }

}

