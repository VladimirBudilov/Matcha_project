<script setup lang="ts">
import type { Interests } from '@/stores/SignUpStore';
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { onMounted, ref } from 'vue';

const profiles = storeToRefs(SignUpStore()).profiles
const getProfileParams = storeToRefs(SignUpStore()).getProfileParams
const sortMainParam = [{label: 'Distance', value: 0},
	{label: 'Rating', value: 1},
	{label: 'Age', value: 2},
	{label: 'Interests', value: 3}]

const sort = [{label: 'Ascending', value:'ASC'},
	{label: 'Descending', value: 'DESC'}]

const age = ref<[number, number]>([getProfileParams.value.MinAge, getProfileParams.value.MaxAge])
const rating = ref<[number, number]>([getProfileParams.value.MinFameRating, getProfileParams.value.MaxFameRating])

const GetProfile = async () => {
	getProfileParams.value.MinAge = age.value[0]
	getProfileParams.value.MaxAge = age.value[1]
	getProfileParams.value.MinFameRating = rating.value[0]
	getProfileParams.value.MaxFameRating = rating.value[1]
	await axios.get('api/profile', {
		params: getProfileParams.value
	}).catch((res) => {

		if (res.code == 403) {
			message.error(`Fill out the profile!`);
		}
		else {
			message.error('Error')
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
onMounted(async () => {
	await GetInterests()
})

const genders = [{value: 'male', label: 'Male'} , {value: 'female', label: 'Female'}, {value: '', label: 'Nothing'}]

</script>

<template>
	<a-card id="input-params-get-profile">
		<div id="input-form-get-profile">
			<a-form
		:label-col="{ span: 10 }"
		:wrapper-col="{ span: 30 }"
		layout="horizontal"
		:disabled="false"
		style="width: 30vw"
		>
			<a-form-item label="User Id">
				<a-input-number v-model:value="getProfileParams.UserId"/>
			</a-form-item>
			<a-form-item label="Sexual preferences">
				<a-select
				v-model:value="getProfileParams.SexualPreferences"
				:options="genders"
				size="middle"
				placeholder="Please select"
				></a-select>
			</a-form-item>
			<a-form-item label="Is matched">
				<a-checkbox v-model:checked="getProfileParams.IsMatched"></a-checkbox>
			</a-form-item>
			<a-form-item label="Is liked user">
				<a-checkbox v-model:checked="getProfileParams.IsLikedUser"></a-checkbox>
			</a-form-item>

			<a-button type="primary" html-type="signup" @click="GetProfile" style="position: relative; margin-left: 7vw; z-index: 1;">Search</a-button>

		</a-form>
		<a-form
		:label-col="{ span: 10 }"
		:wrapper-col="{ span: 15 }"
		layout="horizontal"
		:disabled="false"
		style="width: 30vw"
		>
		<a-form-item label="Interests" direction="vertical">
				<a-select
				v-model:value="getProfileParams.CommonTags"
				:options="interests"
				mode="tags"
				size="middle"
				placeholder="Please select"
				></a-select>
		</a-form-item>
		<a-form-item label="Distance">
				<a-slider v-model:value="getProfileParams.MaxDistance" :max=200 />
			</a-form-item>
			<a-form-item label="Age">
				<a-slider v-model:value="age" range :min=18 />
			</a-form-item>
			<a-form-item label="Rating">
				<a-slider v-model:value="rating" range :max=999 />
			</a-form-item>
		</a-form>
		<a-form
		:label-col="{ span: 10 }"
		:wrapper-col="{ span: 15 }"
		layout="horizontal"
		:disabled="false"
		style="width: 30vw"
		>
		<a-form-item label="Sort location">
			<a-select
				v-model:value="getProfileParams.SortLocation"
				:options="sort"
				size="middle"
				placeholder="Please select"
				></a-select>
		</a-form-item>
		<a-form-item label="Sort fame rating">
			<a-select
				v-model:value="getProfileParams.SortFameRating"
				:options="sort"
				size="middle"
				placeholder="Please select"
				></a-select>
		</a-form-item>
		<a-form-item label="Sort age">
			<a-select
				v-model:value="getProfileParams.SortAge"
				:options="sort"
				size="middle"
				placeholder="Please select"
				></a-select>
		</a-form-item>
		<a-form-item label="Sort common tags">
			<a-select
				v-model:value="getProfileParams.SortCommonTags"
				:options="sort"
				size="middle"
				placeholder="Please select"
				></a-select>
		</a-form-item>
		<a-form-item label="Sorting main parameter">
			<a-select
				v-model:value="getProfileParams.SortingMainParameter"
				:options="sortMainParam"
				size="middle"
				placeholder="Please select"
				></a-select>
		</a-form-item>
		</a-form>
		</div>

	</a-card>


</template>

<style>
#input-params-get-profile {
	position: relative;
	margin-top: 7vh;
	padding-top: 3vh;
	padding-bottom: 2vh;
	background-color: var(--color-background-soft);

}

#input-form-get-profile {
	display: flex;
	flex-wrap: wrap;
}

</style>
