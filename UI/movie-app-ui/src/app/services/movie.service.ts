import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { win32 } from 'node:path/win32';
import { get } from 'http';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

  constructor(private http: HttpClient,private route: ActivatedRoute) { }
  private apiUrl ="http://localhost:5031/api";

  getMovieById(id: string) : Observable<any>{
    return this.http.get(this.apiUrl + "/movies/" + id, {withCredentials: true});
  }
  getAllMovies() :Observable<any>{
    return this.http.get(this.apiUrl + "/tags/5/movies", {withCredentials:true});
  }
  createReview(id: string,content: string, rating: number, headers: any) :Observable<any>{
    return this.http.post(this.apiUrl + `/movies/${id}/reviews`,{content, rating}, {headers, withCredentials: true} );
  }
  getReviews(id: string, headers:any):Observable<any>{
    return this.http.get<Review[]>(this.apiUrl + `/movies/${id}/reviews`, { headers, withCredentials:true}).pipe(
      map(reviews =>
        reviews.map(review => ({
          ...review,
          createdAt: new Date(review.createdAt),
        }))
      )
    );
  }
  deleteReview(movieId:string, reviewId: number, headers:any): Observable<any>{
    return this.http.delete(this.apiUrl + `/movies/${movieId}/reviews/${reviewId}`, { headers, withCredentials:true});

  }
  updateReview(content: string, rating:number, movieId:string, reviewId:number, headers:any): Observable<any>{
    return this.http.put(this.apiUrl + `/movies/${movieId}/reviews/${reviewId}`, {content, rating},{headers, withCredentials:true});
  }
}

export interface Review{
  id: number,
  content: string,
  rating: number,
  createdAt: Date
  userName: string,
  userId: string
}
