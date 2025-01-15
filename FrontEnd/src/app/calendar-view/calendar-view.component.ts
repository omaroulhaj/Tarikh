import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TaskService } from '../services/task.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-calendar-view',
  templateUrl: './calendar-view.component.html',
  styleUrls: ['./calendar-view.component.css'],
  providers: [DatePipe]
})
export class CalendarViewComponent implements OnInit {
  @Input() currentMonth: Date = new Date();
  @Output() daySelected = new EventEmitter<Date>();
  calendarWeeks: { date: Date; otherMonth: boolean }[][] = [];
  tasks: { [date: string]: { title: string; status: string }[] } = {};
  searchQuery: string = '';
  statusFilter: string = '';

  constructor(private taskService: TaskService, private datePipe: DatePipe) {}

  ngOnInit() {
    this.generateCalendar();
    this.loadTasksForMonth();
  }

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

  previousMonth() {
    this.currentMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() - 1, 1);
    this.generateCalendar();
    this.loadTasksForMonth();
  }

  nextMonth() {
    this.currentMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() + 1, 1);
    this.generateCalendar();
    this.loadTasksForMonth();
  }

  selectDay(date: Date) {
    this.daySelected.emit(date);
  }

  loadTasksForMonth() {
    const year = this.currentMonth.getFullYear();
    const month = this.currentMonth.getMonth() + 1;
    this.taskService.getTasksByMonthAndYear(year, month).subscribe(
      (tasks: any) => {
        this.tasks = {};
        tasks.forEach((task: any) => {
          const dateKey = this.datePipe.transform(new Date(task.date), 'yyyy-MM-dd')!;
          if (!this.tasks[dateKey]) {
            this.tasks[dateKey] = [];
          }
          this.tasks[dateKey].push({ title: task.title, status: task.status });
        });
      },
      (error) => {
        console.error('Error loading tasks:', error);
      }
    );
  }

  filterTasks() {
    this.loadTasksForMonth();
  }

  getFilteredTasksForDay(date: Date) {
    const dateKey = this.datePipe.transform(date, 'yyyy-MM-dd')!;
    const tasks = this.tasks[dateKey] || [];
    return tasks.filter(task =>
      task.title.toLowerCase().includes(this.searchQuery.toLowerCase()) &&
      (this.statusFilter === '' || task.status === this.statusFilter)
    );
  }
}