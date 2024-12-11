import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

  constructor(private http: HttpClient) { }
  private apiUrl ="http://localhost:5031/api";

  getMovieById(id: number) : Observable<any>{
    return this.http.get(this.apiUrl + "/movies/" + id, {withCredentials: true});
  }
  getAllMovies() :Observable<any>{
    return this.http.get(this.apiUrl + "/tags/5/movies", {withCredentials:true});
  }
}
