import { Injectable } from "@angular/core"
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Observable } from "rxjs/Rx";
import "rxjs/add/operator/do";
import "rxjs/add/operator/map";

import { User } from "./user.model";
import { ChangePassword } from "./change-password.model";
import { AuthData } from './auth-data.model';
import { UserProfile } from "./user-profile.model";
import { environment } from "../../../environments/environment";

@Injectable()
export class AuthenticationService {
  constructor(private http: HttpClient) { }

  public login(user: User): Observable<AuthData> {
    return this.http.post(
      `${environment.accountServiceUrl}/login`,
      JSON.stringify({
        email: user.email,
        password: user.password
        /*grant_type: "password"*/
      }),
      { observe: 'response' }
    )
      .map(response => {
        let data = response.body;
        return { userId: data['userId'], token: data['token'], email: user.email };
      })
      .do(authData => {
        localStorage.setItem('token', authData.token);
        localStorage.setItem('userId', authData.userId);
        localStorage.setItem('username', authData.email);
      }).catch(this.handleErrors);
  }

  logout() {
    localStorage.removeItem('userId');
    localStorage.removeItem('username');
    localStorage.removeItem('token');
  }

  public getLoggedUsername(): string {
    return localStorage.getItem('username');
  }

  public getLoggedUserId(): number {
    let userId: number = 0;
    let strUserId = localStorage.getItem('userId');
    if (strUserId) {
      userId = parseInt(strUserId);
    }
    return userId;
  }

  public changePassword(changePassword: ChangePassword) {
    return this.http.post(
      `${environment.accountServiceUrl}/changepassword`,
      JSON.stringify({
        email: changePassword.email,
        oldPassword: changePassword.currentPassword,
        password: changePassword.newPassword,
        password2: changePassword.newPassword2,
      }),
      { observe: 'response' }
    ).catch(this.handleErrors);
  }
    
  public register(user: User) {
    return this.http.post<void>(
      `${environment.accountServiceUrl}/register`,
      JSON.stringify({
        email: user.email,
        password: user.password,
        password2: user.password
      }),
      { observe: 'response' }
    ).catch(this.handleErrors);
  }

  public getUserProfile(email: string): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${environment.accountServiceUrl}/${email}`);
  }

  public editUserProfile(userProfile: UserProfile): Observable<boolean> {
    let body = JSON.stringify(userProfile);

    return this.http.put(
      environment.accountServiceUrl, body, { observe: 'response' }
    ).map(response => response.status == 204);
  }

  public getToken(): string {
    return localStorage.getItem('token')
  }

  private handleErrors(error: any) {
    console.log(JSON.stringify(error.json()));
    return Observable.throw(error);
  }
}
