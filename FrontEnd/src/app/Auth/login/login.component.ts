import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from '../../dialog/dialog.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.loading = true;
      const { email, password } = this.loginForm.value;

      this.authService.login(email, password).subscribe({
        next: () => {
          this.loading = false;
          this.showDialog('Connexion réussie', 'Vous êtes maintenant connecté.', 'info');
          this.router.navigate(['/home']);
        },
        error: (error) => {
          this.loading = false;
          this.showDialog('Échec de la connexion', error.message, 'error');
          console.error('Login failed', error.message);
        }
      });
    } else {
      this.showDialog('Formulaire invalide', 'Veuillez remplir correctement tous les champs du formulaire.', 'warning');
    }
  }

  navigateToRegister() {
    this.router.navigate(['/register']);
  }

  navigateToForgotPassword() {
    this.router.navigate(['/forget-password']);
  }

  private showDialog(title: string, message: string, type: 'info' | 'warning' | 'error' | 'confirmation'): void {
    this.dialog.open(DialogComponent, {
      width: '500px',
      data: { title, message, type }
    });
  }
}
