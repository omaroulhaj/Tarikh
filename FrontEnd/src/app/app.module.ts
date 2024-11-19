import { RouterModule } from '@angular/router';  // Ajoutez cette ligne
import { AppComponent } from './app.component';
import { LoginComponent } from './Auth/login/login.component';
import { RegisterComponent } from './Auth/register/register.component';
import { HomeComponent } from './home/home.component';
import { DialogComponent } from './dialog/dialog.component';
import { FogetPasswordComponent } from './Auth/foget-password/foget-password.component';
import { ResetPasswordComponent } from './Auth/reset-password/reset-password.component';
import { ActivateAccountComponent } from './Auth/activate-account/activate-account.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NavbarComponent } from './navbar/navbar.component';
import { AddTaskModalComponent } from './add-task-modal/add-task-modal.component';
import { ProfilComponent } from './profil/profil.component';
import { AddUserFormComponent } from './user-management/add-user-form/add-user-form.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, provideHttpClient, withFetch } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

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
    ProfilComponent,
    AddUserFormComponent,
    UserManagementComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    MatButtonModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatTableModule,
    MatPaginatorModule,
    // Angular Material Modules
    MatChipsModule,
    MatCardModule,
    MatInputModule,
    MatToolbarModule,
    MatIconModule,
    InputTextModule,
    CardModule,
    ToastModule,
    // Ajouter le FullCalendar ou PrimeNG Calendar
    FullCalendarModule, // Pour FullCalendar
    CalendarModule, // Pour PrimeNG Calendar
    MatFormFieldModule,
    MatButtonModule,
    MatButtonModule,
    // RouterModule, assurez-vous que RouterModule est bien présent
    RouterModule
  ],
  providers: [
    provideHttpClient(withFetch())
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
