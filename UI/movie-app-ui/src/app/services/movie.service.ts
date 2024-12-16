import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { win32 } from 'node:path/win32';
import { get } from 'http';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

  constructor(private http: HttpClient,private route: ActivatedRoute) { }
  // private apiUrl ="http://localhost:5031/api";
  private apiUrl = environment.apiUrl;

  getMovieById(id: string) : Observable<any>{
    return this.http.get(this.apiUrl + "/movies/" + id, {withCredentials: true});
  }
  getAllMovies() :Observable<any>{
    return this.http.get(this.apiUrl + "/movies", {withCredentials:true});
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
  addTagToMovie(tags: Tag[], movieId:string, movie:Movie, headers:any):Observable<any>{
    const existingTagIds = movie.tagsIds || [];
    const newTagIds = tags.map(tag => tag.id);
    const updatedTagIds = [...new Set([...existingTagIds, ...newTagIds])];
    const body= {...movie, tags: updatedTagIds};
      console.log(body);
      return this.http.put<Movie>(this.apiUrl +`/movies/${movieId}`, body, {headers, withCredentials:true} )
  }
  addTag(title:string,headers:any):Observable<any>{
    const description = "tt"; 
    return this.http.post(this.apiUrl + `/tags`,{title, description}, {headers, withCredentials:true});
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
export interface Tag{
  id: number,
  title: string,
  description: string,
  userId: string
}
export interface Movie{
  id: number,
  title: string,
  director:string,
  year:number,
  genre: string,
  tags: string[],
  tagsIds: number[],
  posterUrl: string
};
