import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from '../../dialog/dialog.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  submitted = false;
  errorMessage: string | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog
  ) {
    this.registerForm = this.formBuilder.group({
      prenom: ['', Validators.required],
      nom: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)
        ]
      ],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      dateDeNaissance: ['', Validators.required]
    });
  }

  ngOnInit(): void {}

  get formControls() {
    return this.registerForm.controls;
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.registerForm.invalid) {
      return;
    }

    const userData = {
      prenom: this.registerForm.value.prenom,
      nom: this.registerForm.value.nom,
      email: this.registerForm.value.email,
      password: this.registerForm.value.password,
      phoneNumber: this.registerForm.value.phoneNumber,
      dateDeNaissance: this.registerForm.value.dateDeNaissance
    };

    this.authService.register(userData).subscribe({
      next: () => {
        this.showDialog('Inscription réussie', 'Votre inscription a été réalisée avec succès. Vous pouvez maintenant vous connecter.', 'info');
        this.router.navigate(['/login']);
      },
      error: (error) => {
        this.showDialog('Échec de l’inscription', error.message, 'error');
        console.error('Registration error:', error.message);
      }
    });
  }

  private showDialog(title: string, message: string, type: 'info' | 'warning' | 'error' | 'confirmation'): void {
    this.dialog.open(DialogComponent, {
      width: '500px',
      data: { title, message, type }
    });
  }
}
