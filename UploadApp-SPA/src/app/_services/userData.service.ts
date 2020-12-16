import { Injectable, PACKAGE_ROOT_URL } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { User } from "../models/user";
import { environment } from "src/environments/environment";
import { map } from "rxjs/operators";

@Injectable({
  providedIn: "root",
})
export class UserDataService {
  // s/b localhost:5000/s
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUserByEmail(email) : any {
    var params = new HttpParams();
    params = params.append("email", email);
    let url = this.baseUrl + "api/user/" + email;
    console.log("new report info url=" + url, email);
    return this.http
      .get<User>(url, { observe: "response" , params})
      .pipe(
        map((response) => {
          console.log(response);
          localStorage.setItem('isLoggedIn', "true");  
        //   localStorage.setItem('token', response); 
          return response
        })
      );
  }
  
  addNewUser(user) : Observable<any> {
    let url = this.baseUrl + "api/user";
    return this.http.post<any>(url,user);
  }

  //TODO : ERROR HANDLING FOR ALL THE METHODS
}