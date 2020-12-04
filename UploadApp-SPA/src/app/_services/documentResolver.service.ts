import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { DocInfo } from '../models/docinfo';
import { Observable } from 'rxjs';
import { DocDataService } from './docData.service';
import { PaginationResult } from '../models/Pagination';

@Injectable({
  providedIn: 'root'
})
export class DocumentResolverService implements Resolve<PaginationResult<DocInfo[]>> {

constructor(private docService: DocDataService ) { }

resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):  Observable<PaginationResult<DocInfo[]>> {
  // const docInfo: DocInfo = {
  //   id: 101,
  //   firstname: 'Billy',
  //   lastname: 'Blaze',
  //   emailaddress: 'email',
  //   company: 'test hospital',
  //   salesforceid: '101BB',
  //   documentfullname: 'eula_2020.doc',
  //   description: 'test desc',
  //   dateSent: new Date('8/21/1965')
  // };
  // return docInfo;
  let demoCustomerEmail = "jmikalauskas@indxlogic.com"
  const page = 1;
  const pageSize = 10;
  var dict2 = {
    EmailAddress : demoCustomerEmail
   }
  console.log('in resolver resolve...');
  return this.docService.getDocumentInfo(page, pageSize, dict2,"");
}

}
