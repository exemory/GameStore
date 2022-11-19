import {UserRole} from "../enums/user-role";

export interface UserInfo {
  id: string,
  username: string,
  firstName: string,
  lastName: string,
  userRoles: UserRole[],
  hasAvatar: boolean
}
