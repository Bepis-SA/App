import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
// Importá aquí tus componentes hijos para que Angular los reconozca
import { RatingsComponent } from '../ratings/ratings';
import { TravelExperienceComponent } from '../experiences/travel-experience/travel-experience';

@Component({
  selector: 'app-city-detail',
  standalone: true,
  imports: [CommonModule, RatingsComponent, TravelExperienceComponent],
  templateUrl: './city-detail.html',
  styleUrl: './city-detail.scss'
})
export class CityDetailComponent implements OnInit {
  city: any;

  constructor(private router: Router) { }

  ngOnInit(): void {
    this.city = history.state.data;
    if (!this.city) {
      this.router.navigate(['/search-city']);
    }
  }
}
