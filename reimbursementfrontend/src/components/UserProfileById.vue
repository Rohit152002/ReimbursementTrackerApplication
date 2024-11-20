<template>
    <div class="p-6 bg-gray-100 min-h-screen w-full">
        <div class="max-w-2xl mx-auto bg-white shadow-md rounded-lg p-6">
            <h2 class="text-2xl font-semibold text-gray-800 mb-4">User Profile</h2>

            <!-- User Profile Details -->
            <div v-if="user" class="mb-6">
                <p class="text-gray-600 mb-2">
                    <span class="font-medium">Name:</span> {{ user.userName }}
                </p>
                <p class="text-gray-600 mb-2">
                    <span class="font-medium">Email:</span> {{ user.email }}
                </p>
                <p class="text-gray-600 mb-2">
                    <span class="font-medium">Gender:</span> {{ user.genderName }}
                </p>
                <p class="text-gray-600">
                    <span class="font-medium">Department:</span> {{ user.departmentName }}
                </p>
            </div>
            <div v-else>
                <p class="text-gray-600">Loading user profile...</p>
            </div>

            <!-- Assign Manager Section -->
            <h3 class="text-xl font-semibold text-gray-800 mb-4">Assign Manager</h3>
            <div class="mb-4">
                <label for="manager-select" class="block text-sm font-medium text-gray-700 mb-1">
                    Select Manager:
                </label>

                <!-- <VueSelect v-model="selectedManagerId" :options="managers" label="userName"
                    :reduce="manager => manager.id" placeholder="Search and select a manager" class="w-full" /> -->
                <VueSelect v-model="selectedManagerId" placeholder="Search and select a manager" :options="managers"
                    label="userName">
                    <!-- <template #list-footer>
                        <li v-show="hasNextPage" ref="load" class="loader">
                            Loading more options...
                        </li>
                    </template> -->
                </VueSelect>
            </div>

            <button @click="assignManager" :disabled="!selectedManagerId || !user"
                class="w-full bg-blue-500 text-white py-2 px-4 rounded-md hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed">
                Assign Manager
            </button>

            <!-- Status Message -->
            <p v-if="statusMessage" :class="{
                'text-green-500 mt-4': isSuccess,
                'text-red-500 mt-4': !isSuccess,
            }">
                {{ statusMessage }}
            </p>
        </div>
    </div>
</template>

<script>
import { assignManagerRequest, getEmployee } from '@/scripts/Employee';
import { getUserProfileById, searchUser } from '@/scripts/User';
// import { useDebounceFn } from "@vueuse/core"
import VueSelect from 'vue-select';

import "vue-select/dist/vue-select.css";


export default {
    name: "UserProfileById",
    components: {
        VueSelect
    },
    data() {
        return {
            user: {},
            managers: [], // List of available managers
            selectedManagerId: null, // Selected manager ID
            statusMessage: "", // Message for status updates
            limit: 1,
            page: 1,
            isSuccess: false, // Flag for success or error state
        }
    },

    async mounted() {
        try {
            await this.getUserProfile();
            await this.getManager();
        } catch (err) {
            console.log(err)
        }
    },
    methods:
    {
        async getUserProfile() {
            try {
                var result = await getUserProfileById(this.$route.params.id);
                console.log(result.data)
                this.user = result.data
            } catch (err) {
                console.log(err);
            }
        },

        async getManager(search = "A", page = this.page, limit = this.limit) {
            try {
                const result = await searchUser(search, page, limit);
                const employee= await getEmployee(page,limit)

                // Filter out the current user and sort the results
                this.managers = result.data.data
                    .filter((r) => r.id !== +this.user.id)
                    .sort();

                console.log("Filtered Managers:", this.managers);
            } catch (err) {
                console.error("Error fetching managers:", err);
            }
        },


        async assignManager() {
            try {
                event.preventDefault();
                var result = await assignManagerRequest(this.user.id, this.selectedManagerId.id)
                console.log(this.user.id, this.selectedManagerId.id);
                this.isSuccess = result.data.isSuccess
                this.statusMessage = result.data.message

            } catch (err) {
                console.log(err)
            }
        }
    }

}

</script>

<style scoped></style>
