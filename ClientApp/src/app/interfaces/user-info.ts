import {UserRole} from "../enums/user-role";

export interface UserInfo {
  id: string,
  username: string,
  firstName: string,
  lastName: string,
  roles: UserRole[],
  hasAvatar: boolean
}
