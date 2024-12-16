import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Movie, MovieService, Review, Tag } from '../../services/movie.service';
import { AuthService } from '../../services/auth.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl, AbstractControl, NgModel, FormsModule } from '@angular/forms';
import { CommonModule, ViewportScroller } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { response } from 'express';

@Component({
  selector: 'app-movie',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule,FormsModule],
  templateUrl: './movie.component.html',
  styleUrl: './movie.component.scss'
})
export class MovieComponent implements OnInit {
  editingReviewId: number | null = null;
  canEdit = false;
  editedContent?: string = '';
  editingForm: FormGroup = this.formBulder.group({
    content: ['', [Validators.required, Validators.minLength(10)]],
  });
cancelEdit() {
  this.editingReviewId = null;
  this.editingForm.reset();
}
saveComment() {
  var header = this.auth.getBearer();
  const { content } = this.editingForm.value;
  this.movieService.updateReview(content, 10, this.movieId, this.editingReviewId!,header).subscribe(
    (response)=>{
      const index = this.allReviews.findIndex((r) => r.id === this.editingReviewId);
      if (index !== -1) {
        this.allReviews[index] = { ...this.allReviews[index], content };
      }
      this.cancelEdit();
    }
  );

}

editComment(reviewId: number) {
  if (this.editingReviewId === reviewId) return;

  
  this.cancelEdit();
  const review = this.allReviews.find(r=> r.id === reviewId);
  this.editingReviewId = reviewId;
  this.editingForm.patchValue({
   
    content: review?.content
  });
  
}
deleteComment(reviewId: number) {
  var header = this.auth.getBearer();
  this.movieService.deleteReview(this.movieId,reviewId,header).subscribe(
    (response)=>{
      this.allReviews = this.allReviews.filter(review => review.id !== reviewId);
    }
  )
}
  movieId!: string;
  reviews: FormGroup = new FormGroup({
    rating: new FormControl(''),
    content: new FormControl('')
  });
  isAuthenticated = false;
  canDelete= false;
  submitted= false;
  rating = 0;
  userId = '';
  
  constructor(private route: ActivatedRoute, 
              private movieService: MovieService,
              private auth: AuthService,
              private router: Router,
              private scroller: ViewportScroller,
              private formBulder: FormBuilder,
              private cookieService: CookieService,
              private cdr: ChangeDetectorRef
  ){
    this.auth.isAuthenticated$.subscribe( (status) =>{
      this.isAuthenticated = status;
    });
    this.auth.userId$.subscribe( (user)=>{
      this.userId = user;
  });
  }
  movie: Movie | undefined;
  tag: Omit<Tag,'id'> = {
    title: '',
    description: '',
    userId: ' '
  }
  allReviews: Review[] = [];
  allTags: Tag[] = [];
  ngOnInit(): void {
    this.reviews = this.formBulder.group({
      content:[
        '',
        [
          Validators.required,
          Validators.minLength(10)
        ]
      ]
    });
    this.scroller.scrollToPosition([0,0]);
   

    this.route.paramMap.subscribe( (params) =>{
      this.movieId = params.get('id')!;
      this.getMovie();
    });
    this.fetch();

  }
  fetch(){
    var header = this.auth.getBearer();
    this.movieService.getReviews(this.movieId, header).subscribe(
      (response: Review[]) =>{
       this.allReviews = response;
      }
    );
  }
  
  onSubmit(){
    this.submitted = true;
    if(this.reviews.valid){
      var {content} = this.reviews.value;
      if(this.rating < 1){
        this.rating = 0;
      }
      var header = this.auth.getBearer();
      this.movieService.createReview(this.movieId, content, this.rating, header).subscribe(
        (response: Review)=>{
          this.allReviews.unshift(response);
          this.rating=0;
          this.submitted = false;
          this.reviews.reset();
        },
      (error)=>{
        console.log(error);
      })
    }
  }

  get form():{ [key:string]: AbstractControl}{
    return this.reviews.controls;
  }
  get form1():{ [key:string]: AbstractControl}{
    return this.editingForm.controls;
  }

  getMovie(){
    this.movieService.getMovieById(this.movieId).subscribe( (response:Movie) =>{
        this.movie = response;
        this.movie.tags = response.tags;
        this.movie.tagsIds = response.tagsIds;
    });
  }
  setRating(rate: number) {
      this.rating = rate;
  }

  addTag() {
    const header = this.auth.getBearer();
    this.movieService.addTag(this.tag.title, header).subscribe(
      (newTag: Tag) => {
        this.allTags.push(newTag);
  
        this.movieService.addTagToMovie(this.allTags, this.movieId, this.movie!, header).subscribe(
          () => {
            if (this.movie) {
              this.movie.tags = [...this.movie.tags, newTag.title]; // Add new tag to tags array
              console.log(this.movie.tags)
              this.movie.tagsIds = [...this.movie.tagsIds, newTag.id]; // Update tagsIds
            }
            this.tag.title = ''; // Clear the input
            this.cdr.detectChanges(); // Trigger UI update
          }
        );
      }
    );
  }

  addToMovie(){
    var header = this.auth.getBearer();
   
  }


}
