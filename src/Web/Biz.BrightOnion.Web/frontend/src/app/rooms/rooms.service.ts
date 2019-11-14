import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/do";
import "rxjs/add/operator/map";
// import { Config } from "../shared/config";
import { Room, User } from "./room.model";
import { environment } from "../../environments/environment";

@Injectable()
export class RoomService {

  constructor(private http: HttpClient) { }

  public getRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(
      environment.roomServiceUrl);
  }

  public addRoom(room: Room): Observable<boolean> {
    let body = JSON.stringify(room);

    return this.http.post(
      environment.roomServiceUrl, body, { observe: 'response' }
    ).map(response => response.status == 201);
  }

  public editRoom(room: Room): Observable<boolean> {
    let body = JSON.stringify(room);

    return this.http.put(
      environment.roomServiceUrl, body, { observe: 'response' }
    ).map(response => response.status == 201);
  }

  public removeRoom(roomId: number): Observable<boolean> {
    return this.http.delete(
      `${environment.roomServiceUrl}/${roomId}`, { observe: 'response' }
    ).map(response => response.status == 204);
  }

  public getUsers(): Observable<User[]> {
    return this.http.get<User[]>(
      environment.accountServiceUrl);
  }
}
