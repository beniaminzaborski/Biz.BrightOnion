export class MakeOrder {
  public roomId: number;
  public purchaserId: number;
  public quantity: number;
}

export class Order {
  public orderId: number;
  public roomId: number;
  public day: Date;
  public totalPizzas: number = 0;
  public freeSlicesToGrab: number = 0;
  public orderItems: OrderItem[];
}

export class OrderItem {
  public orderItemId: number;
  public purchaserId: number;
  public quantity: number;
}

export class OrdersApproval {
  public pizzaQuantity: number;
  public room: string;
}
