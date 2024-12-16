import { Component, Input, OnInit } from '@angular/core';
import { Movie, MovieService } from '../../services/movie.service';
import { FormsModule, NgModel } from '@angular/forms';
import { Router,  ActivatedRoute, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './main.component.html',
  styleUrl: './main.component.scss'
})

export class MainComponent implements OnInit {
  constructor(private movieServie: MovieService, private router: Router, private route: ActivatedRoute){}
  allMovies: Movie[] = [];
  imageUrl: string | undefined;
  id!: string;

  openMovie(id: number, title: string) {
    const encodedId = encodeURIComponent(id);
    const encodedTitle = encodeURIComponent(title);
    const url = `/movie/${encodedId}/${encodedTitle}`;

    this.router.navigate([url]);

  }

  ngOnInit(): void {
    this.movieServie.getAllMovies().subscribe(
      (response: Movie[]) =>{
       this.allMovies = response;
       console.log(this.allMovies);
      }
    )
  }
}
