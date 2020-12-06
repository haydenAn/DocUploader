import { Injectable } from '@angular/core';
import { DocInfo } from '../models/docinfo';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SentEmailService {
  private infoSource = new BehaviorSubject<DocInfo>(null);
  currentDocInfo = this.infoSource.asObservable();

constructor() { }

changeDocInfo(doc: DocInfo) {
  this.infoSource.next(doc);
  this.currentDocInfo.subscribe(di => console.log(di));
}

}
