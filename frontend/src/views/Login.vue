<script lang="ts" setup>
import { storeToRefs } from 'pinia';
import NamePasswordInput from '../components/NamePasswordInput.vue'
import SignUpForm from '@/components/SignUpForm.vue';
import { SignUpStore } from '@/stores/SignUpStore'

const IsActiveSignUp = storeToRefs(SignUpStore()).IsActiveSignUp

</script>


<template>
	<div class="Login" v-if="!IsActiveSignUp">
		<NamePasswordInput />
	</div>
	<div class="SignUpForm" v-if="IsActiveSignUp">
		<SignUpForm />
	</div>
</template>

<style>
.Login {
	position: relative;
	top: 15vh;
	right: 4vw;
}

.SignUpForm {
	position: relative;
	top: 10vh;
	right: 4vw;
}

</style>
