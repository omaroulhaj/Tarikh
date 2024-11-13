import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from '../../dialog/dialog.component';
import { first } from 'rxjs';

@Component({
  selector: 'app-activate-account',
  templateUrl: './activate-account.component.html',
  styleUrls: ['./activate-account.component.css']
})
export class ActivateAccountComponent implements OnInit {
  userId: string | null = null;
  token: string | null = null;
  loading = false;
  isDialogOpen = false; // Ajout d'un état pour vérifier si un dialogue est déjà ouvert.
  isEmailConfirmed = false; // Ajout d'une variable pour vérifier si l'email est déjà confirmé.

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    // Récupérer les paramètres de l'URL une seule fois
    this.route.queryParams.pipe(first()).subscribe(params => {
      this.userId = params['userId'];
      this.token = params['token'];

      // Vérifier si les paramètres sont présents
      if (this.userId && this.token) {
        this.activateAccount();
      } else {
        this.showDialog('Invalid Parameters', 'Missing userId or token.', 'error');
      }
    });
  }

  activateAccount(): void {
    if (this.userId && this.token && !this.isEmailConfirmed) {
      this.loading = true;

      this.authService.confirmEmail(this.userId, this.token).subscribe({
        next: (response) => {
          console.log('Email confirmed response:', response); // Log de la réponse
          this.loading = false; // Set after response handling
          this.isEmailConfirmed = true; // Marquer l'email comme confirmé

          if (this.isDialogOpen) {
            this.dialog.closeAll(); 
          }

          this.showDialog('Email Confirmed', 'Your email has been confirmed successfully.', 'info');
          this.router.navigate(['/login']);
        },
        error: (error) => {
          this.loading = false;
          const errorMessage = error?.error?.message || error?.message || 'Email confirmation failed.';
          console.error('Error details:', error); // Log détaillé de l'erreur

          if (!this.isEmailConfirmed) {
            this.showDialog('Error', errorMessage, 'error');
          }
        }
      });
    }
  }

  private showDialog(title: string, message: string, type: 'info' | 'warning' | 'error' | 'confirmation'): void {
    this.isDialogOpen = true; 
    const dialogRef = this.dialog.open(DialogComponent, {
      width: '500px',
      data: { title, message, type }
    });

    dialogRef.afterClosed().subscribe(() => {
      this.isDialogOpen = false;
    });
  }
}
