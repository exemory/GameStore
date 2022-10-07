import {AfterViewInit, ChangeDetectorRef, Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {FormGroup} from "@angular/forms";

@Component({
  selector: 'app-currency-input',
  templateUrl: './currency-input.component.html',
  styleUrls: ['./currency-input.component.scss']
})
export class CurrencyInputComponent implements OnInit, AfterViewInit {

  @Input() form!: FormGroup;
  @Input() controlName!: string;
  @Input() label!: string;
  @Input() placeholder?: string;
  @Input() outOfRangeError!: string;

  @ViewChild('input') input?: ElementRef<HTMLInputElement>;

  inputInFocus = false;

  get showCurrencySymbol() {
    return this.inputInFocus || this.input?.nativeElement.value;
  }

  constructor(private cdr: ChangeDetectorRef) {
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.cdr.detectChanges();
  }

  onInput(event: any) {
    const inputString = event.target.value as string;

    let patchedString = inputString.replace(/[^0-9.]/g, '');

    let firstDotIndex = patchedString.indexOf('.');
    if (firstDotIndex !== -1) {
      patchedString = patchedString.slice(0, firstDotIndex + 1) + patchedString.slice(firstDotIndex + 1).replace(/\./g, '').slice(0, 2);
    }

    this.form.get(this.controlName)?.setValue(patchedString);
  }
}
