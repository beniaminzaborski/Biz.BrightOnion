export class UserProfile {
  constructor(
    public email: string,
    public notificationEnabled: boolean = false
  ) { }
}
