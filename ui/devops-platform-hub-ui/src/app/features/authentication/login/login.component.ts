import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatError, MatFormField, MatLabel, MatPrefix } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { BackgroundComponent } from '../background/background.component';
import { ILoginRequest } from '../authentication.models';
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: 'app-authentication-login',
  imports: [
    ReactiveFormsModule,
    MatButton,
    MatCard,
    MatError,
    MatFormField,
    MatInput,
    MatIcon,
    MatLabel,
    MatPrefix,
    RouterLink,
    BackgroundComponent,
  ],
  styleUrl: './login.component.scss',
  templateUrl: './login.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  private readonly _formBuilder = inject(FormBuilder);
  private readonly _authenticationService = inject(AuthenticationService);

  protected readonly isSubmitting = signal(false);
  protected readonly submissionError = signal<string | null>(null);
  protected readonly submissionSucceeded = signal(false);
  protected readonly currentUser = this._authenticationService.currentUser;

  protected readonly loginForm = this._formBuilder.nonNullable.group({
    identifier: ['', [Validators.required]],
    password: ['', [Validators.required]],
  });

  protected onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.submissionError.set(null);
    this.submissionSucceeded.set(false);
    this.isSubmitting.set(true);

    const request: ILoginRequest = this.loginForm.getRawValue();
    this._authenticationService.login(request).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.submissionSucceeded.set(true);
      },
      error: () => {
        this.isSubmitting.set(false);
        this.submissionError.set('We could not sign you in. Check your credentials and try again.');
      },
    });
  }
}
