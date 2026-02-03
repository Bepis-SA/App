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

  loadFavorites() {
    this.favoriteService.getList().subscribe(data => {
      this.favorites = data;
    });
  }

  remove(id: string) {
    this.favoriteService.remove(id).subscribe(() => this.loadFavorites());
  }
}
