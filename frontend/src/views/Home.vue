<script setup lang="ts">
import axios from 'axios';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import { onMounted, ref, watch } from 'vue';
import { message } from 'ant-design-vue';
import { SignUpStore } from '@/stores/SignUpStore';
import ParamsGetProfile from '@/components/ParamsGetProfile.vue'
import { storeToRefs } from 'pinia';
dayjs.extend(relativeTime);

const profiles = storeToRefs(SignUpStore()).profiles
const getFilters = storeToRefs(SignUpStore()).getFilters
const getProfileParams = storeToRefs(SignUpStore()).getProfileParams

const GetFilters = async () => {
	await axios.get('api/profiles/filters').catch((res) => {
		if (res.response.data) {
			message.error(res.response.data)
		}
	}).then (async (res) => {
		if (res?.data) {
			getFilters.value = res.data
			getFilters.value.maxDistance = await Math.floor(getFilters.value.maxDistance)
		}
	})

	getProfileParams.value.search.maxDistance = getFilters.value.maxDistance
	getProfileParams.value.search.minAge = getFilters.value.minAge
	getProfileParams.value.search.maxAge = getFilters.value.maxAge
	getProfileParams.value.search.minFameRating = getFilters.value.minFameRating
	getProfileParams.value.search.maxFameRating = getFilters.value.maxFameRating


	getProfileParams.value.search.isMatched = null
	getProfileParams.value.search.isLikedUser = null
	getProfileParams.value.search.sexualPreferences = null
	await GetProfile()
}

const GetProfile = async () => {
	await axios.post('api/profiles', getProfileParams.value).catch((res) => {
		if (res.response) {
			message.error(`Fill out the profile!`);
		}
	}).then( async (res) => {
		if (res?.data){
			profiles.value = res.data.profiles
			if (getProfileParams.value.pagination.pageSize) {
				getProfileParams.value.pagination.total = getProfileParams.value.pagination.pageSize * res.data.amountOfPages
			}
		}
	})
}



onMounted(async () => {
	await axios.get('api/auth/get-id').then((res) => {
		localStorage.setItem('UserId', String(res?.data))
	})
	await GetFilters()
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
}

const block = async (profileId: number) => {
	await axios.post('api/actions/block', {producerId: Number(localStorage.getItem('UserId')), consumerId: profileId}).catch((res) => {
		message.error(`Error: ${res.response.data.error} ${localStorage.getItem('UserId')}`);
	}).then((res) => {
		if (res?.data) {
			if (res?.data) {
				message.success(`You blocked!`);
				profiles.value.forEach(function(item, index, object) {
					if (item.profileId == profileId) {
						object.splice(index, 1);
					}
				})
			}
		}
	})

}


watch(
	() => getProfileParams.value.pagination.pageNumber,
	async () => {
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
					<a-image v-if="typeof(el.profilePicture) === 'string'"
					:src="'data:image/*' + ';base64,' + el.profilePicture" />
				</template>
			</a-card-meta>
			<a-card-meta :title="el.firstName + ' ' + el.lastName" style="padding-top: 1%;">
				<template #description>
					<p v-if="el.isOnlineUser" style="color: green;">
						Online
					</p>
					<p v-else style="color: red;">
						Offline
						<span style="color: grey;" v-if="el.lastLogin">
							last was seen {{ dayjs(el.lastLogin).format('YYYY-MM-DD HH:mm:ss') }}
						</span>
					</p>
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
			<div>
				<div id="card-button-like-block">
				<a-button v-if="el.hasLike" size='large' type="primary" @click="sendLike(el.profileId)">
					like
				</a-button>
				<a-button v-else size='large' @click="sendLike(el.profileId)">
					like
				</a-button>
				<span id="card-button-block">
					<a-button size='large' @click="block(el.profileId)">
						block
					</a-button>
				</span>

			</div>
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

@media screen and (max-width: 1100px) {
	#profile-card {
		position:inherit;
		width: 100%;
	}
}

#card-button-like-block {
	position: relative;
	padding-top: 3%;
	padding-bottom: 5%;
	padding-left: 20%;
}

#card-button-block {
	position: relative;
	width: fit-content;
	padding-left: 10%;
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
	background-color: var(--color-background-soft);
	color:var(--color-text);
}

</style>
