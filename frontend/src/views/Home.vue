<script setup lang="ts">
import axios from 'axios';
import { onMounted, ref } from 'vue';
import { message } from 'ant-design-vue';
import type { Profile } from '@/stores/SignUpStore';

const columns = [
  {
    title: '#',
    dataIndex: 'profileId',
    key: 'profileId',
	width: '5%',
  },
  {
    title: 'Avatar',
    dataIndex: 'profilePicture',
    key: 'profilePicture',
	width: '10%',
  },
  {
    title: 'First name',
    dataIndex: 'firstName',
    key: 'firstName',
	width: '10%',
  },
  {
    title: 'Last name',
    dataIndex: 'lastName',
    key: 'lastName',
	width: '10%',
  },
  {
    title: 'Age',
    dataIndex: 'age',
    key: 'age',
	width: '5%',
  },
  {
    title: 'Gender',
    dataIndex: 'gender',
    key: 'gender',
	width: '7%',
  },
  {
    title: 'Sexual preferences',
    dataIndex: 'sexualPreferences',
    key: 'sexualPreferences',
	width: '7%',
  },
  {
    title: 'Interests',
    key: 'interests',
    dataIndex: 'interests',
	width: '20%',
  },
  {
    title: 'Latitude',
    key: 'latitude',
    dataIndex: 'latitude',
	width: '5%',
  },
  {
    title: 'Longitude',
    key: 'longitude',
    dataIndex: 'longitude',
	width: '5%',
  },
  {
    title: 'Like',
    key: 'like',
  },
]

const profiles = ref<Profile[]>([])


onMounted(async () => {
	await axios.get('api/auth/get-id').then((res) => {
		localStorage.setItem('UserId', String(res?.data))
	})

	axios.get('api/profile').catch((res) => {
		if (res.code == 403) {
			message.success(`Fill out the profile!`);
		}
	}).then((res) => {
		if (res?.data){
			profiles.value = res?.data.profiles
			console.log(res?.data)
			console.log(res?.data.profiles)
			console.log(profiles.value)
		}
	})
})

const sendLike = async (profileId: number) => {
	await axios.post('api/actions/like', {likerId: Number(localStorage.getItem('UserId')), likedId: profileId}).catch((res) => {
		message.error(`Error: ${res.response.data.error}`);
	}).then((res) => {
		if (res?.data) {
			message.success(`${res?.data}`);
		}
	})
}

</script>

<template>
	<div id="Profiles">
		<a-table :columns="columns" :data-source="profiles">
			<template #bodyCell="{ column, record }">
				<template v-if="column.key === 'firstName'">
					<RouterLink :to="'/user/' + record.profileId">{{ record.firstName }}</RouterLink>
					<!--<a :href="''">

					</a>-->
				</template>
				<template v-if="column.key === 'profilePicture'">
					<a-image
						:width="100"
						:src="'data:image/*' + ';base64,' + record.profilePicture"
					/>
				</template>
				<template v-else-if="column.key === 'interests'">
					<span>
					<a-tag
						v-for="interest in record.interests"
						:key="interest"
						:color="interest === interest.length > 5 ? 'geekblue' : 'green'"
					>
						{{ interest.toUpperCase() }}
					</a-tag>
					</span>
				</template>
				<template v-else-if="column.key === 'like'">
					<a-button @click="sendLike(record.profileId)">
						like
					</a-button>
				</template>
			</template>
		</a-table>
	</div>


</template>

<style>
#Profiles {
	position: relative;
	padding-top: 7vh;
	padding-bottom: 4vh;
	padding-left: 1vw;
	padding-right: 1vw;
}

</style>
