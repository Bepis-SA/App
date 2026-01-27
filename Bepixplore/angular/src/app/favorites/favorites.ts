import { Component, OnInit } from '@angular/core';
import { FavoriteService } from '@proxy/favorites';
import { DestinationDto } from '@proxy/application/contracts/destinations/models';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
})
export class FavoritesComponent implements OnInit {
  favorites: DestinationDto[] = [];

  constructor(private favoriteService: FavoriteService) { }

  ngOnInit() {
    this.loadFavorites();
  }

  // 6.3: Consultar lista personal desde el backend
  loadFavorites() {
    this.favoriteService.getList().subscribe(data => {
      this.favorites = data;
    });
  }

  // 6.2: Eliminar de favoritos y refrescar
  remove(id: string) {
    this.favoriteService.remove(id).subscribe(() => this.loadFavorites());
  }
}
