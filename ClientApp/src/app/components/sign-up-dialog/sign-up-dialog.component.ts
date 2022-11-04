import {Component, OnInit} from '@angular/core';
import {
  FormBuilder,
  Validators
} from "@angular/forms";
import {ConfirmPasswordErrorStateMatcher} from "../../shared/error-state-matchers/confirm-password-error-state-matcher";
import {confirmPasswordValidator} from "../../shared/validators/confirm-password-validator";
import {SignUpData} from "../../interfaces/sign-up-data";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";
import {AuthService} from "../../services/auth.service";
import {MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-sign-up-dialog',
  templateUrl: './sign-up-dialog.component.html',
  styleUrls: ['./sign-up-dialog.component.scss']
})
export class SignUpDialogComponent implements OnInit {

  form = this.fb.group({
    username: ['', [
      Validators.required,
      Validators.minLength(3),
      Validators.maxLength(15),
      Validators.pattern(/^[a-z\d-._@+]*$/i)
    ]],
    email: ['', [
      Validators.required,
      Validators.email
    ]],
    firstName: ['', [
      Validators.required,
      Validators.maxLength(50)
    ]],
    lastName: ['', [
      Validators.required,
      Validators.maxLength(50)
    ]],
    password: ['', [
      Validators.required,
      Validators.minLength(8)
    ]],
    confirmPassword: [''],
  }, {validators: confirmPasswordValidator('password', 'confirmPassword')});

  confirmPasswordStateMatcher = new ConfirmPasswordErrorStateMatcher();

  hidePassword = true;
  hideConfirmPassword = true;

  inProgress = false;

  constructor(private fb: FormBuilder,
              private api: HttpClient,
              private ns: NotificationService,
              private auth: AuthService,
              private dialogRef: MatDialogRef<SignUpDialogComponent>) {
  }

  ngOnInit(): void {
  }

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    const data: SignUpData = {
      username: this.form.value.username!.trim(),
      email: this.form.value.email!,
      firstName: this.form.value.firstName!.trim(),
      lastName: this.form.value.lastName!.trim(),
      password: this.form.value.password!
    }

    this.inProgress = true;
    this.dialogRef.disableClose = true;

    this.api.post('auth/sign-up', data)
      .subscribe({
        next: () => {
          this.ns.notifySuccess('You are successfully registered');
          this.dialogRef.close();
          this.auth.openSignInDialog(data.username);
        },
        error: err => {
          this.ns.notifyError(`Registration failed. ${err.error?.message ?? ''}`);
          this.inProgress = false;
          this.dialogRef.disableClose = false;
        }
      });
  }

  signIn() {
    this.dialogRef.close();
    this.auth.openSignInDialog();
  }
}
