import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '../../environments/environment.development';
import { env } from 'node:process';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private authenticated = new BehaviorSubject<boolean>(false);
  private userRole = new BehaviorSubject<string | null>(null);
  private _userId = new BehaviorSubject<string>('');
  // private apiUrl = "http://localhost:5031/api";
  private apiUrl = environment.apiUrl;

  isAuthenticated$ = this.authenticated.asObservable();
  userRole$ = this.userRole.asObservable();
  userId$ = this._userId.asObservable();
  

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this.initializeAuthState();
  }

  private initializeAuthState(): void {
    const accessToken = this.getAccessToken();
    if (accessToken && !this.isTokenExpired(accessToken)) {
      this.authenticated.next(true);
      this._userId.next(this.getUserId());
      this.decodeAndSetRole(accessToken);
      this.startTokenRefreshInterval();
    } else {
      this.authenticated.next(false);
      this.userRole.next(null);
      this._userId.next('');
    }
  }

  login(username: string, password: string): Observable<any> {
    const loginData = { username, password };

    return this.http
      .post(`${this.apiUrl}/auth/login`, loginData, { withCredentials: true })
      .pipe(
        tap((response: any) => {
          if (response.accessToken) {
            this.setAccessToken(response.accessToken);
            this.authenticated.next(true);
            this._userId.next(this.getUserId())
            this.decodeAndSetRole(response.accessToken);
            this.startTokenRefreshInterval();
          }
        }),
       );
  }
  register(username: string, email: string, password: string): Observable<any>{
    const regData = { username, email, password };
    return this.http.post(`${this.apiUrl}/auth/register`, regData, {withCredentials:true })
    .pipe(
      tap((response:any)=>{
        if (response.accessToken) {
          this.setAccessToken(response.accessToken);
          this.authenticated.next(true);
          this._userId.next(this.getUserId())
          this.decodeAndSetRole(response.accessToken);
          this.startTokenRefreshInterval();
        }
      })
    )
  }

  private decodeAndSetRole(token: string): void {
    try {
      const decodedToken: any = jwtDecode(token);
      const role = decodedToken.role[0] || null;
      this.userRole.next(role);
    } catch (error) {
      console.error('Error decoding token:', error);
      this.logout();
    }
  }

  logout(): Observable<any> {
    this.clearAccessToken();
    this.authenticated.next(false);
    this.userRole.next(null);
    this._userId.next('');
    return this.http.post(`${this.apiUrl}/auth/logout`, {}, { withCredentials: true });
  }

  refreshAccessToken(): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/auth/refresh`,{}, { withCredentials: true })
      .pipe(
        tap((response: any) => {
          this.setAccessToken(response.accessToken);
          this.decodeAndSetRole(response.accessToken);
        }),
        catchError((error) => {
          console.error('Failed to refresh token:', error);
          this.logout();
          return throwError(error);
        })
      );
  }

  private startTokenRefreshInterval(): void {
    const refreshInterval = 2*60;
    setInterval(() => {
      const accessToken = this.getAccessToken();
      if (accessToken) {
        const now = Math.floor(Date.now() / 1000);
        const decoded: any = jwtDecode(accessToken);
        const timeLeft = decoded.exp - now;

        if (timeLeft < 2*60) {
          this.refreshAccessToken().subscribe();
        }
      }
    }, refreshInterval);
  }

  isTokenExpired(token: string): boolean {
    const now = Math.floor(Date.now() / 1000);
    const decoded: any = jwtDecode(token);
    return decoded.exp < now;
  }

  private getAccessToken(): string | null {
    return this.cookieService.get('accessToken') || null;
  }

  private setAccessToken(token: string): void {
    this.cookieService.set('accessToken', token, { path: '/', secure: true, sameSite: 'None' });
  }

  private clearAccessToken(): void {
    this.cookieService.delete('accessToken', '/');
  }
  public getBearer(): HttpHeaders{
    const token = this.cookieService.get('accessToken');
        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        });
      return headers;
  }
  public getUserId(): string{
    const token = this.cookieService.get('accessToken');
    const decoded: any = jwtDecode(token);

    return decoded.sub;

  }
}
