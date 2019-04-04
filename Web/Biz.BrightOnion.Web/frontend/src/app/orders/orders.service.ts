import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/do";
import "rxjs/add/operator/map";
import { MakeOrder, Order, OrderItem, CancelOrder, ApproveOrder } from "./order.model";
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

  public removeOrder(cancelOrder: CancelOrder): Observable<Order> {
    let body = JSON.stringify(cancelOrder);
    return this.http.post<Order>(
      `${environment.orderServiceUrl}/cancel`, body, { observe: 'response' }
    ).map(response => { console.log('body', response.body); return response.body; });
  }

  public approveOrders(approveOrder: ApproveOrder): Observable<boolean> {
    let body = JSON.stringify(approveOrder);
    return this.http.post(
      `${environment.orderServiceUrl}/approve`, body, { observe: 'response' }
    ).map(response => response.status == 204);
  }
}
