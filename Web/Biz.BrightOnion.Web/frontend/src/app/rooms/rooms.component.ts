import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Room } from './room.model';
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
  
  public room: Room;
  public rooms: Room[] = [];
  public selectedRoom: Room = null;

  constructor(
    public router: Router,
    private roomService: RoomService,
    private authenticationService: AuthenticationService) {
      this.room = new Room();
  }

  public ngOnInit(): void {
    this.loadRooms();
  }

  private loadRooms(): void {
    this.roomService.getRooms()
      .subscribe(
        rooms => this.rooms = rooms,
        error => alert(ErrorHelper.getErrorMessage(error))
      );
  }

  public addRoom(): void {
    this.room.managerId = this.authenticationService.getLoggedUserId();
    this.room.managerName = this.authenticationService.getLoggedUsername();
    this.roomService.addRoom(this.room)
      .subscribe(result => {
        if(result) {
          this.room.name = '';
          this.selectedRoom = null;
          this.loadRooms();
        }
      },
      error => alert(ErrorHelper.getErrorMessage(error))
    );
  }

  public selectRoom(room: Room): boolean {
    if(this.selectedRoom == room)
      this.selectedRoom = null;
    else
      this.selectedRoom = room;
    return false;
  }

  public removeRoom(): boolean {
    if(!this.selectedRoom)
      return false;

    this.roomService.removeRoom(this.selectedRoom.id)
      .subscribe(result => {
        if(result) {
          this.selectedRoom = null;
          this.loadRooms();
        }
    },
    error => alert(ErrorHelper.getErrorMessage(error))
    );
    return false;
  }
}
