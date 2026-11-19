import { HttpInterceptorFn } from '@angular/common/http';
import { API_CONFIG, STORAGE_KEYS } from '../constants/api';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem(STORAGE_KEYS.TOKEN);
  const isApiRequest = req.url.startsWith(API_CONFIG.API_URL);

  if (!token || !isApiRequest) {
    return next(req);
  }

  const authReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(authReq);
};