import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './Auth/login/login.component';
import { RegisterComponent } from './Auth/register/register.component';
import { FogetPasswordComponent } from './Auth/foget-password/foget-password.component';
import { HomeComponent } from './home/home.component';
import { authGuard } from './auth.guard';
import { ResetPasswordComponent } from './Auth/reset-password/reset-password.component';
import { ActivateAccountComponent } from './Auth/activate-account/activate-account.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent }, 
  { path: 'forget-password', component: FogetPasswordComponent }, 
  { path: 'activateaccount', component: ActivateAccountComponent }, 
  { path: 'resetpassword', component: ResetPasswordComponent }, 
  { path: 'sidebar', component: ResetPasswordComponent }, 
  { path: 'navbar', component: ResetPasswordComponent }, 
  { path: 'home', component: HomeComponent, canActivate: [authGuard] }, 
  { path: '', redirectTo: '/home', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
