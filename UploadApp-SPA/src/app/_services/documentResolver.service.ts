import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { DocInfo } from '../models/docinfo';
import { Observable } from 'rxjs';
import { DocDataService } from './docData.service';

@Injectable({
  providedIn: 'root'
})
export class DocumentResolverService implements Resolve<DocInfo> {

constructor(private docService: DocDataService ) { }

resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<DocInfo> {
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
  console.log('in resolver resolve...');
  return this.docService.getDocumentInfo(route.params.emaillink);
}

}
