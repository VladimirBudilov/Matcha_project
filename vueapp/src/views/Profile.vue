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
const userName = ref('')
const firstName = ref('')
const lastName = ref('')
const email = ref('')

const checkAtr = async (str1 : string, str2 : string) => {
	if (str1 != str2)
		return false
	return true
}

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

const interests = ref<Interests[]>([])
const GetInterests = async () => {
	await axios.get('/api/profiles/interests').then((res) => {
		interests.value = res.data
		interests.value.forEach((element) => {
			element.value = element.name
		})
	})
}

const GetProfile = async () => {
	await axios.get('/api/profiles/' + localStorage.getItem('UserId')).catch((res) => {
		if (res.response.data){
			message.error(res.response.data)
		}
		else {
			message.error("Error")
		}
	}).then( async (res) => {
		profile.value = res?.data
		uploadUrl.value = '/api/FileManager/uploadPhoto/' + profile.value.profileId

		userName.value = profile.value.userName
		firstName.value = profile.value.firstName
		lastName.value = profile.value.lastName


		if (profile.value.latitude && profile.value.longitude) {
			const response = await fetch('https://geocode.maps.co/reverse?' + new URLSearchParams({
				lat: profile.value.latitude.toString(),
				lon: profile.value.longitude.toString(),
				api_key: import.meta.env.VITE_MAP_API_KEY
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
	})
}

onMounted(async () => {
	GetInterests()
	await GetProfile()
})

const SubmitChanges = async () => {
	errorMsg.value = ''
	if (
		await checkAtr(userName.value, profile.value.userName) ||
		await checkAtr(firstName.value, profile.value.firstName) ||
		await checkAtr(lastName.value, profile.value.lastName) ||
		await checkAtr(email.value, profile.value.email ? profile.value.email : '')
	) {
		await axios.put('/api/users/' + profile.value.profileId, profile.value).catch((res) => {
			errorMsg.value = 'Error'
			message.error(res.response.data.error)
		}).then((res) => {
			if (errorMsg.value == '') {

			}})
	}

	if (errorMsg.value === '') {
		await axios.put('/api/profiles/' + profile.value.profileId, profile.value).catch((res) => {
			errorMsg.value = 'Error'
			if (res.response.data.errors) {
				if (res.response.data.errors.Biography) {
					message.error('The Biography field is required.')
				}
				if (res.response.data.errors.Location) {
					message.error('The Location field is required.')
				}
				if (res.response.data.errors.SexualPreferences) {
					message.error('The Sexual preferences field is required.')
				}
			}
			else if (res.response.data.error) {
				message.error(res.response.data.error)
			}
			else {
				message.error(res.response.data)
			}
			message.error(errorMsg.value)
		}).then((res) => {
			if (errorMsg.value == '') {
				message.success("Success")
			}

		})
	}

}


const getLocation = async () => {
	if (profile.value.location) {
		const response : any = await fetch('https://geocode.maps.co/search?' + new URLSearchParams({
			q: profile.value.location,
			api_key: import.meta.env.VITE_MAP_API_KEY
		}).toString(), {
			method: 'GET'
		})
		const data : any = await response.json()

		if (!data.length) {
			message.error('The city or country was not found')
		}
		else {
			profile.value.latitude = data[0].lat
			profile.value.longitude = data[0].lon
		}
	}
	else {
    let ip: string = ''
    const ipResponse = (await fetch('https://api.ipify.org/')).text()
		await ipResponse.then(ipResponse => ip = ipResponse).catch(() => message.error('Error'))

		const LocatByIp : any = await (await fetch('https://ipwho.is/' + ip)).json()
		if (!LocatByIp || !LocatByIp.latitude || !LocatByIp.longitude) {
			message.error('The city or country was not found')
		}
		else {
			profile.value.latitude = LocatByIp.latitude
			profile.value.longitude = LocatByIp.longitude
			if (LocatByIp.city) {
				profile.value.location = LocatByIp.city
			}
			else if (LocatByIp.country) {
				profile.value.location = LocatByIp.country
			}
			else {
				profile.value.location = 'Milky Way'
			}
		}
	}
}


const genders = [{value: 'male', label: 'Male'} , {value: 'female', label: 'Female'}]
const sexualPreferences = [{value: 'male', label: 'Male'} , {value: 'female', label: 'Female'} , {value: null, label: 'Whatever can move'}]

const fileList = ref([]);
const headers = {
  authorization: 'Bearer ' + localStorage.getItem('token')
};

const handleChange = async (info: UploadChangeParam) => {
  if (info.file.status !== 'uploading') {

  }
  if (info.file.status === 'done') {
    message.success(`${info.file.name} file uploaded successfully`);
	setTimeout(async () => {
		await GetProfile()
	}, 100)

  } else if (info.file.status === 'error') {
    message.error(`${info.file.name} file upload failed.`);
  }
};

const DeletePicture = async (picureId: number) => {
	axios.delete('/api/FileManager/deletePhoto/' + profile.value.profileId + '?photoId=' + picureId).then(async () => {
		await GetProfile()
	})
}



</script>

<template>
	<a-card id="Profile">
	<a-form
	:label-col="{ span: 7 }"
	:wrapper-col="{ span: 12 }"
    layout="horizontal"
    :disabled="componentDisabled"
  	>
		<div class="Main-info">
			<a-button id="submit-changes" type="primary" html-type="signup" @click="SubmitChanges">Submite</a-button>
			<a-form-item label="Fame rating">
				<a-input-number v-model:value="profile.fameRating" disabled style="background-color: var(--color-background-soft); color: var(--color-text)"/>
			</a-form-item>
			<a-form-item label="Username">
				<a-input v-model:value="profile.userName"/>
			</a-form-item>
			<a-form-item label="First Name">
				<a-input v-model:value="profile.firstName"/>
			</a-form-item>
			<a-form-item label="Last Name">
				<a-input v-model:value="profile.lastName"/>
			</a-form-item>
			<a-form-item label="Email">
				<a-input v-model:value="profile.email"/>
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
				:options="sexualPreferences"
				size="middle"
				placeholder="Please select"
				></a-select>
			</a-form-item>
			<a-form-item label="Location">
				<a-input v-model:value="profile.location"/>
			</a-form-item>
			<a-form-item label="Lat & long">
				<a-input-number disabled style="width: 30%; background-color: var(--color-background-soft); color: var(--color-text)" v-model:value="profile.latitude" />
				<a-input-number disabled style="width: 30%; margin-left: 5px; background-color: var(--color-background-soft); color: var(--color-text)" v-model:value="profile.longitude"/>
				<a-button type="primary" html-type="signup" @click="getLocation" style="margin-left: 5px;top: 4px;">Location</a-button>
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
	</a-form>
	</a-card>

	<a-card id="Avatar">
		<a-form-item label="Avatar">
			<a-image v-if="profile.profilePicture.picture"
			:src="'data:image/*' + ';base64,' + profile.profilePicture.picture"
			/>
			<div id="avatar-upload">
				<a-upload
				v-model:file-list="fileList"
				:showUploadList="false"
				name="file"
				:action="uploadUrl + '?isMain=true'"
				:headers="headers"
				:maxCount="1"
				@change="handleChange"
				accept=".jpg, .jpeg, .png"
				>
					<a-button>
					<upload-outlined></upload-outlined>
					Click to Upload
					</a-button>
				</a-upload>
			</div>

		</a-form-item>
	</a-card>

	<a-card id="Photos">
		<a-form-item label="Photos">
			<div id="photos-profile">
				<a-image-preview-group v-for="item in profile.pictures">
					<div id="photo-profile">
						<a-image
						:src="'data:image/*' + ';base64,' + item.picture"

						/>
							<div>
								<a-button id="delete-picture" @click="DeletePicture(item.pictureId)">
								Delete
								</a-button>
							</div>
					</div>
				</a-image-preview-group>
			</div>

			<div id="upload-photo-profile">
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
				<a-button>
				<upload-outlined></upload-outlined>
				Click to Upload
				</a-button>
			</a-upload>
			</div>

		</a-form-item>

	</a-card>
</template>

<style>
#Profile {
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

#submit-changes {
	position: absolute;
	z-index: 1;
}

#Avatar {
	position: fixed;
	top: 8vh;
	margin-left: 50vw;
	width: 50vw;
	background-color: var(--color-background-mute);

	.css-dev-only-do-not-override-19iuou.ant-image .ant-image-img {
		width: 15vw;
		height: 20vh;
	}
}

#avatar-upload{
	margin-top: 3vh;
}

#Photos {
	position: fixed;
	top: 40vh;
	margin-left: 50vw;
	width: 50vw;
	height: 53vh;
	overflow: auto;
	background-color: var(--color-background-mute);
	.css-dev-only-do-not-override-19iuou.ant-image .ant-image-img {
		width: 9vw;
		height: 15vh;
	}
}

#photos-profile {
	display: flex;
	flex-wrap: wrap;
}

#photo-profile {
	margin-bottom: 1vh;
	width: 10vw;
	max-height: 20vh;
}

#delete-picture {
	margin-top: 1vh;
}

#upload-photo-profile {
	margin-top: 3vh;
}

.anticon {
	color: inherit;
}

@media screen and (max-width: 1100px) {
	#Avatar {
		.css-dev-only-do-not-override-19iuou.ant-image .ant-image-img {
			width: 30vw;
			height: 15vh;
		}
	}

	#Photos {
		.css-dev-only-do-not-override-19iuou.ant-image .ant-image-img {
			width: 30vw;
			height: 15vh;
		}
	}

	#photos-profile {
		display: inherit;
	}
}

</style>
