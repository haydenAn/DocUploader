import { Injectable } from "@angular/core";
import {
  Resolve,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from "@angular/router";
import { DocInfo } from "../models/docinfo";
import { Observable, of } from "rxjs";
import { DocDataService } from "./docData.service";
import { PaginationResult } from "../models/Pagination";
import { catchError } from "rxjs/operators";
import { AlertifyService } from "./alertify.service";

@Injectable({
  providedIn: "root",
})
export class ReportPageResolverService
  implements Resolve<PaginationResult<DocInfo[]>> {
  constructor(
    private docService: DocDataService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(
    route: ActivatedRouteSnapshot
  ): Observable<PaginationResult<DocInfo[]>> {
    const page = 1;
    const pageSize = 10;
    return this.docService.getDocumentInfo(page, pageSize).pipe(
      catchError((error) => {
        this.alertify.error(
          "Problem retrieving document for report in resolver" + error
        );
        this.router.navigate([""]);
        return of(null);
      })
    );
  }
}
