<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { message } from 'ant-design-vue';
import { UploadOutlined } from '@ant-design/icons-vue';
import type { UploadChangeParam } from 'ant-design-vue';
import axios from 'axios';
import {type Profile, type Interests} from '@/stores/SignUpStore'

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

const interests = ref<Interests[]>([])
const GetInterests = async () => {
	await axios.get('api/profile/interests').then((res) => {
		console.log(res)
		interests.value = res.data
		interests.value.forEach((element) => {
			element.value = element.name
		})
	})
}

const GetProfile = async () => {
	await axios.get('api/profile/' + localStorage.getItem('UserId')).then((res) => {
		profile.value = res?.data
		uploadUrl.value = axios.defaults.baseURL + 'api/FileManager/uploadPhoto/' + profile.value.profileId
		GetInterests()
		console.log(profile.value)
	})
}

onMounted(async () => {
	await GetProfile()
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


const genders = [{value: 'male', label: 'Male'} , {value: 'female', label: 'Female'}]

const fileList = ref([]);
const headers = {
  authorization: 'Bearer ' + localStorage.getItem('token')
};

const handleChange = async (info: UploadChangeParam) => {
  if (info.file.status !== 'uploading') {
    console.log(info.file, info.fileList);
  }
  if (info.file.status === 'done') {
    message.success(`${info.file.name} file uploaded successfully`);
	await GetProfile()
  } else if (info.file.status === 'error') {
    message.error(`${info.file.name} file upload failed.`);
  }
};

const DeletePicture = async (picureId: number) => {
	axios.delete('api/FileManager/deletePhoto/' + profile.value.profileId + '?photoId=' + picureId).then(async () => {
		await GetProfile()
	})
}



</script>

<template>
	<a-form class="Profile"
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
		</div>

		<a-button type="primary" html-type="signup" @click="SubmiteChanges" style="position: absolute;top: 7vh;">Submite</a-button>
		<div v-if='errorMsg != "Success"' style="color: red">
			<span> {{errorMsg}} </span>
		</div>
		<div v-else-if='errorMsg = "Success"' style="color: green;">
			<span> {{errorMsg}} </span>
		</div>
	</a-form>
	<a-form-item label="Avatar" style="position: absolute ; top: 7vh; left: 50vw; width: 50vw;">
		<a-image
			:width="200"
			:src="'data:image/*' + ';base64,' + profile.profilePicture.picture"
		/>
		<a-upload
			v-model:file-list="fileList"
			:showUploadList="false"
			name="file"
			:action="uploadUrl + '?isMain=true'"
			:headers="headers"
			:maxCount="1"
			style="max-width: 100%;"
			@change="handleChange"
			accept=".jpg, .jpeg, .png"
		>
			<a-button style="margin-left: 3px;">
			<upload-outlined></upload-outlined>
			Click to Upload
			</a-button>
		</a-upload>
	</a-form-item>

	<a-form-item label="Photos" style="position: absolute ;top: 30vh; left: 50vw; width: 50vw; padding-bottom: 4vh;">
		<a-image-preview-group v-for="item in profile.pictures">
			<a-image
				:width="200"
				:src="'data:image/*' + ';base64,' + item.picture"
			/>
			<a-button style="margin-left: 3px;" @click="DeletePicture(item.pictureId)">
			Delete
			</a-button>
		</a-image-preview-group>
		<a-upload
			v-model:file-list="fileList"
			:showUploadList="false"
			name="file"
			:action="uploadUrl + '?isMain=false'"
			:headers="headers"
			:maxCount="1"
			@change="handleChange"
			accept=".jpg, .jpeg, .png"
		>
			<a-button style="margin-left: 3px;">
			<upload-outlined></upload-outlined>
			Click to Upload
			</a-button>
		</a-upload>
	</a-form-item>
</template>

<style>
.Profile {
	position: relative;
	padding-top: 7vh;
	padding-bottom: 4vh;
}


</style>
