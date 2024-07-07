<script setup lang="ts">
import type {  Interests } from '@/stores/SignUpStore';
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { onMounted, ref, watch } from 'vue';

const profiles = storeToRefs(SignUpStore()).profiles
const getProfileParams = storeToRefs(SignUpStore()).getProfileParams
const getFilters = storeToRefs(SignUpStore()).getFilters

const sortMainParam = [{label: 'Distance', value: 0},
	{label: 'Rating', value: 1},
	{label: 'Age', value: 2},
	{label: 'Interests', value: 3}]

const sort = [{label: 'Ascending', value:'ASC'},
	{label: 'Descending', value: 'DESC'}]

const selectTrueFalse = [{label: 'Yes', value: true},
	{label: 'No', value: false},
	{label: 'No matter', value: null}]

const genders = [{value: 'male', label: 'Male'} ,
	{value: 'female', label: 'Female'},
	{label: 'No matter', value: null}]

const age = ref<[number, number]>([getProfileParams.value.search.minAge ? getProfileParams.value.search.minAge : 18, getProfileParams.value.search.maxAge ? getProfileParams.value.search.maxAge : 99])
const rating = ref<[number, number]>([getProfileParams.value.search.minFameRating ? getProfileParams.value.search.minFameRating : 0, getProfileParams.value.search.maxFameRating ? getProfileParams.value.search.maxFameRating : 100])

const GetProfile = async () => {
	getProfileParams.value.search.minAge = Math.min(age.value[0], age.value[1])
	getProfileParams.value.search.maxAge = Math.max(age.value[0], age.value[1])
	getProfileParams.value.search.minFameRating = Math.min(rating.value[0], rating.value[1])
	getProfileParams.value.search.maxFameRating = Math.max(rating.value[0], rating.value[1])

	await axios.post('api/profiles', getProfileParams.value)
	.catch((res) => {
		if (res.code == 403) {
			message.error(`Fill out the profile!`);
		}
		else {
			message.error('Error')
		}
	}).then((res) => {
		if (res?.data){
			profiles.value = res.data.profiles
			if (getProfileParams.value.pagination.pageSize) {
				getProfileParams.value.pagination.total = getProfileParams.value.pagination.pageSize * res.data.amountOfPages
			}
		}
	})
}

const interests = ref<Interests[]>([])
const GetInterests = async () => {
	await axios.get('api/profiles/interests').then((res) => {
		interests.value = res.data
		interests.value.forEach((element) => {
			element.value = element.name
		})
	})
}
onMounted(async () => {
	await GetInterests()
})

watch (
	() => getFilters.value,
	async () => {
		age.value[0] = getFilters.value.minAge
		age.value[1] = getFilters.value.maxAge
		rating.value[0] = getFilters.value.minFameRating
		rating.value[1] = getFilters.value.maxFameRating
	}
)



</script>

<template>
	<div id="input-params-get-profile-collapse-main">
		<a-collapse>
			<a-collapse-panel key="1" header="Filter and sort">

					<div id="input-form-get-profile">
						<a-button id="button-get-profile" type="primary" html-type="signup" @click="GetProfile">Search</a-button>

						<a-form
						:label-col="{ span: 10 }"
						:wrapper-col="{ span: 30 }"
						layout="horizontal"
						:disabled="false"
						id="input-form-get-profile-check-box"
						>
						<a-form-item label="Sexual preferences">
							<a-select
							v-model:value="getProfileParams.search.sexualPreferences"
							:options="genders"
							size="middle"
							placeholder="Please select"
							></a-select>
						</a-form-item>
						<a-form-item label="Is matched">
							<a-select
							v-model:value="getProfileParams.search.isMatched"
							:options="selectTrueFalse"
							size="middle"
							placeholder="Please select"
							></a-select>
						</a-form-item>
						<a-form-item label="Is liked user">
							<a-select
							v-model:value="getProfileParams.search.isLikedUser"
							:options="selectTrueFalse"
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
						id="input-form-get-profile-middle"
						>
						<a-form-item label="Interests" direction="vertical">
								<a-select
								v-model:value="getProfileParams.search.commonTags"
								:options="interests"
								mode="tags"
								size="middle"
								placeholder="Please select"
								></a-select>
						</a-form-item>
						<a-form-item label="Distance">
								<a-slider v-model:value="getProfileParams.search.maxDistance" :min=getFilters?.minDistance :max=getFilters?.maxDistance />
							</a-form-item>
							<a-form-item label="Age">
								<a-slider v-model:value="age" range :min=getFilters?.minAge :max=getFilters?.maxAge />
							</a-form-item>
							<a-form-item label="Rating">
								<a-slider v-model:value="rating" range :min=getFilters?.minFameRating :max=getFilters?.maxFameRating />
							</a-form-item>
						</a-form>
						<a-form
						:label-col="{ span: 10 }"
						:wrapper-col="{ span: 15 }"
						layout="horizontal"
						:disabled="false"
						id="input-form-get-profile-sort"
						>
						<a-form-item label="Sort location">
							<a-select
								v-model:value="getProfileParams.sort.sortLocation"
								:options="sort"
								size="middle"
								placeholder="Please select"
								></a-select>
						</a-form-item>
						<a-form-item label="Sort fame rating">
							<a-select
								v-model:value="getProfileParams.sort.sortFameRating"
								:options="sort"
								size="middle"
								placeholder="Please select"
								></a-select>
						</a-form-item>
						<a-form-item label="Sort age">
							<a-select
								v-model:value="getProfileParams.sort.sortAge"
								:options="sort"
								size="middle"
								placeholder="Please select"
								></a-select>
						</a-form-item>
						<a-form-item label="Sort common tags">
							<a-select
								v-model:value="getProfileParams.sort.sortCommonTags"
								:options="sort"
								size="middle"
								placeholder="Please select"
								></a-select>
						</a-form-item>
						<a-form-item label="Sorting main parameter">
							<a-select
								v-model:value="getProfileParams.sort.sortingMainParameter"
								:options="sortMainParam"
								size="middle"
								placeholder="Please select"
								></a-select>
						</a-form-item>
						</a-form>
					</div>


			</a-collapse-panel>
		</a-collapse>
	</div>




</template>

<style>
#input-params-get-profile-collapse-main {
	position: relative;
	margin-top: 7vh;
	background-color: var(--color-background);
	color: var(--color-text)
}

.css-dev-only-do-not-override-16pw25h.ant-collapse>.ant-collapse-item >.ant-collapse-header {
	color: var(--color-text)
}

.css-dev-only-do-not-override-16pw25h.ant-collapse .ant-collapse-content {
	background-color: var(--color-background-soft);
}

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

#input-form-get-profile-check-box {
	width: 30vw;
}

#input-form-get-profile-middle {
	width: 30vw;
}

#input-form-get-profile-sort {
	width: 30vw;
}

#button-get-profile {
	position: relative;
	margin-left: 1vw;
	z-index: 1;
}

@media screen and (max-width: 1100px) {
	#input-form-get-profile-check-box {
		width: 100%;
	}

	#input-form-get-profile-middle {
		width: 100%;
	}

	#input-form-get-profile-sort {
		width: 100%;
	}

	.css-dev-only-do-not-override-16pw25h.ant-form-item .ant-form-item-label >label {
		font-size: 0.7em;;
	}

}

</style>
