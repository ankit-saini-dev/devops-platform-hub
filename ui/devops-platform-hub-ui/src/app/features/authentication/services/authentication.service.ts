import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  IAuthenticationResponse,
  IAuthenticationSession,
  ICurrentUser,
  ILoginRequest,
  IRegistrationRequest,
} from '../authentication.models';
import { catchError, map, Observable, switchMap, tap, throwError } from 'rxjs';
import { SessionStorageService } from '../../../core/services/session-storage.service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private readonly _httpClient = inject(HttpClient);
  private readonly _sessionStorageService = inject(SessionStorageService);

  private readonly accessToken = signal<string | null>(null);
  public readonly currentUser = signal<ICurrentUser | null>(null);

  private readonly _authenticationRoute = '/api/authentication/';
  private readonly _authenticationSessionStorageKey = 'devops-platform.authentication-session';

  constructor() {
    this.restoreSession();
  }

  public login(request: ILoginRequest): Observable<IAuthenticationSession> {
    return this.completeAuthentication(this._httpClient.post<IAuthenticationResponse>(this._authenticationRoute + 'login', request));
  }

  public register(request: IRegistrationRequest): Observable<IAuthenticationSession> {
    return this.completeAuthentication(this._httpClient.post<IAuthenticationResponse>(this._authenticationRoute + 'register', request));
  }

  public getCurrentUser(): Observable<ICurrentUser> {
    return this._httpClient.get<ICurrentUser>(this._authenticationRoute + 'currentUser');
  }

  public getAccessToken(): string | null {
    return this.accessToken();
  }

  public clearSession(): void {
    this.accessToken.set(null);
    this.currentUser.set(null);
    this._sessionStorageService.removeItem(this._authenticationSessionStorageKey);
  }

  private completeAuthentication(request: Observable<IAuthenticationResponse>): Observable<IAuthenticationSession> {
    return request.pipe(
      tap((response) => this.accessToken.set(response.accessToken)),
      switchMap((response) => {
        return this.getCurrentUser().pipe(
          map((currentUser) => ({
            accessToken: response.accessToken,
            expiresAt: response.expiresAt,
            currentUser,
          })),
        );
      }),
      tap((session) => {
        this.currentUser.set(session.currentUser);
        this._sessionStorageService.setItem(this._authenticationSessionStorageKey, session);
      }),
      catchError((error: unknown) => {
        this.clearSession();

        return throwError(() => error);
      }),
    );
  }

  private restoreSession(): void {
    const storedSession = this._sessionStorageService.getItem(this._authenticationSessionStorageKey);
    if (storedSession == null) {
      return;
    }

    try {
      const session = storedSession;
      if (!this.isUsableSession(session)) {
        this.clearSession();
        return;
      }

      this.accessToken.set(session.accessToken);
      this.currentUser.set(session.currentUser);
    } catch {
      this.clearSession();
    }
  }

  private isUsableSession(session: unknown): session is IAuthenticationSession {
    if (typeof session !== 'object' || session == null) {
      return false;
    }

    const candidate = session as Partial<IAuthenticationSession>;
    const expiresAt = Date.parse(candidate.expiresAt ?? '');
    return (
      typeof candidate.accessToken === 'string' &&
      candidate.accessToken.length > 0 &&
      Number.isFinite(expiresAt) &&
      expiresAt > Date.now() &&
      typeof candidate.currentUser?.id === 'string' &&
      typeof candidate.currentUser.name === 'string' &&
      typeof candidate.currentUser.username === 'string' &&
      Array.isArray(candidate.currentUser.roles)
    );
  }
}
