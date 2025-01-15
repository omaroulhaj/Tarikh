import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TaskService } from '../services/task.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-day-view',
  templateUrl: './day-view.component.html',
  styleUrls: ['./day-view.component.css'],
  providers: [DatePipe]
})
export class DayViewComponent implements OnInit {
  @Input() dayViewDate: Date = new Date();
  @Output() dateChanged = new EventEmitter<Date>();
  tasks: { title: string; status: string }[] = [];
  filteredTasks: { title: string; status: string }[] = [];
  searchQuery: string = '';
  statusFilter: string = '';

  constructor(private taskService: TaskService, private datePipe: DatePipe) {}

  ngOnInit() {
    this.loadTasksForDay();
  }

  loadTasksForDay() {
    const dateKey = this.datePipe.transform(this.dayViewDate, 'yyyy-MM-dd')!;
    this.taskService.getTasksByMonthAndYear(this.dayViewDate.getFullYear(), this.dayViewDate.getMonth() + 1).subscribe(
      (tasks: any) => {
        this.tasks = tasks.filter((task: any) => {
          const taskDate = this.datePipe.transform(new Date(task.date), 'yyyy-MM-dd')!;
          return taskDate === dateKey;
        });
        this.filterTasks();
      },
      (error) => {
        console.error('Error loading tasks:', error);
      }
    );
  }

  navigateDay(direction: 'previous' | 'next') {
    const newDate = new Date(this.dayViewDate);
    newDate.setDate(newDate.getDate() + (direction === 'previous' ? -1 : 1));
    this.dayViewDate = newDate;
    this.dateChanged.emit(newDate);
    this.loadTasksForDay();
  }

  filterTasks() {
    this.filteredTasks = this.tasks.filter(task =>
      task.title.toLowerCase().includes(this.searchQuery.toLowerCase()) &&
      (this.statusFilter === '' || task.status === this.statusFilter)
    );
  }
}