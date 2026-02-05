import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { RatingsComponent } from '../../ratings/ratings.component';
import { TravelExperienceComponent } from '../../experiences/travel-experience/travel-experience.component';
import { FavoriteService } from '@proxy/favorites';
import { DestinationDto } from '@proxy/application/contracts/destinations';
import { ActivatedRoute } from '@angular/router';
import { DestinationService } from '@proxy/destinations';

@Component({
  selector: 'app-city-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, RatingsComponent, TravelExperienceComponent],
  templateUrl: './city-detail.component.html',
  styleUrl: './city-detail.component.scss'
})
export class CityDetailComponent implements OnInit {
  private router = inject(Router);
  private favoriteService = inject(FavoriteService);
  private authService = inject(AuthService);
  private toaster = inject(ToasterService);
  private location = inject(Location);
  private route = inject(ActivatedRoute);
  private destinationService = inject(DestinationService);

  city: any;
  isFavorite: boolean = false;
  isLoading: boolean = false;
  isLoggedIn: boolean = false;

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated;

    if (history.state && history.state.data) {
      this.city = history.state.data;
      console.log('Ciudad cargada desde el buscador/state');
      if (this.isLoggedIn) this.checkIfFavorite();
    }
    else {
      const id = this.route.snapshot.paramMap.get('id');
      if (id) {
        this.isLoading = true;
        this.destinationService.get(id).subscribe({
          next: (data) => {
            this.city = data;
            this.isLoading = false;
            if (this.isLoggedIn) this.checkIfFavorite();
          },
          error: (err) => {
            console.error('No se encontró la ciudad en la DB', err);
            this.router.navigate(['/cities']);
          }
        });
      } else {
        this.router.navigate(['/']);
      }
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
        this.favoriteService.delete(this.city.id).subscribe({
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
