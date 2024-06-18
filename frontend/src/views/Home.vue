<script setup lang="ts">
import axios from 'axios';
import { onMounted, ref, watch } from 'vue';
import { message } from 'ant-design-vue';
import {useNotificationStore} from '@/stores/NotoficationStore'
import { SignUpStore } from '@/stores/SignUpStore';
import ParamsGetProfile from '@/components/ParamsGetProfile.vue'
import { storeToRefs } from 'pinia';

const store = useNotificationStore();

const profiles = storeToRefs(SignUpStore()).profiles
const getFilters = storeToRefs(SignUpStore()).getFilters
const getProfileParams = storeToRefs(SignUpStore()).getProfileParams

const GetFilters = async () => {
	await axios.get('api/profiles/filters').catch((res) => {
		if (res.response.data) {
			message.error(res.response.data)
		}
	}).then ((res) => {
		if (res?.data) {
			getFilters.value = res.data
			getFilters.value.maxDistance = Math.floor(getFilters.value.maxDistance)
		}
	})
}

const GetProfile = async () => {
	getProfileParams.value.search.maxDistance = getFilters.value.maxDistance
	getProfileParams.value.search.minAge = getFilters.value.minAge
	getProfileParams.value.search.maxAge = getFilters.value.maxAge
	getProfileParams.value.search.minFameRating = getFilters.value.minFameRating
	getProfileParams.value.search.maxFameRating = getFilters.value.maxFameRating

	console.log(getProfileParams.value)
	await axios.post('api/profiles', getProfileParams.value).catch((res) => {
		if (res.response) {
			message.error(`Fill out the profile!`);
		}
	}).then((res) => {
		if (res?.data){
			profiles.value = res.data.profiles
			if (getProfileParams.value.pagination.pageSize) {
				getProfileParams.value.pagination.total = getProfileParams.value.pagination.pageSize * res.data.amountOfPages
			}

			console.log(profiles.value)
		}
	})
}



onMounted(async () => {
	await axios.get('api/auth/get-id').then((res) => {
		localStorage.setItem('UserId', String(res?.data))
	})
	await GetFilters()
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

  await store?.connection?.invoke("SendNotificationToUser", Number(profileId)).catch(err => console.log(err.toString()));
}


watch(
	() => getProfileParams.value.pagination.pageNumber,
	async () => {
		console.log('current', getProfileParams.value.pagination.pageNumber);
		await GetProfile()
	}
);

</script>

<template>
	<div id="profiles">
		<ParamsGetProfile/>
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
		v-model:current="getProfileParams.pagination.pageNumber"
		:total="getProfileParams.pagination.total" show-less-items
		:defaultPageSize="getProfileParams.pagination.pageSize"/>
	</div>


</template>

<style>

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
