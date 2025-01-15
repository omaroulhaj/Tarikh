import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DialogComponent } from '../dialog/dialog.component';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['nom', 'prenom', 'email', 'phoneNumber', 'roles', 'status', 'action'];
  dataSource: MatTableDataSource<any> = new MatTableDataSource();
  searchKeyword: string = '';
  isSidebarClosed = false;
  currentView: 'user-list' | 'add-user' = 'user-list'; // Mise à jour du type de currentView
  isSearchHidden: boolean = true; // État de l'affichage de la barre de recherche
  index = 0;
  btnClass: any;
  iptClass:any;

tabChange(data: number){
this.index = data;
}
btnClickHandler() {
  if(this.btnClass) {
this.btnClass  = '';
this.iptClass  = '';
  } else {
this.btnClass = 'close'
this.iptClass = 'square'

  }
}

  @ViewChild(MatPaginator) paginator!: MatPaginator; // Non-null assertion
  @ViewChild(MatSort) sort!: MatSort; // Non-null assertion

  toggleSidebar() {
    this.isSidebarClosed = !this.isSidebarClosed;
  }

  switchView(view: 'user-list' | 'add-user'): void {
    this.currentView = view;
  }

  constructor(
    private userService: UserService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.fetchUsers();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  fetchUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users) => (this.dataSource.data = users),
      error: (err) => console.error('Error fetching users:', err)
    });
  }

  applyFilter(filterValue: string): void {
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  confirmDelete(userId: string, email: string): void {
    const dialogRef = this.dialog.open(DialogComponent, {
      data: {
        type: 'confirmation',
        title: 'Confirm Deletion',
        message: `Are you sure you want to delete the user with email: "${email}"?`
      }
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.deleteUser(userId);
      }
    });
  }

  deleteUser(userId: string): void {
    this.userService.deleteUser(userId).subscribe({
      next: () => {
        this.snackBar.open('User deleted successfully', 'Close', { duration: 3000 });
        this.fetchUsers();  // Rechargement des utilisateurs après suppression
      },
      error: (err) => {
        console.error('Error deleting user:', err);
        this.snackBar.open('Error deleting user.', 'Close', { duration: 3000 });
      }
    });
  }
}