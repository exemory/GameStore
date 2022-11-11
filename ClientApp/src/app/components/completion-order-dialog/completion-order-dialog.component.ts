import {AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {AuthService} from "../../services/auth.service";
import {NotificationService} from "../../services/notification.service";
import {MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-complete-order-dialog',
  templateUrl: './completion-order-dialog.component.html',
  styleUrls: ['./completion-order-dialog.component.scss']
})
export class CompletionOrderDialogComponent implements OnInit, AfterViewInit {
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
              private dialogRef: MatDialogRef<CompletionOrderDialogComponent>) {
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

    this.ns.notifySuccess("Order confirmed");
    this.dialogRef.close(true);
  }
}
