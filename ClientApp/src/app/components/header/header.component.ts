import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {MatDialog} from "@angular/material/dialog";
import {UploadAvatarDialogComponent} from "../upload-avatar-dialog/upload-avatar-dialog.component";
import {HttpClient} from "@angular/common/http";
import {DomSanitizer, SafeUrl} from "@angular/platform-browser";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  avatarUrl?: SafeUrl | string = 'assets/default-user-avatar.png';

  constructor(public auth: AuthService,
              private dialog: MatDialog,
              private api: HttpClient,
              private sanitizer: DomSanitizer) {
  }

  ngOnInit(): void {
    if (this.auth.isLoggedIn) {
      this.loadUserAvatar();
    }
  }

  loadUserAvatar() {
    this.api.get<Blob>('avatar', {observe: 'body', responseType: 'blob' as 'json'})
      .subscribe({
        next: blob => {
          let objectURL = URL.createObjectURL(blob);
          this.avatarUrl = this.sanitizer.bypassSecurityTrustUrl(objectURL);
        },
        error: err => {
          this.avatarUrl = 'assets/default-user-avatar.png';
        }
      });
  }

  get fullUserName() {
    return `${this.auth.session?.userInfo.firstName} ${this.auth.session?.userInfo.lastName}`;
  }

  signIn() {
    this.auth.openSignInDialog()
      .afterClosed().subscribe({
      next: isAuthorized => {
        if (isAuthorized) {
          this.loadUserAvatar();
        }
      }
    })
  }

  signUp() {
    this.auth.openSignUpDialog();
  }

  signOut() {
    this.auth.signOut();
  }

  uploadAvatar() {
    const dialogRef = this.dialog.open(UploadAvatarDialogComponent,
      {
        maxWidth: '400px',
        width: '100%'
      });

    dialogRef.afterClosed().subscribe({
      next: avatarHasBeenUploaded => {
        if (avatarHasBeenUploaded) {
          this.loadUserAvatar();
        }
      }
    })
  }
}
