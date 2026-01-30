/* import { Component, OnInit } from '@angular/core';
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
}*/


import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { RatingsComponent } from '../ratings/ratings';
import { TravelExperienceComponent } from '../experiences/travel-experience/travel-experience';
import { FavoriteService } from '@proxy/favorites';
import { DestinationDto } from '@proxy/application/contracts/destinations';

@Component({
  selector: 'app-city-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, RatingsComponent, TravelExperienceComponent],
  templateUrl: './city-detail.html',
  styleUrl: './city-detail.scss'
})
export class CityDetailComponent implements OnInit {
  private router = inject(Router);
  private favoriteService = inject(FavoriteService);
  private authService = inject(AuthService);
  private toaster = inject(ToasterService);
  private location = inject(Location); 

  city: any; 
  isFavorite: boolean = false;
  isLoading: boolean = false;
  isLoggedIn: boolean = false;
  
  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated;
    this.city = history.state.data;

    if (!this.city) {
      // this.router.navigate(['/search-city']);   no anda si apreta f5
      this.router.navigate(['/']);  // Por ahora lo dejo así, no soluciona pero no se rompe
      return;
    }

    if (this.isLoggedIn) {
      this.checkIfFavorite();
    }
  }

  checkIfFavorite() {
    this.favoriteService.getList().subscribe({
      next: (favorites) => {
        const found = favorites.find(f => 
          f.name?.toLowerCase() === this.city.name?.toLowerCase() && 
          f.country?.toLowerCase() === this.city.country?.toLowerCase()
        );

        if (found) {
          this.isFavorite = true;
          this.city.id = found.id; 
        } else {
          this.isFavorite = false;
        }
      }
    });
  }

  toggleFavorite() {
    if (!this.isLoggedIn) {
      this.toaster.info("Debes iniciar sesión para guardar favoritos", "Atención");
      this.authService.navigateToLogin();
      return;
    }

    this.isLoading = true;

    if (this.isFavorite) {
      if (this.city.id) {
        this.favoriteService.remove(this.city.id).subscribe({
          next: () => {
            this.isFavorite = false;
            this.toaster.info("Destino eliminado de favoritos");
            this.isLoading = false;
          },
          error: (err) => {
            this.toaster.error("No se pudo eliminar", "Error");
            this.isLoading = false;
          }
        });
      }
    } else {
      const input = {
        name: this.city.name,
        country: this.city.country,
        city: this.city.name,
        population: this.city.population || 0,
        photo: '', 
        coordinates: { 
            latitude: this.city.latitude || 0, 
            longitude: this.city.longitude || 0 
        },
        updateDate: new Date().toISOString()
      } as any; 

      this.favoriteService.add(input).subscribe({
        next: (result: DestinationDto) => {
          this.isFavorite = true;
          this.city.id = result.id; 
          this.toaster.success("¡Guardado en tu lista de viajes!", "Favorito Agregado");
          this.isLoading = false;
        },
        error: (err) => {
          console.error(err);
          this.toaster.error("Ocurrió un error al guardar", "Error");
          this.isLoading = false;
        }
      });
    }
  }
  goBack(): void {
    this.location.back();
  }
}

