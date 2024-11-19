import { Component, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  isClosed = false;
  menuItems = [
    { label: 'Home', route: '/home', icon: 'fa fa-home' },
    { label: 'Profile', route: '/add-task-modal', icon: 'fa fa-user' },
    { label: 'Settings', route: '/settings', icon: 'fa fa-cog' },
  ];

  constructor(private router: Router) {}

  @Output() toggleSidebar = new EventEmitter<void>();

  toggleSidebarState() {
    this.isClosed = !this.isClosed;
    this.toggleSidebar.emit(); 
  }

  navigate(route: string) {
    this.router.navigate([route]);
  }
}
