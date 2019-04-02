import { Component, OnInit, ViewChild, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { RoomService } from '../rooms/rooms.service';
import { Room } from '../rooms/room.model';
import { MakeOrder, Order, OrderItem, CancelOrder } from './order.model';
import { AuthenticationService } from '../shared/auth/authentication.service';
import { OrdersService } from './orders.service';
import { BaseChartDirective } from 'ng2-charts';
import { ErrorHelper } from '../shared/error-helper';
import * as signalR from "@aspnet/signalr";
import { Message, OperationType } from '../shared/message.model';
import { Config } from '../shared/config';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-root',
  providers: [
    RoomService,
    OrdersService,
    AuthenticationService
  ],
  templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit {

  public rooms: Room[] = [];
  public selectedRoom: Room;
  public quantity: number;
  public order: Order = new Order();

  @ViewChild(BaseChartDirective) chart: BaseChartDirective;

  // Pie
  public pieChartLabels: string[] = [];
  public pieChartData: number[] = [];
  public pieChartColours: any[] = [{ backgroundColor: ["#FFA1B5", "#7B68EE", "#87CEFA", "#B22222", "#FFE29A", "#D2B48C", "#90EE90", "#FF69B4", "#EE82EE", "#6A5ACD", "#b8436d", "#9ACD32", "#00d9f9", "#800080", "#FF6347", "#DDA0DD", "#a4c73c", "#a4add3", "#008000", "#DAA520", "#00BFFF", "#2F4F4F", "#FF8C00", "#A9A9A9", "#FFB6C1", "#00FFFF", "#6495ED", "#7FFFD4", "#F0F8FF", "#7FFF00", "#008B8B", "#9932CC", "#E9967A", "#8FBC8F", "#483D8B", "#D3D3D3", "#ADD8E6"] }];
  public pieChartType: string = 'pie';

  constructor(
    public router: Router,
    private roomService: RoomService,
    private ordersService: OrdersService,
    private authenticationService: AuthenticationService) {
  }

  public ngOnInit(): void {
    this.registerSignalR();
    this.loadRooms();
  }

  private registerSignalR() {

    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.orderSignalrHubUrl, { accessTokenFactory: () => this.authenticationService.getToken() })
      .build();

    hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))

    hubConnection.on('UpdatedOrderStatus', (data) => {
      // console.log(data);
      this.loadOrdersInRoom(data.roomId);
    });
  }

  private loadRooms(): void {
    this.roomService.getRooms()
      .subscribe(rooms => this.onLoadRooms(rooms));
  }

  private onLoadRooms(rooms: Room[]): void {
    this.rooms = rooms;
    if (rooms.length > 0) {
      if (this.selectedRoom && this.rooms.some(r => r.name == this.selectedRoom.name))
        this.selectRoom(this.selectedRoom);
      else
        this.selectRoom(rooms[0]);
    }
  }

  public selectRoom(room: Room): boolean {
    this.rooms.forEach((r) => {
      r.isActive = r.id == room.id;
    });

    this.selectedRoom = room;

    this.loadOrdersInRoom(this.selectedRoom.id);

    return false;
  }

  private loadOrdersInRoom(roomId: number) {
    this.ordersService.getOrder(roomId)
      .subscribe(
        order => this.onLoadOrder(order),
        error => alert(ErrorHelper.getErrorMessage(error))
      );
  }

  private onLoadOrder(order: Order): void {
    this.order = order;
    if (!this.order)
      this.order = new Order();
    this.preparePizzaChart();
  }

  private preparePizzaChart(): void {
    let pieChartLabels: string[] = [];
    let pieChartData: number[] = [];

    if (this.order && this.order.orderItems) {
      this.order.orderItems.forEach((o) => {
        pieChartLabels.push(o.purchaserEmail);
        pieChartData.push(o.quantity);
      });

      if (this.order.totalPizzas == 0)
        return;

      if (this.order.freeSlicesToGrab > 0) {
        pieChartLabels.push('FREE');
        pieChartData.push(this.order.freeSlicesToGrab);
      }

      this.pieChartLabels = pieChartLabels;
      this.pieChartData = pieChartData;

      setTimeout(() => {
        if (this.chart && this.chart.chart && this.chart.chart.config) {
          this.chart.chart.config.data.labels = this.pieChartLabels;
          //this.chart.chart.config.data.datasets = this.pieChartData;
          this.chart.chart.config.data.colors = this.pieChartColours;
          this.chart.chart.update();
        }
      });
    }
  }

  public makeOrder(): boolean {
    let makeOrderCommand = new MakeOrder();
    makeOrderCommand.roomId = this.selectedRoom.id;
    makeOrderCommand.purchaserId = this.authenticationService.getLoggedUserId();
    makeOrderCommand.quantity = this.quantity;
    this.ordersService.makeOrder(makeOrderCommand)
      .subscribe(
        order => this.onLoadOrder(order),
        error => alert(ErrorHelper.getErrorMessage(error))
      );
    return false;
  }

  public cancel(): boolean {
    let cancelOrderCommand = new CancelOrder();
    cancelOrderCommand.orderId = this.order.orderId;
    cancelOrderCommand.purchaserId = this.authenticationService.getLoggedUserId();

    this.ordersService.removeOrder(cancelOrderCommand)
      .subscribe(
        order => this.onLoadOrder(order),
        error => alert(ErrorHelper.getErrorMessage(error))
    );

    return false;
  }

  public refresh(): boolean {
    this.loadOrdersInRoom(this.selectedRoom.id);
    return false;
  }

  public approveOrders(): void {
    this.ordersService.approveOrders(this.order.orderId)
      .subscribe(
      result => this.refresh(),
      error => alert(ErrorHelper.getErrorMessage(error))
      );
  }

  // events
  public chartClicked(e: any): void {
    //console.log(e);
  }

  public chartHovered(e: any): void {
    //console.log(e);
  }
}
