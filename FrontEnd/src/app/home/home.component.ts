import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  isSidebarClosed = false;
  currentView: 'task' | 'calendar' | 'day' = 'calendar';
  currentMonth = new Date();
  selectedDate: Date | null = null;

  constructor() {}

  ngOnInit() {}

  toggleSidebar() {
    this.isSidebarClosed = !this.isSidebarClosed;
  }

  switchView(view: 'calendar' | 'task' | 'day'): void {
    this.currentView = view;
  }

  selectDay(date: Date) {
    this.selectedDate = date;
    this.switchView('day'); 
  }
}