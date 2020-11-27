import { Injectable, PACKAGE_ROOT_URL } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DocInfo } from '../models/docinfo';
import { environment } from 'src/environments/environment';
import { PaginationResult } from '../models/Pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DocDataService {
  // s/b localhost:5000/
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getDocumentInfo(emaillink): Observable<DocInfo> {
  const url = this.baseUrl + 'api/licensing/' + emaillink;
  console.log('in docdataservice...');
  return this.http.get<DocInfo>(url);
}

getReportInfo(page?, itemsPerPage?, filter?): Observable<PaginationResult<DocInfo[]>> {
  const paginatedResults: PaginationResult<DocInfo[]> = new PaginationResult<DocInfo[]>();
  let params = new HttpParams();

  if (page != null && itemsPerPage != null) {
     params = params.append('pageNumber', page);
     params = params.append('pageSize', itemsPerPage);
  }

  let url = this.baseUrl + 'api/licensing/report';
  if (filter != null) {
    url += '/' + filter;
  }

  console.log('new report info url=' + url);
  return this.http.get<DocInfo[]>(url, { observe: 'response', params })
    .pipe(
      map(response => {
        paginatedResults.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResults.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResults;
      })
    );
}

resendLink(id: number) {
  return this.http.post(this.baseUrl + 'api/licensing/resendlink/' + id, {});
}

updateViewDate(id: number) {
  return this.http.put(this.baseUrl + 'api/licensing/' + 'view-doc/' + id, {});
}

updateAgreeDate(id: number) {
  return this.http.put(this.baseUrl  + 'api/licensing/' + 'confirm-doc/' + id, {});
}

}
