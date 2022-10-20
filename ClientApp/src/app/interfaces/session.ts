import {UserInfo} from "./user-info";

export interface Session {
  userInfo: UserInfo
  accessToken: string
}
