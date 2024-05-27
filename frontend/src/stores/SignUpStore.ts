import { ref } from 'vue'
import { defineStore } from 'pinia'

export const SignUpStore = defineStore('SignUp', () => {
  const IsActiveSignUp = ref(false)
  const IsLogin = ref(localStorage.getItem('token') ? true : false)

  return { IsActiveSignUp, IsLogin,  }
})

export interface ProfilePicture {
  "pictureId": number,
  "picture": string
}

export interface Interests {
  interestId: number,
  name: string,
  value: string
}

export interface Profile {
  "profileId": number,
  "userName": string,
  "firstName": string,
  "lastName": string,
  "gender": string | null,
  "sexualPreferences": string | null,
  "biography": string | null,
  "fameRating": number,
  "age": number,
  "latitude": number,
  "longitude": number
  "profilePicture": ProfilePicture,
  "pictures": Array<ProfilePicture>,
  "interests": Array<string>,
  "hasLike"?: boolean
}

export interface GetProfileParams {
	UserId?: number,
  SexualPreferences?: string,
  MaxDistance?: number,
  MinFameRating?: number,
  MaxFameRating?: number,
  MaxAge?: number,
  MinAge?: number,
  IsLikedUser?: boolean,
  CommonTags?: Array<string>,
  IsMatched?: boolean,
  SortLocation?: string,
  SortFameRating?: string,
  SortAge?: string,
  SortCommonTags?: string,
  SortingMainParameter?: string,
  PageNumber?: number,
  PageSize?: number,
  Total?: number
}
