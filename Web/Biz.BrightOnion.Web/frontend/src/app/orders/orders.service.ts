import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/do";
import "rxjs/add/operator/map";
import { MakeOrder, Order, OrderItem } from "./order.model";
import { environment } from "../../environments/environment";

@Injectable()
export class OrdersService {

  constructor(private http: HttpClient) { }

  public getOrder(roomId: number): Observable<Order> {
    return this.http.get<Order>(
      `${environment.orderServiceUrl}/${roomId}`);
  }

  public makeOrder(makeOrder: MakeOrder): Observable<Order> {
    let body = JSON.stringify(makeOrder);
    return this.http.post<Order>(
      `${environment.orderServiceUrl}/make`, body, { observe: 'response' }
    ).map(response => { console.log('body', response.body); return response.body; });
  }

  public removeOrder(orderId: string):  Observable<boolean> {
    return this.http.delete(
      `${environment.orderServiceUrl}/${orderId}`, { observe: 'response' }
    ).map(response => response.status == 204);
  }

  public approveOrders(room: string): Observable<boolean> {
    return this.http.post(
      `${environment.orderServiceUrl}/${room}/approve`, null, { observe: 'response' }
    ).map(response => response.status == 201);
  }
}
