import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from '../../dialog/dialog.component';

@Component({
  selector: 'app-foget-password',
  templateUrl: './foget-password.component.html',
  styleUrls: ['./foget-password.component.css']
})
export class FogetPasswordComponent {
  forgetPwForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog
  ) {
    this.forgetPwForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  onSubmit() {
    if (this.forgetPwForm.valid) {
      this.loading = true;
      const { email } = this.forgetPwForm.value;
  
      this.authService.requestPasswordReset(email).subscribe({
        next: () => {
          this.loading = false;
          this.showDialog('Email valid', 'Password reset link sent to your email.', 'info');
          this.forgetPwForm.reset();
          this.forgetPwForm.markAsPristine();
          this.forgetPwForm.markAsUntouched();
        },
        error: (error) => {
          this.loading = false;
          const errorMessage = error?.error?.message || error?.message || 'An unexpected error occurred. Please try again later.';
          this.showDialog('Email invalid', errorMessage, 'error');
        }
      });
    } else {
      this.showDialog('Formulaire invalide', 'Veuillez remplir correctement le champ du formulaire.', 'warning');
    }
  }
  

  navigateToLogin() {
    this.router.navigate(['/login']);
  }

  private showDialog(title: string, message: string, type: 'info' | 'warning' | 'error' | 'confirmation'): void {
    this.dialog.open(DialogComponent, {
      width: '500px',
      data: { title, message, type }
    });
  }
}
