import {Component, OnInit, ViewChild} from '@angular/core';
import {UserInfo} from "../../interfaces/user-info";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";
import {MatTableDataSource} from "@angular/material/table";
import {MatPaginator} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {UserRole} from "../../enums/user-role";
import {UserRolesUpdateData} from "../../interfaces/user-roles-update-data";

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {

  @ViewChild(MatPaginator) set paginator(paginator: MatPaginator) {
    if (paginator) {
      this.dataSource.paginator = paginator;
    }
  }

  @ViewChild(MatSort) set sort(sort: MatSort) {
    if (sort) {
      this.dataSource.sort = sort;
    }
  }

  loading = true;
  loadingError = false;

  displayedColumns: string[] = ['username', 'firstName', 'lastName', 'roles', 'options'];
  dataSource = new MatTableDataSource<UserInfo>();

  constructor(private api: HttpClient,
              private ns: NotificationService) {
  }

  ngOnInit(): void {
    this.api.get<UserInfo[]>('users')
      .subscribe({
        next: users => {
          this.dataSource.data = users;
          this.loading = false;
        },
        error: err => {
          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
          this.loading = false;
          this.loadingError = true;
        }
      });
  }

  getUserRoles(user: UserInfo) {
    return user.roles.join(', ');
  }

  isUserHasManagerRole(user: UserInfo) {
    return user.roles.includes(UserRole.Manager);
  }

  addManagerRoleToUser(user: UserInfo) {
    const data: UserRolesUpdateData = {
      manager: true
    };

    this.api.put(`users/${user.id}/roles`, data)
      .subscribe({
        next: () => {
          if (!user.roles.includes(UserRole.Manager)) {
            user.roles.push(UserRole.Manager);
          }
        },
        error: err => {
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  removeManagerRoleFromUser(user: UserInfo) {
    const data: UserRolesUpdateData = {
      manager: false
    };

    this.api.put(`users/${user.id}/roles`, data)
      .subscribe({
        next: () => {
          const index = user.roles.indexOf(UserRole.Manager);
          if (index !== -1) {
            user.roles.splice(index, 1);
          }
        },
        error: err => {
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }
}
