import { Component, inject } from '@angular/core';
import { UserService } from '../../proxy/users';
import { PublicUserProfileDto } from '../../proxy/users'; 
import { ToasterService } from '@abp/ng.theme.shared';
import { finalize } from 'rxjs/operators';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-public-profile',
  imports: [CommonModule, FormsModule],
  templateUrl: './public-profile.component.html',
  styleUrl: './public-profile.component.scss',
  standalone: true,
})

export class PublicProfileComponent {

  searchUsername: string = '';
  loading = false;
  
  foundUser: PublicUserProfileDto | null = null;
  errorMessage: string = '';

  private readonly userService = inject(UserService);
  private readonly toaster = inject(ToasterService);

  searchUser() {
    if (!this.searchUsername.trim()) return;

    this.loading = true;
    this.foundUser = null;
    this.errorMessage = '';

    this.userService.getPublicProfile(this.searchUsername)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (result) => {
          this.foundUser = result;
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'No se pudo encontrar el perfil o es privado.';
          this.toaster.error(this.errorMessage, 'Error de Búsqueda');
        }
      });
}
}