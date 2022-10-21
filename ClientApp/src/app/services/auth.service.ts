import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {tap} from "rxjs";
import {Session} from "../interfaces/session";
import {SignInData} from "../interfaces/sign-in-data";
import {JwtHelperService} from '@auth0/angular-jwt';
import {MatDialog} from "@angular/material/dialog";
import {SignInDialogComponent} from "../components/sign-in-dialog/sign-in-dialog.component";
import {SignUpDialogComponent} from "../components/sign-up-dialog/sign-up-dialog.component";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private jwtHelper = new JwtHelperService();
  private _session?: Session;

  constructor(private api: HttpClient,
              private dialog: MatDialog) {
    const session = this.storedSession;

    if (!session) {
      return;
    }

    if (this.jwtHelper.isTokenExpired(session.accessToken)) {
      this.removeSession();
      return;
    }

    this._session = session;
  }

  public signIn(login: string, password: string, remember: boolean) {
    return this.api.post<Session>('auth/sign-in', <SignInData>{login, password})
      .pipe(
        tap(session => {
          this._session = session;

          if (remember) {
            this.storeSession(session);
          }
        })
      );
  }

  private get storedSession(): Session | undefined {
    const storedSession = localStorage.getItem('session');

    if (!storedSession) {
      return undefined;
    }

    return JSON.parse(storedSession);
  }

  private storeSession(session: Session) {
    localStorage.setItem('session', JSON.stringify(session));
  }

  private removeSession() {
    localStorage.removeItem('session');
  }

  public signOut() {
    this._session = undefined;
    this.removeSession();
  }

  get isLoggedIn(): boolean {
    return !!this._session;
  }

  get session(): Session | undefined {
    return this._session;
  }

  public openSignInDialog(login?: string) {
    return this.dialog.open(SignInDialogComponent,
      {
        autoFocus: false,
        maxWidth: '400px',
        width: '100%',
        data: login
      });
  }

  openSignUpDialog() {
    return this.dialog.open(SignUpDialogComponent,
      {
        maxWidth: '400px',
        width: '100%'
      });
  }
}
