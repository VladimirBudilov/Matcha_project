<script setup lang="ts">
import type { GetProfileParams, Interests } from '@/stores/SignUpStore';
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { onMounted, reactive, ref } from 'vue';

const getProfileParams = reactive<GetProfileParams>({
	PageNumber: 1,
	PageSize: 10,
	Total: 0,
	SexualPreferences: '',
	//MaxDistance: number,
	//MinFameRating?: number,
	//MaxFameRating?: number,
	//MaxAge?: number,
	//MinAge?: number,
	//IsLikedUser?: boolean,
	CommonTags: [],
	//IsMatched?: boolean,
	//SortLocation: '',
	//SortFameRating: '',
	//SortAge: '',
	//SortCommonTags: '',
	//SortingMainParameter: '',
})

const profiles = storeToRefs(SignUpStore()).profiles

const GetProfile = async () => {
	await axios.get('api/profile', {
		params: getProfileParams
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
		:label-col="{ span: 14 }"
		:wrapper-col="{ span: 10 }"
		layout="horizontal"
		:disabled="false"
		style="width: 15vw"
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
			<a-form-item label="Max Distance">
				<a-input-number v-model:value="getProfileParams.MaxDistance"/>
			</a-form-item>
			<a-form-item label="Max Distance">
				<a-input-number v-model:value="getProfileParams.MaxDistance"/>
			</a-form-item>
			<a-form-item label="Is liked user">
				<a-checkbox v-model:checked="getProfileParams.IsLikedUser"></a-checkbox>
			</a-form-item>

		</a-form>
		<a-form
		:label-col="{ span: 14 }"
		:wrapper-col="{ span: 10 }"
		layout="horizontal"
		:disabled="false"
		style="width: 15vw"
		>
			<a-form-item label="Min fame rating">
				<a-input-number v-model:value="getProfileParams.MinFameRating"/>
			</a-form-item>
			<a-form-item label="Max fame rating">
				<a-input-number v-model:value="getProfileParams.MaxFameRating"/>
			</a-form-item>
			<a-form-item label="Max age">
				<a-input-number v-model:value="getProfileParams.MaxAge"/>
			</a-form-item>
			<a-form-item label="Min age">
				<a-input-number v-model:value="getProfileParams.MinAge" :min='18'/>
			</a-form-item>
			<a-form-item label="Is matched">
				<a-checkbox v-model:checked="getProfileParams.IsMatched"></a-checkbox>
			</a-form-item>
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
		</a-form>
		<a-form
		:label-col="{ span: 10 }"
		:wrapper-col="{ span: 15 }"
		layout="horizontal"
		:disabled="false"
		style="width: 30vw"
		>
		<a-form-item label="Sort location">
			<a-input v-model:value="getProfileParams.SortLocation" />
		</a-form-item>
		<a-form-item label="Sort fame rating">
			<a-input v-model:value="getProfileParams.SortFameRating" />
		</a-form-item>
		<a-form-item label="Sort age">
			<a-input v-model:value="getProfileParams.SortAge" />
		</a-form-item>
		<a-form-item label="Sort common tags">
			<a-input v-model:value="getProfileParams.SortCommonTags" />
		</a-form-item>
		<a-form-item label="Sorting main parameter">
			<a-input v-model:value="getProfileParams.SortingMainParameter" />
		</a-form-item>
		</a-form>
		</div>
		<a-button type="primary" html-type="signup" @click="GetProfile" style="position: absolute; padding-left: 1vw; z-index: 1;">Search</a-button>
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
