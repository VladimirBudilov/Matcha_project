<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { message } from 'ant-design-vue';
import { useRoute } from 'vue-router';
import axios from 'axios';
import {type Profile} from '@/stores/SignUpStore'

const componentDisabled = ref(true);
const uploadUrl = ref('')

const route = useRoute();

const profile = ref<Profile>({
	"profileId": 0,
	"userName": '',
	"firstName": '',
	"lastName": '',
	"gender": null,
	"sexualPreferences": null,
	"biography": null,
	"fameRating": 0,
	"age": 0,
	"location": '',
	"latitude": 0,
	"longitude": 0,
	"profilePicture": {
		"pictureId": 0,
		"picture": ''
	},
	"pictures": [],
	"interests": []
})

const GetProfile = async () => {
	await axios.get('api/profiles/' + route.params.id).catch( async () => {
		message.error(`User was not found!!!`);
	}).then(async (res) => {
		if (res?.data) {
			profile.value = res?.data
			uploadUrl.value = axios.defaults.baseURL + 'api/FileManager/uploadPhoto/' + profile.value.profileId
		}
		if (profile.value.latitude && profile.value.longitude) {
			const response = await fetch('https://geocode.maps.co/reverse?' + new URLSearchParams({
				lat: profile.value.latitude.toString(),
				lon: profile.value.longitude.toString(),
				api_key: process.env.MAP_API_KEY
			}).toString(), {
				method: 'GET'
			})
			const data : any = await response.json()
			if (data.error || !data.address || (!data.address.city && !data.address.country)){
				profile.value.location = 'Milky Way'
			}
			else if (data.address.city) {
				profile.value.location = data.address.city
			}
			else if (data.address.country) {
				profile.value.location = data.address.country
			}
		}
	});
}

const sendLike = async (profileId: number) => {
	await axios.post('api/actions/like', {producerId: Number(localStorage.getItem('UserId')), consumerId: profileId}).catch((res) => {
		message.error(`Error: ${res.response.data.error} ${localStorage.getItem('UserId')}`);
	}).then((res) => {
		if (res?.data) {
			if (res?.data.isLiked) {
				message.success(`You have liked!`);
			}
			else {
				message.success(`You have removed you like!`);
			}
			profile.value.hasLike = !profile.value.hasLike
		}
	})
}

onMounted(async () => {
	await GetProfile()
})


</script>

<template>
	<a-card id="User">
		<a-form
	:label-col="{ span: 6 }"
	:wrapper-col="{ span: 12 }"
    layout="horizontal"
    :disabled="false"
  	>
		<div class="Main-info">

			<a-button v-if="!profile.hasLike" id="user-like-button" html-type="signup" @click="sendLike(profile.profileId)">Like</a-button>
			<a-button v-else id="user-like-button" type="primary" html-type="signup" @click="sendLike(profile.profileId)">Like</a-button>
			<a-form-item label="Fame rating">
				<a-input-number v-model:value="profile.fameRating" disabled style="background-color: var(--color-background-soft); color: var(--color-text)"/>
			</a-form-item>
			<a-form-item label="Username">
				<a-input v-model:value="profile.userName" disabled style="background-color: var(--color-background-soft); color: var(--color-text)"/>
			</a-form-item>
			<a-form-item label="First Name">
				<a-input v-model:value="profile.firstName" disabled style="background-color: var(--color-background-soft); color: var(--color-text)"/>
			</a-form-item>
			<a-form-item label="Last Name">
				<a-input v-model:value="profile.lastName" disabled style="background-color: var(--color-background-soft); color: var(--color-text)"/>
			</a-form-item>
		</div>
		<div class="Optional-info">
			<a-form-item label="Gender">
				<a-input v-model:value="profile.gender" disabled style="background-color: var(--color-background-soft); color: var(--color-text)" />
			</a-form-item>
			<a-form-item label="Age">
				<a-input-number v-model:value="profile.age" disabled style="background-color: var(--color-background-soft); color: var(--color-text)"/>
			</a-form-item>
			<a-form-item label="Sexual preferences">
				<a-input v-model:value="profile.sexualPreferences" disabled style="background-color: var(--color-background-soft); color: var(--color-text)" />
			</a-form-item>
			<a-form-item label="Location">
				<a-input v-model:value="profile.location" style="background-color: var(--color-background-soft); color: var(--color-text)"/>
			</a-form-item>
			<a-form-item label="Interests" direction="vertical">
				<a-input mode="tags" v-model:value="profile.interests" disabled style="background-color: var(--color-background-soft); color: var(--color-text)" />
			</a-form-item>
			<a-form-item label="Biography">
				<a-textarea v-model:value="profile.biography" placeholder="Biography" :rows="4" disabled style="background-color: var(--color-background-soft); color: var(--color-text)"/>
			</a-form-item>
		</div>
	</a-form>
	</a-card>

	<a-card id="Avatar">
			<a-form-item label="Avatar" >
				<a-image v-if="profile.profilePicture.picture"
				:src="'data:image/*' + ';base64,' + profile.profilePicture.picture"
				/>
		</a-form-item>
	</a-card>

	<a-card id="Photos">
		<a-form-item label="Photos">
			<a-image-preview-group v-for="item in profile.pictures">
				<a-image
					:src="'data:image/*' + ';base64,' + item.picture"
				/>
			</a-image-preview-group>
		</a-form-item>
	</a-card>

</template>

<style>
#User {
	position: fixed;
	width: 50%;
	height: 85vh;
	margin-top: 8vh;
	margin-right: 60vw;
	margin-bottom: 15vh;
	background-color: var(--color-background-mute);
	overflow: auto;
	padding-top: 1vh;
	padding-bottom: 1vh;
}

#user-like-button {
	position: absolute;
	padding-left: 1vw;
	z-index: 1;
}

</style>
