import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatError, MatFormField, MatLabel, MatPrefix } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { BackgroundComponent } from '../background/background.component';
import { IRegistrationRequest } from '../authentication.models';
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: 'app-authentication-register',
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
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterComponent {
  private readonly _formBuilder = inject(FormBuilder);
  private readonly _authenticationService = inject(AuthenticationService);

  protected readonly isSubmitting = signal(false);
  protected readonly submissionError = signal<string | null>(null);
  protected readonly submissionSucceeded = signal(false);

  protected readonly registerForm = this._formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    username: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[^@]+$/)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  protected onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();

      return;
    }

    this.submissionError.set(null);
    this.submissionSucceeded.set(false);
    this.isSubmitting.set(true);

    const request: IRegistrationRequest = this.registerForm.getRawValue();
    this._authenticationService.register(request).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.submissionSucceeded.set(true);
      },
      error: () => {
        this.isSubmitting.set(false);
        this.submissionError.set('We could not create your account. Review the details and try again.');
      },
    });
  }
}
