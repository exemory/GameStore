import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  constructor(public auth: AuthService) {
  }

  ngOnInit(): void {
  }

  get fullUserName() {
    return `${this.auth.session?.userInfo.firstName} ${this.auth.session?.userInfo.lastName}`;
  }

  signIn() {
    this.auth.openSignInDialog();
  }

  signUp() {
    this.auth.openSignUpDialog();
  }

  get userAvatarUrl() {
    return this.auth.session?.userInfo.hasAvatar ? '/api/avatar' : 'assets/default-user-avatar.png';
  }

  signOut() {
    this.auth.signOut();
  }
}
