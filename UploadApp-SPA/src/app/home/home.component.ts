import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { UserDataService } from "../_services/userData.service";
import { User } from "../models/user";
////////TODO :  validation needed here + authentication

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"],
})
export class HomeComponent implements OnInit {
  panelOpenState: boolean = false;
  panelSignUpState: boolean = false;
  agreedToTerms: boolean = false;
  signInemail: string = "";
  email : string ="";
  pwd: string = "";
  firstName: string = "";
  lastName: string = "";
  constructor(private router: Router, private userService: UserDataService) {}

  ngOnInit() {}

  login() {
    console.log(this.signInemail)
    this.userService.getUserByEmail(this.signInemail).subscribe(
      (response) => {
        if(response.status == 200){
          let user = response.body;
          localStorage.setItem('isLoggedIn', "true");  
          localStorage.setItem('currentUser',user);
          localStorage.setItem('userId', user.id);
          this.router.navigate(["report-page", user.id]);
        }
      },
      (error) => {
        console.log("Error logging in", error);
      }
    );
  }

  toggleAuth() {
    this.panelOpenState = true;
  }
  toggleSignUp() {
    this.panelSignUpState = !this.panelSignUpState;
  }
  signUp() {
    const user: User = {
      firstname: this.firstName,
      lastname: this.lastName,
      emailaddress: this.email,
    };
    this.userService.addNewUser(user).subscribe((res) => {
      if (res != null) {
        // this.router.navigate([""]);
        console.log(res)
      }
      //TODO : show error msg if sign up fail
    });
  }
}
