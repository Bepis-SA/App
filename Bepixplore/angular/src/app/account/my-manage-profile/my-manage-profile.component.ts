import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { RestService, AuthService } from '@abp/ng.core';
import { UserService } from '../../proxy/users';
import { UserProfileDto, NotificationChannel, NotificationFrequency } from '../../account/models/user-profile.dto';

@Component({
  selector: 'app-my-manage-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './my-manage-profile.component.html',
  styleUrls: ['./my-manage-profile.component.scss']
})
export class MyManageProfileComponent implements OnInit {
  selectedTab = 0;
  personalForm: FormGroup;
  passwordForm: FormGroup;
  loading = false;

  eNotificationChannel = NotificationChannel;
  eNotificationFrequency = NotificationFrequency;
  
  private fb = inject(FormBuilder);
  private rest = inject(RestService);
  private toaster = inject(ToasterService);
  private authService = inject(AuthService);
  private userService = inject(UserService);

  constructor() {
    this.buildForms();
  }

  ngOnInit() {
    this.loadProfile();
  }

  buildForms() {
    this.personalForm = this.fb.group({
      userName: [{ value: '', disabled: true }],
      email: ['', [Validators.required, Validators.email]],
      name: ['', Validators.required],
      surname: ['', Validators.required],
      phoneNumber: [''],
      profilePictureUrl: [''], 
      notificationChannel: [null], 
      notificationFrequency: [null],
      concurrencyStamp: [null]
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', Validators.required]
    });
  }

 loadProfile() {
    this.rest.request<void, UserProfileDto>({
      method: 'GET',
      url: '/api/account/my-profile'
    }).subscribe({
      next: (user) => {
        this.personalForm.patchValue(user);
        if (user.extraProperties) {
          this.personalForm.patchValue({
            profilePictureUrl: user.extraProperties.ProfilePictureUrl || '',
            notificationChannel: user.extraProperties.NotificationChannel,
            notificationFrequency: user.extraProperties.NotificationFrequency
          });
        }
      },
      error: () => this.toaster.error('Error al cargar perfil')
    });
  }

 savePersonalData() {
    if (this.personalForm.invalid) return;
    this.loading = true;
    const formValues = this.personalForm.getRawValue();
    const body = {
      userName: formValues.userName,
      email: formValues.email,
      name: formValues.name,
      surname: formValues.surname,
      phoneNumber: formValues.phoneNumber,
      concurrencyStamp: formValues.concurrencyStamp,
      extraProperties: {
        ProfilePictureUrl: formValues.profilePictureUrl,
        NotificationChannel: Number(formValues.notificationChannel),
        NotificationFrequency: Number(formValues.notificationFrequency)
      }
    };
  this.rest.request<any, UserProfileDto>({
      method: 'PUT',
      url: '/api/account/my-profile',
      body: body
    }).subscribe({
      next: (res) => {
        this.toaster.success('Datos actualizados correctamente');
        this.personalForm.patchValue(res);
        if (res.extraProperties) {
             this.personalForm.patchValue({
                profilePictureUrl: res.extraProperties.ProfilePictureUrl,
                notificationChannel: res.extraProperties.NotificationChannel,
                notificationFrequency: res.extraProperties.NotificationFrequency
             });
        }
        this.loading = false;
      },
      error: (err) => {
        this.toaster.error(err.error?.message || 'Error al guardar');
        this.loading = false;
      }
    });
  }

  changePassword() {
    if (this.passwordForm.invalid) return;

    const { currentPassword, newPassword, confirmNewPassword } = this.passwordForm.value;

    if (newPassword !== confirmNewPassword) {
      this.toaster.error('Las nuevas contraseñas no coinciden', 'Error de validación');
      return;
    }

    this.loading = true;

    this.rest.request({
      method: 'POST',
      url: '/api/account/my-profile/change-password',
      body: { currentPassword, newPassword }
    }).subscribe({
      next: () => {
        this.toaster.success('Contraseña cambiada exitosamente');
        this.passwordForm.reset();
        this.loading = false;
      },
      error: (err) => {
        this.toaster.error(err.error?.message || 'La contraseña actual es incorrecta o hubo un error.');
        this.loading = false;
      }
    });
  }

  deleteAccount() {
    if (confirm('¿ESTÁS TOTALMENTE SEGURO? Esta acción NO se puede deshacer.')) {
      this.userService.deleteMyAccount().subscribe({
        next: () => {
          this.toaster.success('Cuenta eliminada correctamente.', 'Hasta luego');
          this.authService.logout().subscribe(() => {
             window.location.href = '/';
          });
        },
        error: (err) => {
          this.toaster.error(err.error?.message || 'Error al eliminar.', 'Error');
        }
      });
    }
  }

  setTab(index: number) {
    this.selectedTab = index;
  }
}