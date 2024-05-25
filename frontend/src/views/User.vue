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
	<a-form id="User"
	:label-col="{ span: 3 }"
	:wrapper-col="{ span: 8 }"
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
	<a-form-item label="Avatar" style="position: absolute ; top: 7vh; left: 50vw; width: 50vw;">
		<a-image v-if="profile.profilePicture.picture"
		:width="200"
		:src="'data:image/*' + ';base64,' + profile.profilePicture.picture"
		/>
	</a-form-item>

	<a-form-item label="Photos" style="position: absolute ;top: 30vh; left: 50vw; width: 50vw; padding-bottom: 4vh;">
		<a-image-preview-group v-for="item in profile.pictures">
			<a-image
				:width="200"
				:src="'data:image/*' + ';base64,' + item.picture"
			/>
		</a-image-preview-group>
	</a-form-item>
</template>

<style>
#User {
	position: relative;
	padding-top: 7vh;
	padding-bottom: 4vh;
	color:black
}

</style>
