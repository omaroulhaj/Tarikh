import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from '../dialog/dialog.component';

@Component({
  selector: 'app-foget-password',
  templateUrl: './foget-password.component.html',
  styleUrl: './foget-password.component.css'
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
          this.showDialog('Email valid', 'Code sent in your email.', 'info');
        },
        error: (error) => {
          this.loading = false;
          this.showDialog('Email invalid', error.message, 'error');
        }
      });
    } else {
      this.showDialog('Formulaire invalide', 'Veuillez remplir correctement le champs du formulaire.', 'warning');
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
