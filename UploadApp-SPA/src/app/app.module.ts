import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Routes, RouterModule } from '@angular/router';

import { FileUploadModule } from 'ng2-file-upload';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { Ng2SmartTableModule } from 'ng2-smart-table';

import { AppComponent } from './app.component';

import { UploadPageComponent } from './upload-page/upload-page.component';
import { HeaderComponent } from './header/header.component';
import { SuccessPageComponent } from './success-page/success-page.component';
import { DestinationComponent } from './destination/destination.component';
import { DocumentResolverService } from './_services/documentResolver.service';
import { DocDataService } from './_services/docData.service';
import { AlertifyService } from './_services/alertify.service';
import { SentEmailService } from './_services/sentEmail.service';
import { ReportPageComponent } from './report-page/report-page.component';
import { NavComponent } from './nav/nav.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ReportPageResolverService } from './_services/ReportPageResolver.service';
import { SearchComponent } from './search/search.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ConfirmationPageComponent } from './confirmation-page/confirmation-page.component';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
   {
      path: '',
      // redirectTo: 'upload-page',
      pathMatch: 'full',
      component : HomeComponent
   },
   {
     path: 'upload-page',
     component: UploadPageComponent
   },
   {
      path: 'success-upload',
      component: SuccessPageComponent
   },
   {
      path: 'sign-doc/:emaillink',
      component: DestinationComponent,
      resolve: { doc: DocumentResolverService }
   },
   {
      path: 'report-page/:id',
      component: ReportPageComponent,
      resolve: { docs : ReportPageResolverService }
   },
   {
      path: 'confirmation-page',
      component: ConfirmationPageComponent
   }
];

@NgModule({
   declarations: [
      AppComponent,
      UploadPageComponent,
      HeaderComponent,
      SuccessPageComponent,
      DestinationComponent,
      SuccessPageComponent,
      DestinationComponent,
      ReportPageComponent,
      NavComponent,
      SearchComponent,
      ConfirmationPageComponent,
      HomeComponent
   ],
   imports: [
      BrowserModule,
      BrowserAnimationsModule,
      FormsModule,
      HttpClientModule,
      FileUploadModule,
      ReactiveFormsModule,
      NgxSpinnerModule,
      PdfViewerModule,
      Ng2SmartTableModule,
      RouterModule.forRoot(routes),
      PaginationModule.forRoot()
   ],
   providers: [
      AlertifyService,
      DocDataService,
      DocumentResolverService,
      SentEmailService
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
