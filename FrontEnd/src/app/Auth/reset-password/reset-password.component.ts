import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from '../../dialog/dialog.component';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {
  resetPwForm: FormGroup;
  loading = false;
  email: string | null = null;
  token: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog
  ) {
    // Initialize the form group with password and confirmPassword fields
    this.resetPwForm = this.fb.group({
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, {
      // Custom validator to ensure passwords match
      validator: this.passwordMatchValidator
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      this.token = params['token'];
  
      if (this.email) {
        this.resetPwForm.get('email')?.setValue(this.email);
      }
    });
  }
  

  passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const password = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    return password === confirmPassword ? null : { passwordsDontMatch: true };
  }

  onSubmit() {
    if (this.resetPwForm.valid && this.token) {
      this.loading = true;
      const { newPassword } = this.resetPwForm.value;
  
      // Ajoutez l'email manuellement aux données de la requête
      const email = this.resetPwForm.get('email')?.value;
  
      this.authService.resetPassword(email, this.token, newPassword).subscribe({
        next: () => {
          this.loading = false;
          this.showDialog('Password Reset', 'Your password has been reset successfully.', 'info');
          this.router.navigate(['/login']);
        },
        error: (error) => {
          this.loading = false;
          let errorMessage = error?.error?.message || error?.message || 'An unexpected error occurred. Please try again later.';
          if (error?.error?.Errors) {
            errorMessage = error.error.Errors.join(', ');  // Afficher les erreurs spécifiques, si elles existent
          }
          this.showDialog('Error', errorMessage, 'error');
        }
      });
    } else {
      this.showDialog('Form Invalid', 'Please ensure all fields are filled correctly.', 'warning');
    }
  }
  
  private showDialog(title: string, message: string, type: 'info' | 'warning' | 'error' | 'confirmation'): void {
    this.dialog.open(DialogComponent, {
      width: '500px',
      data: { title, message, type }
    });
  }
}
