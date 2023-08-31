import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AccountService);
  const router = inject(Router);

  return authService.currentUser$.pipe(
    map((auth) => {
      if (auth) {
        return true;
      } else {
        router.navigate(['account/login'], {
          queryParams: { returnUrl: state.url },
        }); //query params will be used to send back user to prev screen after login(screen which he tries to access)
        return false;
      }
    })
  );
};
