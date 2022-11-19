import {Injectable} from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor, HttpStatusCode
} from '@angular/common/http';
import {environment as env} from "../../environments/environment";
import {catchError, EMPTY, Observable, throwError} from "rxjs";
import {AuthService} from "../services/auth.service";
import {NotificationService} from "../services/notification.service";

@Injectable()
export class ApiInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService,
              private ns: NotificationService) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    request = request.clone({url: `${env.apiUrl}${request.url}`});

    if (this.auth.isLoggedIn.value) {
      const token = this.auth.session?.accessToken!;

      request = request.clone({
        headers: request.headers.set('Authorization', `Bearer ${token}`)
      });
    }

    return next.handle(request).pipe(
      catchError(err => {
        if (err.status === HttpStatusCode.Unauthorized && this.auth.isLoggedIn.value) {
          this.auth.signOut();
          this.ns.notifyError("Session expired, please sign in again");
          return EMPTY;
        }

        return throwError(err);
      })
    );
  }
}
