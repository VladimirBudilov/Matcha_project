import { reactive, ref } from 'vue'
import { defineStore } from 'pinia'
import axios from 'axios'

export const SignUpStore = defineStore('SignUp', () => {
  const IsActiveSignUp = ref(false)
  const IsLogin = ref(localStorage.getItem('token') ? true : false)
  const profiles = ref<Profile[]>([])
  const getFilters = ref<GetFiltersType>({
    maxAge: 18,
    maxDistance: 0,
    maxFameRating: 0,
    minAge: 18,
    minDistance: 0,
    minFameRating: 0
  })

  const getProfileParams = ref<GetProfileParams>({
    PageNumber: 1,
    PageSize: 10,
    Total: 0,
    SexualPreferences: '',
    MaxDistance: 0,
    MinFameRating: 0,
    MaxFameRating: 0,
    MinAge: 0,
    MaxAge: 0,
    IsLikedUser: false,
    CommonTags: [],
    IsMatched: false,
    SortLocation: 'ASC',
    SortFameRating: 'ASC',
    SortAge: 'ASC',
    SortCommonTags: 'ASC',
    SortingMainParameter: 0,
  })

  return { IsActiveSignUp, IsLogin, profiles, getProfileParams, getFilters }
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

interface GetProfileParams {
  SexualPreferences?: string,
  MaxDistance: number,
  MinFameRating: number,
  MaxFameRating: number,
  MaxAge: number,
  MinAge: number,
  IsLikedUser: boolean,
  CommonTags: Array<string>,
  IsMatched: boolean,
  SortLocation: string,
  SortFameRating: string,
  SortAge: string,
  SortCommonTags: string,
  SortingMainParameter: number,
  PageNumber: number,
  PageSize: number,
  Total: number
}

export interface GetFiltersType {
  maxAge: number,
  maxDistance: number,
  maxFameRating: number,
  minAge: number,
  minDistance: number,
  minFameRating: number
}
