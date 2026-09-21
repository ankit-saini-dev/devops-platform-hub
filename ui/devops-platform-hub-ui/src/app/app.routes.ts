import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/authentication/authentication.routes').then((route) => route.authenticationRoutes),
  },
];
