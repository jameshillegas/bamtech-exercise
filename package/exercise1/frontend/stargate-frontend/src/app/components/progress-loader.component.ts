import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-progress-loader',
  standalone: true,
  templateUrl: './progress-loader.component.html',
  styleUrls: ['./progress-loader.component.css'],
  imports: [CommonModule]
})
export class ProgressLoaderComponent {
  @Input() visible = true;
  @Input() height = 3; // px
  @Input() color?: string;
}
