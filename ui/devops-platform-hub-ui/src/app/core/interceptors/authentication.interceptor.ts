import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthenticationService } from '../../features/authentication/services/authentication.service';

export const authenticationInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthenticationService).getAccessToken();
  if (!token) {
    return next(req);
  }

  if (req.url.startsWith('/api/')) {
    const newReq = req.clone({
      headers: req.headers.append('Authorization', 'Bearer ' + token),
    });

    return next(newReq);
  }

  return next(req);
};
