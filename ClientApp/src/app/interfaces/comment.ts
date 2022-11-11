import {CommentUserInfo} from "./comment-user-info";

export interface Comment {
  id: string,
  body: string,
  creationDate: string,
  parentId?: string,
  userInfo: CommentUserInfo
}
