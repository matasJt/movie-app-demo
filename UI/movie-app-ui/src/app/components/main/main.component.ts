import { Component, Input, OnInit } from '@angular/core';
import { MovieService } from '../../services/movie.service';
import { FormsModule } from '@angular/forms';
import { response } from 'express';
import { Router } from '@angular/router';

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './main.component.html',
  styleUrl: './main.component.scss'
})

export class MainComponent implements OnInit {
  constructor(private movieServie: MovieService, private router: Router){}
  allMovies: Movie[] = [];
  imageUrl: string | undefined;
  id!: number;
  emit(id: number) {
    console.log("id:",id);
  }
  loadImage(){
      this.movieServie.getMovieById(this.id).subscribe( 
        (response)=>{
          this.imageUrl = response.posterUrl;
      });
  }

  ngOnInit(): void {
    this.movieServie.getAllMovies().subscribe(
      (response: Movie[]) =>{
       this.allMovies = response;
      }
    )
  }
}
interface Movie{
  id: number,
  title: string,
  director:string,
  year:number,
  tags: number[],
  genre: string,
  posterUrl: string
};
