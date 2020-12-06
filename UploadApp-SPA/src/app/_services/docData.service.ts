import { Injectable, PACKAGE_ROOT_URL } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { DocInfo } from "../models/docinfo";
import { environment } from "src/environments/environment";
import { PaginationResult } from "../models/Pagination";
import { map } from "rxjs/operators";

@Injectable({
  providedIn: "root",
})
export class DocDataService {
  // s/b localhost:5000/s
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDocumentInfo(
    page?,
    itemsPerPage?,
    filter?,
    keyword?
  ): Observable<PaginationResult<DocInfo[]>> {
    const paginatedResults: PaginationResult<DocInfo[]> = new PaginationResult<DocInfo[]>();
    var params = new HttpParams();
    const validateFilter = (target) => {
      if (target == null) {
        return false;
      }
      return Object.keys(target).length !== 0;
    };
    var copiedFilter = validateFilter(filter) ? Object.assign({}, filter) : null;///need to use copy of filter here otherwise the value will have dulipated+nested filter within filter

    if (page != null && itemsPerPage != null) {
      params = params.append("pageNumber", page);
      params = params.append("pageSize", itemsPerPage);
    }
    if(Boolean(keyword)){ 
      params = params.append("Keyword", keyword);
      }

    if (copiedFilter != null) {
      let arr = [];

      for (let key in copiedFilter) {
          params = params.append(key, copiedFilter[key]);
          arr.push(key);
      }
      let updatedFilter = { "Keys" : arr };
      Object.assign(copiedFilter, updatedFilter);
      params = params.append("Keys", copiedFilter["Keys"]);
    }

    let url = this.baseUrl + "api/docinfo";

    console.log("new report info url=" + url, params);
    return this.http
      .get<DocInfo[]>(url, { observe: "response", params })
      .pipe(
        map((response) => {
          console.log(response);
          paginatedResults.result = response.body;
          if (response.headers.get("Pagination") != null) {
            paginatedResults.pagination = JSON.parse(
              response.headers.get("Pagination")
            );
          }
          console.log(paginatedResults);
          return paginatedResults;
        })
      );
  }
  getReportInfo(
    page?,
    itemsPerPage?,
    filter?
  ): Observable<PaginationResult<DocInfo[]>> {
    console.log("api/licensing get report info");

    const paginatedResults: PaginationResult<DocInfo[]> = new PaginationResult<
      DocInfo[]
    >();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append("pageNumber", page);
      params = params.append("pageSize", itemsPerPage);
    }

    let url = this.baseUrl + "api/licensing/report";
    if (filter != null) {
      url += "/" + filter;
    }

    console.log("new report info url=" + url, params);
    return this.http
      .get<DocInfo[]>(url, { observe: "response", params })
      .pipe(
        map((response) => {
          console.log(response);
          paginatedResults.result = response.body;
          if (response.headers.get("Pagination") != null) {
            paginatedResults.pagination = JSON.parse(
              response.headers.get("Pagination")
            );
          }
          return paginatedResults;
        })
      );
  }

  resendLink(id: number) {
    return this.http.post(this.baseUrl + "api/licensing/resendlink/" + id, {});
  }

  updateViewDate(id: number) {
    return this.http.put(
      this.baseUrl + "api/licensing/" + "view-doc/" + id,
      {}
    );
  }

  updateAgreeDate(id: number) {
    return this.http.put(
      this.baseUrl + "api/licensing/" + "confirm-doc/" + id,
      {}
    );
  }
}
