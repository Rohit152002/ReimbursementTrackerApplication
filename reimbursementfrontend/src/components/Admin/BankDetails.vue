<template>
    <div class=" mx-auto p-4">
        <h1 class="text-2xl font-bold text-center mb-6">Bank Account Details</h1>

        <!-- Search Bar -->
        <input type="text" v-model="searchQuery" placeholder="Search..."
            class="w-full p-3 border border-gray-300 rounded mb-4 focus:outline-none focus:ring-2 focus:ring-blue-500" />

        <!-- Data Table -->
        <div v-if="filteredData" class="overflow-x-auto">
            <table class="min-w-full border-collapse border border-gray-200">
                <thead class="bg-gray-100">
                    <tr>
                        <th class="border border-gray-200 px-4 py-2 text-left">#</th>
                        <th class="border border-gray-200 px-4 py-2 text-left">User Name</th>
                        <th class="border border-gray-200 px-4 py-2 text-left">Email</th>
                        <th class="border border-gray-200 px-4 py-2 text-left">Department</th>
                        <th class="border border-gray-200 px-4 py-2 text-left">Gender</th>
                        <th class="border border-gray-200 px-4 py-2 text-left">Account Number</th>
                        <th class="border border-gray-200 px-4 py-2 text-left">Branch Name</th>
                        <th class="border border-gray-200 px-4 py-2 text-left">IFSC Code</th>
                        <th class="border border-gray-200 px-4 py-2 text-left">Branch Address</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in filteredData" :key="item.id" class="hover:bg-gray-50">
                        <td class="border border-gray-200 px-4 py-2">{{ index + 1 }}</td>
                        <td class="border border-gray-200 px-4 py-2">{{ item.user.userName }}</td>
                        <td class="border border-gray-200 px-4 py-2">{{ item.user.email }}</td>
                        <td class="border border-gray-200 px-4 py-2">{{ item.user.departmentName }}</td>
                        <td class="border border-gray-200 px-4 py-2">{{ item.user.genderName }}</td>
                        <td class="border border-gray-200 px-4 py-2">{{ item.accNo }}</td>
                        <td class="border border-gray-200 px-4 py-2">{{ item.branchName }}</td>
                        <td class="border border-gray-200 px-4 py-2">{{ item.ifscCode }}</td>
                        <td class="border border-gray-200 px-4 py-2">{{ item.branchAddress }}</td>
                    </tr>
                    <tr v-if="filteredData.length === 0">
                        <td colspan="9" class="border border-gray-200 px-4 py-2 text-center text-gray-500 italic">
                            No data found.
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
import { getAllBanksDetails } from '@/scripts/Bank';

export default {
    data() {
        return {
            searchQuery: "", // For search input
            response: {},
        };
    },
    computed: {
        // Filter data based on the search query
        filteredData() {
            if (!this.searchQuery) return this.response.data;

            const query = this.searchQuery.toLowerCase();
            return this.response.data.filter((item) => {
                return (
                    item.user.userName.toLowerCase().includes(query) ||
                    item.user.email.toLowerCase().includes(query) ||
                    item.user.departmentName.toLowerCase().includes(query) ||
                    item.user.genderName.toLowerCase().includes(query) ||
                    item.accNo.toLowerCase().includes(query) ||
                    item.branchName.toLowerCase().includes(query) ||
                    item.ifscCode.toLowerCase().includes(query) ||
                    item.branchAddress.toLowerCase().includes(query)
                );
            });
        },
    },
    methods:
    {
        async getAllBanks() {
            const res = await getAllBanksDetails(1, 10);
            console.log(res);
            this.response = res.data
        }
    },
    async mounted() {
        await this.getAllBanks();
    }
};
</script>

<style>
/* No additional CSS needed as Tailwind handles everything */
</style>
