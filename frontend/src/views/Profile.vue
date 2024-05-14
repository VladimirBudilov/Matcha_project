<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import axios from 'axios';
import {type Profile} from '@/stores/SignUpStore'

const componentDisabled = ref(false);
const uploadUrl = ref('')
const errorMsg = ref('')

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


onMounted(async () => {
	await axios.get('api/profile/' + localStorage.getItem('UserId')).then((res) => {
		profile.value = res?.data
		uploadUrl.value = 'api/FileManager/uploadPhoto/' + profile.value.profileId
		console.log(profile.value)
		console.log(profile.value.interests)
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

const getLocation = async () => {
	errorMsg.value = ''
	navigator.geolocation.getCurrentPosition((pos => {
		profile.value.latitude = pos.coords.latitude
		profile.value.longitude = pos.coords.longitude
	}), (err => {
		profile.value.latitude = 0
		profile.value.longitude = 0
		errorMsg.value = err.message
	}))
}

const interests = [{value: 'Music'} , {value: 'Dance'}, {value: 'kek'}, {value: 'Movie'}]
const genders = [{value: 'male', label: 'Male'} , {value: 'female', label: 'Female'}]

</script>

<template>
	<a-form class="Profile"
	:label-col="{ span: 9 }"
	:wrapper-col="{ span: 9 }"
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
			<a-select
			v-model:value="profile.gender"
			:options="genders"
			size="middle"
			placeholder="Please select"
			></a-select>
		</a-form-item>
		<a-form-item label="Age">
			<a-input-number v-model:value="profile.age" :min="18" :max="120"/>
		</a-form-item>
		<a-form-item label="Sexual preferences">
			<a-select
			v-model:value="profile.sexualPreferences"
			:options="genders"
			size="middle"
			placeholder="Please select"
			></a-select>
		</a-form-item>
		<a-form-item label="Location">
			<a-input-number style="width: 35%;" v-model:value="profile.latitude"/>
			<a-input-number style="width: 35%; margin-left: 5px;" v-model:value="profile.longitude"/>
			<a-button type="primary" html-type="signup" @click="getLocation" style="margin-left: 5px;">Location</a-button>
		</a-form-item>
		<a-form-item label="Interests" direction="vertical">
			<a-select
			v-model:value="profile.interests"
			:options="interests"
			mode="tags"
			size="middle"
			placeholder="Please select"
			></a-select>
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
			<a-upload :action="uploadUrl + '?isMain=true'" list-type="picture-card" :maxCount="1" accept=".jpg, .jpeg, .png">
				<div>
					<PlusOutlined style="color:grey"/>
					<div style="margin-top: 8px; color:grey">Upload</div>
				</div>
			</a-upload>
		</a-form-item>
		<a-form-item label="Photos" style="position: absolute ;top: 20vh; right: 5vw; max-width: 25vw;">
			<a-upload :action="uploadUrl + '?isMain=false'" list-type="picture-card" :maxCount="6" accept=".jpg, .jpeg, .png">
				<div>
					<PlusOutlined style="color:grey"/>
				<div style="margin-top: 8px; color:grey" >Upload</div>
				</div>
			</a-upload>
		</a-form-item>
	</a-form>
</template>

<style>
.Gender {
	width: 100%;
	height: 32px;
	padding: 0 11px;
	cursor: pointer;
	border-radius: 6px
}


</style>
