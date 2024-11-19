import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  isSidebarClosed = false;
  currentMonth = new Date();
  calendarWeeks: { date: Date; otherMonth: boolean }[][] = [];
  selectedDate: Date | null = null;
  tasks: { [date: string]: { title: string }[] } = {};
  isModalOpen = false; // For task modal visibility

  // Toggle de la barre latérale
  toggleSidebar() {
    this.isSidebarClosed = !this.isSidebarClosed;
  }

  // Générer les semaines du mois
  ngOnInit() {
    this.generateCalendar();
  }

  // Générer les semaines du calendrier pour le mois courant
  generateCalendar() {
    const firstDayOfMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth(), 1);
    const lastDayOfMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() + 1, 0);
    const firstDayOfWeek = firstDayOfMonth.getDay();
    const daysInMonth = lastDayOfMonth.getDate();

    this.calendarWeeks = [];
    let week: { date: Date; otherMonth: boolean }[] = [];

    // Génération des jours pour chaque semaine
    for (let i = 1 - firstDayOfWeek; i <= daysInMonth; i++) {
      const date = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth(), i);
      if (i <= 0 || i > daysInMonth) {
        week.push({ date, otherMonth: true });
      } else {
        week.push({ date, otherMonth: false });
      }

      if (week.length === 7) {
        this.calendarWeeks.push(week);
        week = [];
      }
    }

    // Ajouter la dernière semaine si elle n'est pas vide
    if (week.length > 0) {
      this.calendarWeeks.push(week);
    }
  }

  // Naviguer vers le mois précédent
  previousMonth() {
    this.currentMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() - 1, 1);
    this.generateCalendar();
  }

  // Naviguer vers le mois suivant
  nextMonth() {
    this.currentMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() + 1, 1);
    this.generateCalendar();
  }

  selectDay(day: { date: Date; otherMonth: boolean }) {
    this.selectedDate = day.date;
  }

  openAddTaskModal() {
    this.isModalOpen = true; // Open the modal
  }

  closeModal() {
    this.isModalOpen = false; // Close the modal
  }

  getTasksForDay(date: Date) {
    const dateKey = date.toISOString().split('T')[0];
    return this.tasks[dateKey] || [];
  }

  isSelectedDay(date: Date) {
    return this.selectedDate?.toISOString().split('T')[0] === date.toISOString().split('T')[0];
  }
}
