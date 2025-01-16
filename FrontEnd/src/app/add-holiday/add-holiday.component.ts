import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HolidayService } from '../services/holiday.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-holiday',
  templateUrl: './add-holiday.component.html',
  styleUrls: ['./add-holiday.component.css']
})
export class AddHolidayComponent implements OnInit {
  holidayForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private holidayService: HolidayService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    // Initialisation du formulaire avec les champs nécessaires
    this.holidayForm = this.fb.group({
      date: ['', Validators.required],
      title: ['', [Validators.required, Validators.maxLength(100)]],
      details: ['', Validators.required],
      category: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.holidayForm.valid) {
      // Préparer les données à envoyer au backend
      const holidayData = {
        Nom: this.holidayForm.value.title,
        Details: this.holidayForm.value.details,
        DateJour: new Date(this.holidayForm.value.date).toISOString(),
        Categorie: this.holidayForm.value.category
      };

      // Appel du service pour créer le jour férié
      this.holidayService.addHoliday(holidayData).subscribe({
        next: () => {
          this.snackBar.open('Jour férié créé avec succès !', 'Fermer', { duration: 3000 });
          this.holidayForm.reset(); // Réinitialiser le formulaire
        },
        error: (error) => {
          this.snackBar.open(`Erreur : ${error.message}`, 'Fermer', { duration: 3000 });
        }
      });
    } else {
      this.snackBar.open('Veuillez remplir tous les champs correctement.', 'Fermer', { duration: 3000 });
    }
  }
}
