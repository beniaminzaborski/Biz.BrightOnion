import { exec } from "child_process";

export class MakeOrder {
  public roomId: number;
  public quantity: number;
}

export class CancelOrder {
  public orderId: number;
}

export class ApproveOrder {
  public orderId: number;
  public roomId: number;
}

export class Order {
  public orderId: number;
  public roomId: number;
  public day: Date;
  public totalPizzas: number = 0;
  public freeSlicesToGrab: number = 0;
  public orderItems: OrderItem[];
  public isApproved: boolean;
}

export class OrderItem {
  public orderItemId: number;
  public purchaserId: number;
  public purchaserEmail: string;
  public quantity: number;
}
