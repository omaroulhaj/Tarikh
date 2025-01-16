import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { HolidayService } from '../services/holiday.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-calendar-holidays',
  templateUrl: './calendar-holidays.component.html',
  styleUrls: ['./calendar-holidays.component.css']
})
export class CalendarHolidaysComponent implements OnInit {
  @Input() currentMonth: Date = new Date();
  @Output() daySelected = new EventEmitter<Date>();

  calendarWeeks: { date: Date; otherMonth: boolean }[][] = [];
  holidays: { [date: string]: { title: string; status: string; details: string }[] } = {};
  searchQuery: string = '';
  statusFilter: string = '';

  constructor(private holidayService: HolidayService, private datePipe: DatePipe) {}

  ngOnInit() {
    this.generateCalendar();
    this.loadHolidaysForMonth();
  }

  // Génération du calendrier
  generateCalendar() {
    const firstDayOfMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth(), 1);
    const lastDayOfMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() + 1, 0);
    const firstDayOfWeek = firstDayOfMonth.getDay();
    const daysInMonth = lastDayOfMonth.getDate();

    this.calendarWeeks = [];
    let week: { date: Date; otherMonth: boolean }[] = [];

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

    if (week.length > 0) {
      this.calendarWeeks.push(week);
    }
  }

  // Changer le mois précédent
  previousMonth() {
    this.currentMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() - 1, 1);
    this.generateCalendar();
    this.loadHolidaysForMonth();
  }

  // Changer le mois suivant
  nextMonth() {
    this.currentMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() + 1, 1);
    this.generateCalendar();
    this.loadHolidaysForMonth();
  }

  // Sélectionner un jour
  selectDay(date: Date) {
    this.daySelected.emit(date);
  }

  // Charger les jours fériés pour le mois
  loadHolidaysForMonth() {
    this.holidayService.getHolidays().subscribe(
      (response: any) => {
        console.log('Réponse de l\'API:', response);

        const holidays = response.joursFeries || []; // Adapter en fonction de votre API
        if (!Array.isArray(holidays)) {
          console.error('Les jours fériés ne sont pas dans un format valide (attendu un tableau).');
          return;
        }

        this.holidays = {};
        holidays.forEach((holiday: any) => {
          const parsedDate = new Date(holiday.dateJour);
          const dateKey = this.datePipe.transform(parsedDate, 'yyyy-MM-dd')!;

          if (!this.holidays[dateKey]) {
            this.holidays[dateKey] = [];
          }

          this.holidays[dateKey].push({
            title: holiday.nom,
            status: holiday.categorie,
            details: holiday.details,
          });
        });

        console.log('Objet des jours fériés:', this.holidays);
      },
      (error) => {
        console.error('Erreur lors du chargement des jours fériés:', error);
      }
    );
  }

  // Méthode pour générer les classes du jour
  getDayClasses(day: { date: Date; otherMonth: boolean }) {
    const isHoliday = this.getFilteredHolidaysForDay(day.date).length > 0;
    const isCurrentDay = day.date.toDateString() === new Date().toDateString();

    return {
      'other-month': day.otherMonth,
      'holiday-day': isHoliday,
      'current-day': isCurrentDay,
    };
  }

  // Méthode pour filtrer les jours fériés d'un jour spécifique
  getFilteredHolidaysForDay(date: Date) {
    const dateKey = this.datePipe.transform(date, 'yyyy-MM-dd')!;
    const holidays = this.holidays[dateKey] || [];

    return holidays.filter(
      (holiday) =>
        holiday.title.toLowerCase().includes(this.searchQuery.toLowerCase()) &&
        (this.statusFilter === '' || holiday.status === this.statusFilter)
    );
  }
}
