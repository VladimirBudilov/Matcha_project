<script setup lang="ts">
import axios from 'axios';
import { onMounted, reactive, ref, watch } from 'vue';
import { message } from 'ant-design-vue';
import type { Profile, GetProfileParams } from '@/stores/SignUpStore';

const profiles = ref<Profile[]>([])

const GetProfile = async () => {

	await axios.get('api/profile', {
		params: getProfileParams
	}).catch((res) => {
		if (res.code == 403) {
			message.success(`Fill out the profile!`);
		}
	}).then((res) => {
		if (res?.data){
			profiles.value = res.data.profiles
			console.log(res.data)
			console.log(res.data.profiles)
			console.log(profiles.value)
		}
	})
}

const getProfileParams = reactive<GetProfileParams>({
	PageNumber: 1,
	PageSize: 10
})

onMounted(async () => {
	await axios.get('api/auth/get-id').then((res) => {
		localStorage.setItem('UserId', String(res?.data))
	})

	await GetProfile()
})

const sendLike = async (profileId: number) => {
	await axios.post('api/actions/like', {likerId: Number(localStorage.getItem('UserId')), likedId: profileId}).catch((res) => {
		message.error(`Error: ${res.response.data.error}`);
	}).then((res) => {
		if (res?.data) {
			if (res?.data.isLiked) {
				message.success(`You have liked!`);
			}
			else {
				message.success(`You have removed you like!`);
			}
			profiles.value.forEach(el => {
				if (el.profileId === profileId) {
					el.hasLike = !el.hasLike
				}
			})
		}
	})
}


watch(() => getProfileParams.PageNumber,
	async () => {
  	console.log('current', getProfileParams.PageNumber);
  	await GetProfile()}
);

</script>

<template>
	<a-space id="Profiles">
		<template #split>
			<a-divider type="vertical" />
		</template>
		<div v-for="el in profiles">
		<a-card style="position: relative; width: 30vw" :span="8">
			<a-card-meta>
				<template #avatar>
					<a-image
					:src="'data:image/*' + ';base64,' + el.profilePicture" />
				</template>
			</a-card-meta>
			<a-card-meta :title="el.firstName + ' ' + el.lastName" style="padding-top: 1%;">
				<template #description>
					<p>
						Age: {{ el.age }}
					</p>
					<p>
						Gender: {{ el.gender }}
					</p>
					<p>
						Interests:
						<a-tag
							v-for="interest in el.interests"
							:color="'geekblue'"
						>
							{{ interest.toUpperCase() }}
						</a-tag>
					</p>
					<p>
						Sexual preferences: {{ el.sexualPreferences }}
					</p>
					<p>
						<RouterLink :to="'/user/' + el.profileId">Get more info</RouterLink>
					</p>
				</template>
			</a-card-meta>
			<div id="card-button-like">
				<a-button v-if="el.hasLike" size='large' type="primary" @click="sendLike(el.profileId)">
					like
				</a-button>
				<a-button v-else size='large' @click="sendLike(el.profileId)">
					like
				</a-button>
			</div>
		</a-card>
	</div>
	</a-space>
	<a-pagination
		id="pagination-users"
		v-model:current="getProfileParams.PageNumber"
		:total="50" show-less-items
		:defaultPageSize="getProfileParams.PageSize"/>
</template>

<style>
#Profiles {
	position: relative;
	padding-top: 7vh;
	padding-bottom: 4vh;
	padding-left: 1vw;
	padding-right: 1vw;
}
#card-button-like {
	position: relative;
	padding-top: 3%;
	padding-bottom: 3%;
	padding-left: 40%;
}

#pagination-users {
	position: relative;
	top: 7vh;
	padding-top: 2vh;
	padding-bottom: 2vh;
	padding-left: 20vw;
	padding-right: 20vw;
	background-color: grey;
	color:black
}

</style>
