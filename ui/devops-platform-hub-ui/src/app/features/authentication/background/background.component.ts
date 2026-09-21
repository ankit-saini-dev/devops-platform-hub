import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-authentication-background',
  imports: [],
  styleUrl: './background.component.scss',
  templateUrl: './background.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BackgroundComponent {}
