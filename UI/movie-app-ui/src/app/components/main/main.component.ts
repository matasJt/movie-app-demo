import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { Movie, MovieService } from '../../services/movie.service';
import { FormsModule, NgModel } from '@angular/forms';
import { Router,  ActivatedRoute, RouterLink, NavigationEnd } from '@angular/router';
import { NgIf } from '@angular/common';
import { ScrollService } from '../../services/scroll.service';
import { ViewportScroller } from '@angular/common';
import { Location } from '@angular/common';
import { PaginationComponent } from '../pagination/pagination.component';
import { tick } from '@angular/core/testing';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './main.component.html',
  styleUrl: './main.component.scss'
})

export class MainComponent implements OnInit {
  constructor(private movieServie: MovieService, private router: Router, private route: ActivatedRoute, private scroll: ScrollService,
    private scroller: ViewportScroller, private location: Location
  ){}
  allMovies: Movie[] = [];
  currentPage: number = 1;
  itemsPerPage: number = 18;
  totalItems!: number;
  filtered: Movie[] = [];
  id!: string;
  searched:  boolean = false;
  filter!:string;

  openMovie(id: number, title: string) {
    const encodedId = encodeURIComponent(id);
    const encodedTitle = encodeURIComponent(title);
    const url = `/movie/${encodedId}/${encodedTitle}`;
    this.scroll.setScrollPosition(this.scroller.getScrollPosition())
    this.router.navigate([url]);
  }

  ngOnInit(): void {
    this.searched = false;
    this.fetchData();
  }
  fetchData(){
    this.movieServie.getAllMovies().subscribe(
      (response: Movie[]) =>{
       this.allMovies = response;
       this.filtered = response;
       this.totalItems = this.allMovies.length;
       this.searched = false;
       setTimeout( ()=>{
        const position = this.scroll.getScrollPosition()
        this.scroller.scrollToPosition(position);
      });
       this.route.queryParams.subscribe(
        (params) =>{
          const searchTitle = params['search'] || '';
          this.filter = searchTitle;
            this.filterData(searchTitle);
            this.scroller.scrollToPosition([0,0]);
        }
      )
      }
    ) 
  }
  filterData(filterParam: string){
      if(!filterParam){
        this.filtered = [...this.allMovies];
      }
      else{
        this.filtered = this.allMovies.filter(
          (item) => item.title.toLowerCase().includes(filterParam.toLowerCase())
        );
        this.searched = true;
      }
      this.totalItems = this.filtered.length;
  }
}
