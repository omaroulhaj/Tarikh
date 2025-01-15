import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../services/profile.service';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from '../dialog/dialog.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-profil',
  templateUrl: './profil.component.html',
  styleUrls: ['./profil.component.css']
})
export class ProfilComponent implements OnInit {
  isSidebarClosed = false;
  userProfile: any;
  profileData: any = {};
  passwordData: any = {};
  currentView: 'info' | 'password' | 'delete' = 'info';

  // Formulaires réactifs
  profileForm: FormGroup;
  passwordForm: FormGroup;

  constructor(
    private profileService: ProfileService,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog,
    private fb: FormBuilder
  ) {
    // Initialisation du formulaire de profil
    this.profileForm = this.fb.group({
      prenom: ['', Validators.required],
      nom: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''] // Optionnel
    });

    // Initialisation du formulaire de changement de mot de passe
    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  ngOnInit(): void {
    console.log('Initializing ProfilComponent...');
    const token = localStorage.getItem('token');
    console.log('Token from localStorage:', token);

    if (!token || this.authService.isTokenExpired()) {
      console.log('Token is missing or expired. Redirecting to login...');
      this.router.navigate(['/login']);
      return;
    }

    console.log('Token is valid. Loading user profile...');
    this.loadUserProfile();
  }

  loadUserProfile(): void {
    this.profileService.getUserProfile().subscribe(
      (data) => {
        this.userProfile = data;
        this.profileData = { ...data };
        this.profileForm.patchValue(this.profileData); // Remplir le formulaire avec les données du profil
      },
      (error) => {
        console.error('Error loading user profile:', error);
        if (error.status === 401) {
          this.router.navigate(['/login']);
        }
      }
    );
  }

  updateProfile(): void {
    if (this.profileForm.invalid) {
      this.dialog.open(DialogComponent, {
        data: {
          title: 'Error',
          message: 'Please fill out all required fields correctly.',
          type: 'error'
        }
      });
      return;
    }

    this.profileService.updateUserProfile(this.profileForm.value).subscribe(
      (response) => {
        this.dialog.open(DialogComponent, {
          data: {
            title: 'Success',
            message: 'Profile updated successfully!',
            type: 'info'
          }
        });
        this.loadUserProfile();
      },
      (error) => {
        console.error('Error updating profile:', error);
        this.dialog.open(DialogComponent, {
          data: {
            title: 'Error',
            message: error.error?.message || 'Failed to update profile. Please try again.',
            type: 'error'
          }
        });
      }
    );
  }

  changePassword(): void {
    if (this.passwordForm.invalid) {
      this.dialog.open(DialogComponent, {
        data: {
          title: 'Error',
          message: 'Please fill out all required fields correctly.',
          type: 'error'
        }
      });
      return;
    }

    this.profileService.changePassword(this.passwordForm.value).subscribe(
      (response) => {
        this.dialog.open(DialogComponent, {
          data: {
            title: 'Success',
            message: 'Password changed successfully!',
            type: 'info'
          }
        });
        this.passwordForm.reset(); // Réinitialiser le formulaire après un changement réussi
      },
      (error) => {
        console.error('Error changing password:', error);
        this.dialog.open(DialogComponent, {
          data: {
            title: 'Error',
            message: error.error?.message || 'Failed to change password. Please try again.',
            type: 'error'
          }
        });
      }
    );
  }

  deleteAccount(): void {
    const dialogRef = this.dialog.open(DialogComponent, {
      data: {
        title: 'Confirm Deletion',
        message: 'Are you sure you want to delete your account? This action cannot be undone.',
        type: 'confirmation'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const password = prompt('Please enter your password to confirm account deletion:');
        if (password) {
          this.profileService.deleteUserAccount(password).subscribe(
            (response) => {
              this.dialog.open(DialogComponent, {
                data: {
                  title: 'Success',
                  message: 'Account deleted successfully!',
                  type: 'info'
                }
              });
              this.router.navigate(['/login']);
            },
            (error) => {
              this.dialog.open(DialogComponent, {
                data: {
                  title: 'Error',
                  message: error.error?.message || 'Failed to delete account. Please try again.',
                  type: 'error'
                }
              });
            }
          );
        }
      }
    });
  }

  toggleSidebar(): void {
    this.isSidebarClosed = !this.isSidebarClosed;
  }

  switchView(view: 'info' | 'password' | 'delete'): void {
    this.currentView = view;
  }
}