import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { map } from 'rxjs/operators';
import { FavoriteService } from '../proxy/favorites/favorite.service';
import { DestinationDto } from '../proxy/application/contracts/destinations/models';

@Component({
  selector: 'app-destinations',
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule],
  providers: [ListService],
  templateUrl: './destinations.html',
  styleUrl: './destinations.scss'
})
export class Destinations implements OnInit {
  public readonly list = inject(ListService);
  private readonly favoriteService = inject(FavoriteService);

  items: DestinationDto[] = [];
  totalCount = 0;

  ngOnInit() {
    const favoriteStreamCreator = (query) =>
      this.favoriteService.getList().pipe(
        map((response) => ({
          items: response,
          totalCount: response.length
        }))
      );

    this.list.hookToQuery(favoriteStreamCreator).subscribe((response) => {
      this.items = response.items;
      this.totalCount = response.totalCount;
    });
  }

  loadFavorites() {
    this.favoriteService.getList().subscribe((response) => {
      this.items = response;
      this.totalCount = response.length;
    });
  }

  delete(id: string) {
    if (confirm('¿Seguro que querés quitar este destino de tus favoritos?')) {
      this.favoriteService.remove(id).subscribe(() => {
        this.loadFavorites();
      });
    }
  }
}
