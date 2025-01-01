import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ScrollService {
  private scrollPosition : [number, number] = [0,0];
  constructor() { }

  setScrollPosition(position: [number, number]){
    this.scrollPosition = position;
  }
  getScrollPosition(): [number,number]{
    return this.scrollPosition;
  }
}
