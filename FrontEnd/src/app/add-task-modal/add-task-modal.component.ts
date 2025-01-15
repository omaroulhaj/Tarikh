import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../services/task.service';
import { AuthService } from '../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-task-modal',
  templateUrl: './add-task-modal.component.html',
  styleUrls: ['./add-task-modal.component.css']
})
export class AddTaskModalComponent implements OnInit {
  taskForm!: FormGroup;
  token: string = '';

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.taskForm = this.fb.group({
      date: ['', Validators.required],
      title: ['', [Validators.required, Validators.maxLength(100)]],
      status: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.taskForm.valid) {
      const taskData = {
        ...this.taskForm.value,
        date: new Date(this.taskForm.value.date).toISOString(), 
      };

      this.token = localStorage.getItem('token') || '';

      this.taskService.createTask(taskData, this.token).subscribe({
        next: () => {
          this.snackBar.open('Tâche créée avec succès !', 'Fermer', { duration: 3000 });
          this.taskForm.reset(); 
        },
        error: (error) => {
          this.snackBar.open(`Erreur : ${error.message}`, 'Fermer', { duration: 3000 });
        }
      });
    }
  }
}
