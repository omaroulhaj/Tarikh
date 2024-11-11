import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service'; // Ensure this path is correct
import { Router } from '@angular/router'; // For navigation after registration

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'] // Include CSS styles if necessary
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  submitted = false;
  errorMessage: string | null = null;

  constructor(private formBuilder: FormBuilder, private authService: AuthService, private router: Router) {
    // Initialize the form
    this.registerForm = this.formBuilder.group({
      prenom: ['', Validators.required],
      nom: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]], // Adjust the pattern as needed
      dateDeNaissance: ['', Validators.required]
    });
  }

  ngOnInit(): void {}

  // Access the form controls
  get formControls() {
    return this.registerForm.controls;
  }

  // Form submission
  onSubmit(): void {
    this.submitted = true;

    // Check if the form is valid
    if (this.registerForm.invalid) {
      return;
    }

    // Retrieve form data
    const userData = {
      prenom: this.registerForm.value.prenom,
      nom: this.registerForm.value.nom,
      email: this.registerForm.value.email,
      password: this.registerForm.value.password,
      phoneNumber: this.registerForm.value.phoneNumber,
      dateDeNaissance: this.registerForm.value.dateDeNaissance
    };

    // Call the authentication service to register
    this.authService.register(userData).subscribe(
      response => {
        console.log('Registration successful!', response);
        this.router.navigate(['/login']); // Redirect to the login page after successful registration
      },
      error => {
        this.errorMessage = error.error.message || 'Error during registration. Please try again.'; // Handle errors
        console.error('Registration error:', error);
      }
    );
  }
}
