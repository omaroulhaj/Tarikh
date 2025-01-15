import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HolidayService } from '../services/holiday.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-add-holiday',
  templateUrl: './add-holiday.component.html',
  providers: [DatePipe],  // Ajoutez DatePipe ici
  styleUrls: ['./add-holiday.component.css']
})
export class AddHolidayComponent implements OnInit {
  holidayForm!: FormGroup;
  formattedDate: string = '';  // Déclarez la variable pour stocker la date formatée

  constructor(
    private fb: FormBuilder,
    private holidayService: HolidayService,
    private snackBar: MatSnackBar,
    private datePipe: DatePipe  // Injectez DatePipe dans le constructeur
  ) {}

  ngOnInit(): void {
    this.holidayForm = this.fb.group({
      date: ['', Validators.required],
      title: ['', [Validators.required, Validators.maxLength(100)]],
      details: ['', Validators.required],
      category: ['', Validators.required]
    });
  }

  // Utilisation de DatePipe pour formater la date dans le bon format
  onSubmit(): void {
    if (this.holidayForm.valid) {
      // Formatez la date avant d'envoyer au backend
      const rawDate = new Date(this.holidayForm.value.date);
      this.formattedDate = this.datePipe.transform(rawDate, 'yyyy-MM-ddTHH:mm:ss') || ''; // Par exemple, format ISO
  
      // Utilisez les bons noms de champs qui correspondent à ce qui est attendu par le backend
      const holidayData = {
        Nom: this.holidayForm.value.title,  // Assurez-vous de correspondre au champ attendu par le backend
        Categorie: this.holidayForm.value.category,  // Idem ici
        Date: this.formattedDate,
        Details: this.holidayForm.value.details
      };
  
      // Appeler le service pour ajouter le jour férié
      this.holidayService.addHoliday(holidayData).subscribe(
        (response) => {
          this.snackBar.open('Jour férié ajouté avec succès!', 'Fermer', { duration: 3000 });
          this.holidayForm.reset();
        },
        (error) => {
          // Afficher les erreurs de validation pour l'utilisateur
          if (error.error.errors) {
            const validationErrors = error.error.errors;
            const errorMessages = Object.values(validationErrors).join(' ');
            this.snackBar.open(`Erreur de validation : ${errorMessages}`, 'Fermer', { duration: 3000 });
          } else {
            this.snackBar.open('Erreur lors de l\'ajout du jour férié.', 'Fermer', { duration: 3000 });
          }
        }
      );
    }
  }
  
  
}
