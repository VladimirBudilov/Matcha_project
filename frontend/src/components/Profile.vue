<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import axios from 'axios';

const componentDisabled = ref(false);
const labelCol = { style: { width: '150px' } };
const wrapperCol = { span: 14 };
const uploadUrl = ref('')
const errorMsg = ref('')


interface profile {
	"profileId": number,
	"userName": string,
	"firstName": string,
	"lastName": string,
	"gender": string | null,
	"sexualPreferences": string | null,
	"biography": string | null,
	"fameRating": number,
	"age": number,
	"location": string | null,
	"profilePicture": number | null,
	"pictures": Array<number>,
	"interests": Array<number>
}

const profile = ref<profile>({
	"profileId": 0,
	"userName": 'string',
	"firstName": 'string',
	"lastName": 'string',
	"gender": null,
	"sexualPreferences": null,
	"biography": null,
	"fameRating": 0,
	"age": 0,
	"location": null,
	"profilePicture": null,
	"pictures": [],
	"interests": []
})


onMounted(async () => {
	await axios.get('api/profile/' + localStorage.getItem('UserId')).then((res) => {
		profile.value = res?.data
		uploadUrl.value = 'api/FileManager/uploadPhoto/' + profile.value.profileId
		console.log(profile.value)
	})

})

const SubmiteChanges = async () => {
	errorMsg.value = ''
	await axios.put('api/profile/' + profile.value.profileId, profile.value).catch((msg) => {
		if (msg.response.data.errors) {
			if (msg.response.data.errors.Biography) {
				errorMsg.value = errorMsg.value + ' The Biography field is required.'
			}
			if (msg.response.data.errors.Location) {
				errorMsg.value = errorMsg.value + " The Location field is required."
			}
			if (msg.response.data.errors.SexualPreferences) {
				errorMsg.value = errorMsg.value + " The Sexual preferences field is required."
			}
			console.log(errorMsg.value)
		}
		if (msg.response.data.error) {
			errorMsg.value = errorMsg.value + msg.response.data.error
		}
	}).then((res) => {
		if (errorMsg.value == '') {
			errorMsg.value = "Success"
		}

	})
	setTimeout(() => {
		errorMsg.value = ''
	}, 10000)
}

</script>

<template>
	<a-form class="Profile"
    :label-col="labelCol"
    :wrapper-col="wrapperCol"
    layout="horizontal"
    :disabled="componentDisabled"
    style="max-width: 70vw"
  	>
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
		<a-form-item label="Gender">
			<a-select ref="select" style="width: 120px; color: red;" v-model:value="profile.gender">
				<a-select-option value="male" style="color:red">Male</a-select-option>
				<a-select-option value="female" style="color:red">Female</a-select-option>
			</a-select>
		</a-form-item>
		<a-form-item label="Age">
			<a-input-number v-model:value="profile.age" :min="18" :max="120"/>
		</a-form-item>
		<a-form-item label="Sexual preferences">
			<a-select v-model:value="profile.sexualPreferences">
				<a-select-option value="male">Male</a-select-option>
				<a-select-option value="female">Female</a-select-option>
			</a-select>
		</a-form-item>
		<a-form-item label="Location">
			<a-input v-model:value="profile.location"/>
		</a-form-item>
		<a-form-item label="Biography">
			<a-textarea v-model:value="profile.biography" placeholder="Biography" :rows="4" />
		</a-form-item>
		<a-button type="primary" html-type="signup" @click="SubmiteChanges" style="left: 40%;">Submite</a-button>
		<div v-if='errorMsg != "Success"' style="color: red">
			<span> {{errorMsg}} </span>
		</div>
		<div v-else-if='errorMsg = "Success"' style="color: green;">
			<span> {{errorMsg}} </span>
		</div>
		<a-form-item label="Avatar" style="position: absolute ;top: 20vh; left: 5vw">
			<a-upload :action="uploadUrl + '?id=1'" list-type="picture-card" :maxCount="1" accept=".jpg, .jpeg, .png">
				<div>
					<PlusOutlined style="color:grey"/>
				<div style="margin-top: 8px; color:grey">Upload</div>
				</div>
			</a-upload>
		</a-form-item>
		<a-form-item label="Photos" style="position: absolute ;top: 20vh; right: 5vw; max-width: 25vw;">
			<a-upload :action="uploadUrl + '?id=2'" list-type="picture-card" :maxCount="6" accept=".jpg, .jpeg, .png">
				<div>
					<PlusOutlined style="color:grey"/>
				<div style="margin-top: 8px; color:grey" >Upload</div>
				</div>
			</a-upload>
		</a-form-item>
	</a-form>

</template>

<style>

</style>
