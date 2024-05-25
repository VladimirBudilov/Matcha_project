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
	await axios.get('api/profile/' + route.params.id).catch(() => {
		message.error(`User was not found!!!`);
	}).then((res) => {
		if (res?.data) {
			profile.value = res?.data
			uploadUrl.value = axios.defaults.baseURL + 'api/FileManager/uploadPhoto/' + profile.value.profileId
			console.log(profile.value)
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
	:label-col="{ span: 5 }"
	:wrapper-col="{ span: 12 }"
    layout="horizontal"
    :disabled="componentDisabled"
  	>
		<div class="Main-info">
			<a-form-item label="ID">
				<a-input-number v-model:value="profile.profileId" disabled style="background-color: grey; color:black"/>
			</a-form-item>
			<a-form-item label="Fame rating">
				<a-input-number v-model:value="profile.fameRating" disabled style="background-color: grey; color:black"/>
			</a-form-item>
			<a-form-item label="Username">
				<a-input v-model:value="profile.userName" disabled style="background-color: grey; color:black"/>
			</a-form-item>
			<a-form-item label="First Name">
				<a-input v-model:value="profile.firstName" disabled style="background-color: grey; color:black"/>
			</a-form-item>
			<a-form-item label="Last Name">
				<a-input v-model:value="profile.lastName" disabled style="background-color: grey; color:black"/>
			</a-form-item>
		</div>
		<div class="Optional-info">
			<a-form-item label="Gender">
				<a-input v-model:value="profile.gender" disabled style="background-color: grey; color:black" />
			</a-form-item>
			<a-form-item label="Age">
				<a-input-number v-model:value="profile.age" disabled style="background-color: grey; color:black"/>
			</a-form-item>
			<a-form-item label="Sexual preferences">
				<a-input v-model:value="profile.sexualPreferences" disabled style="background-color: grey; color:black" />
			</a-form-item>
			<a-form-item label="Location">
				<a-input-number style="width: 35%; background-color: grey; color:black" v-model:value="profile.latitude" disabled />
				<a-input-number style="width: 35%; margin-left: 5px; background-color: grey; color:black" v-model:value="profile.longitude" disabled/>
			</a-form-item>
			<a-form-item label="Interests" direction="vertical">
				<a-input mode="tags" v-model:value="profile.interests" disabled style="background-color: grey; color:black" />
			</a-form-item>
			<a-form-item label="Biography">
				<a-textarea v-model:value="profile.biography" placeholder="Biography" :rows="4" disabled style="background-color: grey; color:black"/>
			</a-form-item>
		</div>
	</a-form>
	</a-card>

	<a-card id="Avatar">
		<a-form-item label="Avatar" >
		<a-image v-if="profile.profilePicture.picture"
		:width="200"
		:src="'data:image/*' + ';base64,' + profile.profilePicture.picture"
		/>
	</a-form-item>

	<a-form-item label="Photos">
		<a-image-preview-group v-for="item in profile.pictures">
			<a-image
				:width="200"
				:src="'data:image/*' + ';base64,' + item.picture"
			/>
		</a-image-preview-group>
	</a-form-item>
	</a-card>

</template>

<style>
#User {
	position: relative;
	margin-top: 8vh;
	margin-left: 1vw;
	margin-right: 60vw;
	margin-bottom: 15vh;
	background-color: var(--color-background-soft);
	padding-top: 1vh;
	padding-bottom: 1vh;
}

</style>
