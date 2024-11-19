import { NgModule } from '@angular/core';
import { LoginComponent } from './Auth/login/login.component';
import { RegisterComponent } from './Auth/register/register.component';
import { FogetPasswordComponent } from './Auth/foget-password/foget-password.component';
import { HomeComponent } from './home/home.component';
import { authGuard } from './auth.guard';
import { ResetPasswordComponent } from './Auth/reset-password/reset-password.component';
import { ActivateAccountComponent } from './Auth/activate-account/activate-account.component';
import { AddTaskModalComponent } from './add-task-modal/add-task-modal.component';
import { ProfilComponent } from './profil/profil.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent }, 
  { path: 'forget-password', component: FogetPasswordComponent }, 
  { path: 'user-management', component: UserManagementComponent }, 
  { path: 'activateaccount', component: ActivateAccountComponent }, 
  { path: 'resetpassword', component: ResetPasswordComponent }, 
  { path: 'sidebar', component: ResetPasswordComponent, canActivate: [authGuard]  }, 
  { path: 'navbar', component: ResetPasswordComponent, canActivate: [authGuard]  }, 
  { path: 'add-task-modal', component: AddTaskModalComponent, canActivate: [authGuard]  }, 
  { path: 'profil', component: ProfilComponent, canActivate: [authGuard]  }, 
  { path: 'home', component: HomeComponent, canActivate: [authGuard] }, 
  { path: '', redirectTo: '/home', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
