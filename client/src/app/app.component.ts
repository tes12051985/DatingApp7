import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { ɵafterNextNavigation } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Dating App';
  users: any;
  constructor(private http:HttpClient){
    this.http.get("https://localhost:5001/Users").subscribe(
      {
        next : response => this.users = response,
        error: error=> console.error(error),
        complete: ()=>console.log("request has completed!")
      }
    )
  }
}
