<script setup lang="ts">
import axios from 'axios';
import { onMounted, reactive, ref, watch } from 'vue';
import { message } from 'ant-design-vue';
import type { Profile, GetProfileParams } from '@/stores/SignUpStore';
import {useNotificationStore} from '@/stores/NotoficationStore'
import { SignUpStore } from '@/stores/SignUpStore';
import ParamsGetProfile from '@/components/ParamsGetProfile.vue'
import { storeToRefs } from 'pinia';

const store = useNotificationStore();
const connection = store.connection

const profiles = storeToRefs(SignUpStore()).profiles

const getProfileParams = reactive<GetProfileParams>({
	PageNumber: 1,
	PageSize: 10,
	Total: 0,
})
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
			if (getProfileParams.PageSize) {
				getProfileParams.Total = getProfileParams.PageSize * res.data.amountOfPages
			}

			console.log(res.data)
			console.log(res.data.profiles)
			console.log(profiles.value)
		}
	})
}



onMounted(async () => {
	await axios.get('api/auth/get-id').then((res) => {
		localStorage.setItem('UserId', String(res?.data))
	})

	await GetProfile()
})

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
			profiles.value.forEach(el => {
				if (el.profileId === profileId) {
					el.hasLike = !el.hasLike
				}
			})
		}
	})

  await connection?.invoke("SendNotificationToUser", Number(profileId));
}


watch(() => getProfileParams.PageNumber,
	async () => {
  	console.log('current', getProfileParams.PageNumber);
  	await GetProfile()}
);

</script>

<template>
	<div id="profiles">
		<ParamsGetProfile />
		<a-space id="profile-cards">
		<template #split>
			<a-divider type="vertical" />
		</template>
		<div v-for="el in profiles">
		<a-card id="profile-card" :span="8">
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
		:total="getProfileParams.Total" show-less-items
		:defaultPageSize="getProfileParams.PageSize"/>
	</div>


</template>

<style>
#profiles {
}

#profile-cards {
	/*position:absolute;*/
	padding-top: 7vh;
	padding-bottom: 10vh;
	padding-left: 1vw;
	padding-right: 1vw;
	width: 98vw;
	display: flex;
	flex-wrap: wrap;
}

#profile-card {
	position:inherit;
	width: 30vw;
}

#card-button-like {
	position: relative;
	padding-top: 3%;
	padding-bottom: 5%;
	padding-left: 40%;
}

#pagination-users {
	position: relative;
	margin-top: 10vh;
	width: 100%;
	bottom: 3vh;
	padding-top: 2vh;
	padding-bottom: 2vh;
	padding-left: 3vh;
	padding-right: 3vh;
	background-color: grey;
	color:black
}

</style>
