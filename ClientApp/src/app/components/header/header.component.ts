import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {MatDialog} from "@angular/material/dialog";
import {UploadAvatarDialogComponent} from "../upload-avatar-dialog/upload-avatar-dialog.component";
import {HttpClient} from "@angular/common/http";
import {DomSanitizer, SafeUrl} from "@angular/platform-browser";
import {CartService} from "../../services/cart.service";
import {map} from "rxjs";
import {UserRole} from "../../enums/user-role";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  cartItemsCount = this.cart.items.pipe(
    map(i => i.reduce((sum, item) => sum + item.quantity, 0))
  );

  avatarUrl?: SafeUrl | string = 'assets/default-user-avatar.png';
  showMobileMenu = false;

  constructor(public auth: AuthService,
              private dialog: MatDialog,
              private api: HttpClient,
              private sanitizer: DomSanitizer,
              public cart: CartService,
              private cdr: ChangeDetectorRef) {
  }

  ngOnInit(): void {
    document.addEventListener('click', this.onPageClick.bind(this));

    this.auth.isLoggedIn.subscribe({
      next: isLoggedIn => {
        if (isLoggedIn) {
          this.loadUserAvatar();
        }
      }
    });
  }

  private onPageClick(e: any) {
    const target = e.target as Element;

    if (!target.closest('.menu-toggle-btn') &&
      !target.closest('.cdk-overlay-backdrop') &&
      !target.classList.contains('mobile-menu') &&
      !target.classList.contains('menu-overlay')) {
      this.showMobileMenu = false;
      this.cdr.detectChanges();
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

  get isUserAdmin() {
    return this.auth.session?.userInfo.roles.includes(UserRole.Admin);
  }

  signIn() {
    this.auth.openSignInDialog();
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
