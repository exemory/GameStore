import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {AuthService} from "../../services/auth.service";
import {NotificationService} from "../../services/notification.service";
import {MatDialogRef} from "@angular/material/dialog";
import {PaymentType} from "../../enums/payment-type";
import {HttpClient} from "@angular/common/http";
import {OrderCreationData} from "../../interfaces/order-creation-data";
import {OrderItem} from "../../interfaces/order-item";
import {CartService} from "../../services/cart.service";

@Component({
  selector: 'app-complete-order-dialog',
  templateUrl: './completion-order-dialog.component.html',
  styleUrls: ['./completion-order-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CompletionOrderDialogComponent implements OnInit, AfterViewInit {
  public paymentType = PaymentType;

  @ViewChild('firstName') firstNameElementRef!: ElementRef;
  @ViewChild('email') emailElementRef!: ElementRef;

  form = this.fb.group({
    firstName: ['', [
      Validators.required,
      Validators.maxLength(50)
    ]],
    lastName: ['', [
      Validators.required,
      Validators.maxLength(50)
    ]],
    email: ['', [
      Validators.required,
      Validators.email
    ]],
    phone: ['', [
      Validators.required,
      Validators.pattern(/^\+\d{11,16}$/)
    ]],
    paymentType: ['', [
      Validators.required
    ]],
    comments: ['', Validators.maxLength(600)]
  });

  inProgress = false;

  constructor(private fb: FormBuilder,
              private auth: AuthService,
              private cdr: ChangeDetectorRef,
              private ns: NotificationService,
              private dialogRef: MatDialogRef<CompletionOrderDialogComponent>,
              private api: HttpClient,
              private cart: CartService) {
    this.initFields();
  }

  private initFields() {
    if (!this.auth.isLoggedIn.value) {
      return;
    }

    const userInfo = this.auth.session!.userInfo;

    const fieldValues = {
      firstName: userInfo.firstName,
      lastName: userInfo.lastName
    }

    this.form.patchValue(fieldValues);
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.auth.isLoggedIn.value ? this.emailElementRef.nativeElement.focus() : this.firstNameElementRef.nativeElement.focus();
    this.cdr.detectChanges();
  }

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    const formValue = this.form.value;

    const orderItems: OrderItem[] = this.cart.items.value.map(
      i => <OrderItem>{
        gameId: i.game.id,
        quantity: i.quantity
      });

    const data: OrderCreationData = {
      firstName: formValue.firstName!.trim(),
      lastName: formValue.lastName!.trim(),
      email: formValue.email!,
      phone: formValue.phone!,
      paymentType: formValue.paymentType!,
      comments: formValue.comments ? formValue.comments : undefined,
      items: orderItems
    };

    this.inProgress = true;
    this.dialogRef.disableClose = true;

    this.api.post('orders', data)
      .subscribe({
        next: () => {
          this.ns.notifySuccess("Order confirmed.");
          this.dialogRef.close(true);
        },
        error: err => {
          this.inProgress = false;
          this.dialogRef.disableClose = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }
}
