import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-holiday',
  templateUrl: './add-holiday.component.html',
  styleUrl: './add-holiday.component.css'
})
export class AddHolidayComponent {
  holidayForm!: FormGroup;
  token: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.holidayForm = this.fb.group({
      date: ['', Validators.required],
      title: ['', [Validators.required, Validators.maxLength(100)]],
    });
  }

  onSubmit(): void {
    if (this.holidayForm.valid) {
      const taskData = {
        ...this.holidayForm.value,
        date: new Date(this.holidayForm.value.date).toISOString(), 
      };

      this.token = localStorage.getItem('token') || '';
    }
  }
}
