import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { FileUploader } from "ng2-file-upload";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";

import { environment } from "src/environments/environment";
import { AlertifyService } from "../_services/alertify.service";
import { SentEmailService } from "../_services/sentEmail.service";
import { DocInfo } from "../models/docinfo";
import { Observable } from "rxjs";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: "app-upload-page",
  templateUrl: "./upload-page.component.html",
  styleUrls: ["./upload-page.component.css"],
})
export class UploadPageComponent implements OnInit {
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  droppedFiles: string[];
  baseUrl = environment.apiUrl + "api/docinfo";
  dntLinkUrl = "";
  tags = [];
  owners = [];
  userEmails = new FormGroup({
    firstname: new FormControl(""),
    lastname: new FormControl(""),
    company: new FormControl(""),
    title: new FormControl(""),
    email: new FormControl("", [
      Validators.required,
      Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+.[a-z]{2,4}$"),
    ]),
  });
  @ViewChild("firstName", { static: true }) firstNameField: ElementRef;

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private notify: AlertifyService,
    private spinner: NgxSpinnerService,
    private emailService: SentEmailService
  ) {}

  ngOnInit() {
    // debugger;
    this.spinner.show();
    this.initializeUploader();
    this.dntLinkUrl = this.route.snapshot.params.temporaryUrl;
    this.userEmails.get("firstname").setValue("John");
    this.userEmails.get("lastname").setValue("Mikalauskas");
    this.userEmails.get("company").setValue("Southern NH Hospital");
    this.userEmails.get("title").setValue("CIO");
    this.userEmails.get("email").setValue("jmikalauskas@indxlogic.com");
    setTimeout(() => {
      this.spinner.hide();
    }, 300);
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  public onFileDrop(e: any): void {
    console.log("Dropped=" + e);
    this.droppedFiles = e;
  }
  public onFileChanged(e: any): void {
    console.log(e.target.files);
    console.log(this.uploader.queue)
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + "docs", // 'users/' + this.authService.decodedToken.nameid + '/photos',
      // authToken: 'Bearer' + localStorage.getItem('token'),
      isHTML5: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
      headers: [
        { name: "Access-Control-Allow-Origin", value: "http://localhost:4200" },
      ],
    });

    this.uploader.onBeforeUploadItem = (item) => {
      item.withCredentials = false;
    };
  }

  storeDocInfo() {
    this.owners.push(localStorage.getItem("userId"));
    const docInfo: DocInfo = {
      documentfullname: "",
      firstname: this.userEmails.get("firstname").value,
      lastname: this.userEmails.get("lastname").value,
      emailaddress: this.userEmails.get("email").value,
      title: this.userEmails.get("title").value,
      company: this.userEmails.get("company").value,
      salesforceid: 101,
      dateSent: new Date(),
      dateViewed: new Date(),
      dateAgreed: new Date(),
      dateResent: new Date(),
      description: "",
      emaillinkid: "",
      tags: this.tags,
      filepath: "",
      owners: this.owners,
    };
    const files = [];
    let docs = "";
    console.log(this.uploader);
    console.log("uploader=");
    this.uploader.queue.forEach((f) => {
      files.push(f.file.name);
      docs = docs + f.file.name + ";";
      docInfo.filepath = f.file.name;
    });
    docInfo.documentfullname =
      "C:\\users\\default.DESKTOP-GRQ62EF\\Pictures\\" + docs;
    docInfo.filepath =
      "C:\\Users\\hayde_5jbn1tp\\Desktop\\DocInfoService (1).cs";
    debugger;

    this.spinner.show();
    console.log(docInfo);
    this.sendPostRequest(this.baseUrl, docInfo).subscribe(
      (response) => {
        // debugger;
        docInfo.emaillinkid = response.emaillink;
        this.emailService.changeDocInfo(docInfo);
        this.notify.success("Saved and email sent");
        console.log("Post call successful w response=" + response.emaillink);
        this.clearFields();
      },
      (error) => {
        console.log("Error during sendLink POST op", error);
      }
    );
    setTimeout(() => {
      this.spinner.hide();
      this.navigate("success-upload");
    }, 1000);
  }

  navigate(url) {
    this.router.navigate(["/" + url]);
  }
  getTags(tags) {
    this.tags = tags;
  }

  clearFields() {
    this.userEmails.get("firstname").setValue("");
    this.userEmails.get("lastname").setValue("");
    this.userEmails.get("company").setValue("");
    this.userEmails.get("title").setValue("");
    this.userEmails.get("email").setValue("");
    this.uploader.clearQueue();
    // debugger;
    let qlen = this.uploader.queue.length;
    try {
      for (let i = 0; i < qlen; i++) {
        this.uploader.queue[i].remove();
      }
    } catch {}
    this.firstNameField.nativeElement.focus();
  }

  sendPostRequest(url: string, data: any): Observable<any> {
    return this.http.post<any>(url, data);
  }
}
