import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Room, User } from './room.model';
import { RoomService } from './rooms.service';
import { ErrorHelper } from '../shared/error-helper';
import { AuthenticationService } from '../shared/auth/authentication.service';

@Component({
  selector: 'app-root',
  providers: [
    RoomService,
    AuthenticationService
  ],
  templateUrl: './rooms.component.html'
})
export class RoomsComponent implements OnInit {
  
  public rooms: Room[] = [];
  public selectedRoom: Room;
  public users: User[] = [];

  constructor(
    public router: Router,
    private roomService: RoomService,
    private authenticationService: AuthenticationService) {
    this.selectedRoom = new Room();
  }

  public ngOnInit(): void {
    this.loadRooms();
    this.loadUsers();
  }

  private loadRooms(): void {
    this.roomService.getRooms()
      .subscribe(
        rooms => this.rooms = rooms,
        error => alert(ErrorHelper.getErrorMessage(error))
      );
  }

  private loadUsers(): void {
    this.roomService.getUsers()
      .subscribe(
        users => this.users = users,
        error => alert(ErrorHelper.getErrorMessage(error))
      );
  }

  public onManagerChange(value: any) {
    this.selectedRoom.managerName = this.users.find(u => u.userId == value).email;
  }

  public saveRoom(): void {
    if (!this.selectedRoom.id) { // New Room

      this.roomService.addRoom(this.selectedRoom)
        .subscribe(result => {
          if (result) {
            this.selectedRoom = new Room();
            this.loadRooms();
          }
        },
          error => alert(ErrorHelper.getErrorMessage(error))
        );
    } else { // Update Room

      this.roomService.editRoom(this.selectedRoom)
        .subscribe(result => {
          if (result) {
            this.selectedRoom = new Room();
            this.loadRooms();
          }
        },
          error => alert(ErrorHelper.getErrorMessage(error))
        );
    }
  }

  public selectRoom(room: Room): boolean {
    console.log('selectRoom:', room);
    this.selectedRoom = room;
    return false;
  }

  public newRoom(): boolean {
    this.selectedRoom = new Room();
    return false;
  }

  public removeRoom(): boolean {
    if(!this.selectedRoom)
      return false;

    this.roomService.removeRoom(this.selectedRoom.id)
      .subscribe(result => {
        if (result) {
          this.selectedRoom = new Room();
          this.loadRooms();
        }
    },
    error => alert(ErrorHelper.getErrorMessage(error))
    );
    return false;
  }
}
