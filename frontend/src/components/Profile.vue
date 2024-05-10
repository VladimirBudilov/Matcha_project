<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import axios from 'axios';

const componentDisabled = ref(false);
const labelCol = { style: { width: '150px' } };
const wrapperCol = { span: 14 };


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


onMounted(() => {
	axios.get('api/profile/' + localStorage.getItem('UserId')).then((res) => {
		profile.value = res?.data
		console.log(profile.value)
	})
})

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
			<a-input-number v-model:value="profile.profileId" disabled="true" style="background-color: grey; color:black"/>
		</a-form-item>
		<a-form-item label="Fame rating">
			<a-input-number v-model:value="profile.fameRating" disabled="true" style="background-color: grey; color:black"/>
		</a-form-item>
		<a-form-item label="Username">
			<a-input v-model:value="profile.userName" disabled="true" style="background-color: grey; color:black"/>
		</a-form-item>
		<a-form-item label="First Name">
			<a-input v-model:value="profile.firstName" disabled="true" style="background-color: grey; color:black"/>
		</a-form-item>
		<a-form-item label="Last Name">
			<a-input v-model:value="profile.lastName" disabled="true" style="background-color: grey; color:black"/>
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
		<a-form-item label="Upload pictures">
			<a-upload list-type="picture-card" maxCount="6" accept=".jpg, .jpeg, .png">
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
