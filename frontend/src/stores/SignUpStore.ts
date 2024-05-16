import { ref } from 'vue'
import { defineStore } from 'pinia'

export const SignUpStore = defineStore('SignUp', () => {
  const IsActiveSignUp = ref(false)
  const IsLogin = ref(localStorage.getItem('token') ? true : false)

  return { IsActiveSignUp, IsLogin,  }
})

interface ProfilePicture {
  "pictureId": number,
  "picture": string
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
  "interests": Array<string>
}
