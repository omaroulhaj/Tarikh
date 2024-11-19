import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, provideHttpClient, withFetch } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './Auth/login/login.component';
import { RegisterComponent } from './Auth/register/register.component'; 

// Angular Material Modules
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';

// PrimeNG Modules
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { ToastModule } from 'primeng/toast';

// Calendar Module (Ajouté pour le calendrier)
import { FullCalendarModule } from '@fullcalendar/angular'; // Si vous utilisez FullCalendar

// Components
import { HomeComponent } from './home/home.component';
import { DialogComponent } from './dialog/dialog.component';
import { FogetPasswordComponent } from './Auth/foget-password/foget-password.component';
import { ResetPasswordComponent } from './Auth/reset-password/reset-password.component';
import { ActivateAccountComponent } from './Auth/activate-account/activate-account.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NavbarComponent } from './navbar/navbar.component';

// Ajouter des imports spécifiques pour le calendrier si vous l'utilisez dans votre application
import { CalendarModule } from 'primeng/calendar';
import { AddTaskModalComponent } from './add-task-modal/add-task-modal.component';
import { ProfilComponent } from './profil/profil.component'; // Si vous utilisez PrimeNG Calendar

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    DialogComponent,
    FogetPasswordComponent,
    ResetPasswordComponent,
    ActivateAccountComponent,
    SidebarComponent,
    NavbarComponent,
    AddTaskModalComponent,
    ProfilComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,

    // Angular Material Modules
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatToolbarModule,
    MatIconModule,

    // PrimeNG Modules
    ButtonModule,
    InputTextModule,
    CardModule,
    ToastModule,
    
    // Ajouter le FullCalendar ou PrimeNG Calendar
    FullCalendarModule, // Pour FullCalendar
    CalendarModule // Pour PrimeNG Calendar (si vous optez pour PrimeNG)
  ],
  providers: [
    provideHttpClient(withFetch())
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
