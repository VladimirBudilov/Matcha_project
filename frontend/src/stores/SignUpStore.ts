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
    sort: {
      sortLocation: 'ASC',
      sortFameRating: 'ASC',
      sortAge: 'ASC',
      sortCommonTags: 'ASC',
      sortingMainParameter: 0,
    },
    pagination: {
      pageNumber: 1,
      pageSize: 10,
      total: 0
    },
    search: {
      maxDistance: 0,
      minFameRating: 0,
      maxFameRating: 0,
      minAge: 0,
      maxAge: 0,
      commonTags: [],
    }
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

interface sort {
  sortLocation: string,
  sortFameRating: string,
  sortAge: string,
  sortCommonTags: string,
  sortingMainParameter: number
}

interface pagination {
  pageNumber: number,
  pageSize: number
  total: number
}

interface search {
  sexualPreferences?: string,
  maxDistance: number,
  minFameRating: number,
  maxFameRating: number,
  maxAge: number,
  minAge: number,
  isLikedUser?: boolean,
  commonTags: Array<string>,
  isMatched?: boolean
}

interface GetProfileParams {
  sort: sort,
  pagination: pagination,
  search: search
}

export interface GetFiltersType {
  maxAge: number,
  maxDistance: number,
  maxFameRating: number,
  minAge: number,
  minDistance: number,
  minFameRating: number
}
