import { Injectable } from "@angular/core"
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Observable } from "rxjs/Rx";
import "rxjs/add/operator/do";
import "rxjs/add/operator/map";

import { Config } from "app/shared/config";
import { User } from "./user.model";
import { ChangePassword } from "./change-password.model";
import { AuthData } from './auth-data.model';
import { UserProfile } from "./user-profile.model";

@Injectable()
export class AuthenticationService {
  constructor(private http: HttpClient) { }

  public login(user: User): Observable<AuthData> {
    return this.http.post(
      /*Config.apiUrl + */"https://localhost:7100/identity-api/account/login",
      JSON.stringify({
        email: user.email,
        password: user.password
        /*grant_type: "password"*/
      }),
      { observe: 'response' }
    )
      .map(response => {
        let data = response.body;
        return { token: data['token'], email: user.email };
      })
      .do(authData => {
        localStorage.setItem('token', authData.token);
        localStorage.setItem('username', authData.email);
      })
      .catch(this.handleErrors);
  }

  logout() {
    localStorage.removeItem('username');
    localStorage.removeItem('token');
  }

  public getLoggedUser(): string {
    return localStorage.getItem('username');
  }

  public changePassword(changePassword: ChangePassword) {
    return this.http.post(
      Config.apiUrl + "user/changepassword",
      JSON.stringify({
        email: changePassword.email,
        currentPasswd: changePassword.currentPassword,
        Passwd: changePassword.newPassword,
        Passwd2: changePassword.newPassword2,
      }),
      { observe: 'response' }
    )
      .catch(this.handleErrors);
  }
    
  public register(user: User) {
    return this.http.post<void>(
      /*Config.apiUrl + */"https://localhost:7100/identity-api/account/register",
      JSON.stringify({
        email: user.email,
        password: user.password,
        password2: user.password
      }),
      { observe: 'response' }
    )
      /*.catch(this.handleErrors)*/;
  }

  public getUserProfile(email: string): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${Config.apiUrl}user/${email}`);
  }

  public editUserProfile(userProfile: UserProfile): Observable<boolean> {
    let body = JSON.stringify(userProfile);

    return this.http.put(
      `${Config.apiUrl}user/profile/${userProfile.email}`, body, { observe: 'response' }
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
