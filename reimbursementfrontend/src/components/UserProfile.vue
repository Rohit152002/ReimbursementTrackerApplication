<template>
    <div class="max-w-md mx-auto mt-9` bg-white p-6 rounded-lg shadow-md">
        <h2 class="text-2xl font-bold text-gray-800 mb-4">User Profile</h2>

        <div class="space-y-4">
            <div class="flex">
                <p class="font-semibold text-gray-700">Name:</p>
                <p class="text-gray-600">{{ userName }}</p>
            </div>

            <div class="flex">
                <p class="font-semibold text-gray-700">Email:</p>
                <p class="text-gray-600">{{ email }}</p>
            </div>

            <div class='flex'>
                <p class="font-semibold text-gray-700">Department:</p>
                <p class="text-gray-600">{{ departmentName }}</p>
            </div>

            <div class="flex">
                <p class="font-semibold text-gray-700">Gender:</p>
                <p class="text-gray-600">{{ genderName }}</p>
            </div>
        </div>
    </div>
</template>

<script>
import { getUserProfile } from '@/scripts/User';

export default {
    name: "UserProfile",
    data() {
        return {
            userName: '',
            email: '',
            departmentName: '',
            genderName: '',
        }
    },
    async mounted() {
        const res = await getUserProfile();
        console.log(res);
        this.userName = res.data.userName;
        this.email = res.data.email;
        this.departmentName = res.data.departmentName;
        this.genderName = res.data.genderName;
    }
}
</script>

<style scoped></style>
