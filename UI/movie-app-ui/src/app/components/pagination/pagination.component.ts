import { Component, EventEmitter, Input, NgModule, Output } from '@angular/core';
import { Movie, MovieService } from '../../services/movie.service';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [NgFor],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})
export class PaginationComponent {
  allMovies: Movie[] = [];
  constructor(private movieService: MovieService){
   this.currentPage = 1;
   this.itemsPerPage = 15;
   this.totalItems = 0;
   this.movieService.getAllMovies().subscribe(
    (response: Movie[]) =>{
      this.allMovies = response;
    }
   )
  }
  @Input() currentPage: number;
  @Input() itemsPerPage: number;
  @Input() totalItems: number;
  @Output() pageChanged: EventEmitter<number> = new EventEmitter();

  get totalPages():number{
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }
  changePage(page:number){
      if(page >=1 && page && page <= this.totalPages){
        this.currentPage = page;
        this.pageChanged.emit(page);
      }
     
      
  }
}
