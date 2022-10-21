import {AfterViewInit, ChangeDetectorRef, Component, ElementRef, Inject, OnInit, ViewChild} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {FormBuilder} from "@angular/forms";
import {NotificationService} from "../../services/notification.service";
import {Router} from "@angular/router";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-sign-in-dialog',
  templateUrl: './sign-in-dialog.component.html',
  styleUrls: ['./sign-in-dialog.component.scss']
})
export class SignInDialogComponent implements OnInit, AfterViewInit {

  @ViewChild("login") loginField!: ElementRef;
  @ViewChild("password") passwordField!: ElementRef;

  form = this.fb.group({
    login: [this.username ?? ''],
    password: [''],
    remember: [true]
  });

  hidePassword = true;
  inProgress = false;

  constructor(private auth: AuthService,
              private fb: FormBuilder,
              private ns: NotificationService,
              private router: Router,
              private cdr: ChangeDetectorRef,
              private dialogRef: MatDialogRef<SignInDialogComponent>,
              @Inject(MAT_DIALOG_DATA) public username?: string) {
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.username ? this.passwordField.nativeElement.focus() : this.loginField.nativeElement.focus()
    this.cdr.detectChanges();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    this.inProgress = true;
    this.dialogRef.disableClose = true;

    const formValue = this.form.value;

    this.auth.signIn(formValue.login!, formValue.password!, formValue.remember!)
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: err => {
          this.inProgress = false;
          this.dialogRef.disableClose = false;
          this.ns.notifyError(`Authentication failed. ${err.error?.message ?? ''}`);
        }
      });
  }
}
