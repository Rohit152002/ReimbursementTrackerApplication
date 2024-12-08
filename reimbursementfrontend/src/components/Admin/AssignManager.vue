<template>
    <div class="w-full flex items-center justify-center p-6 bg-gray-100 min-h-screen">
        <div class="w-3/4 bg-white p-6 shadow-md rounded-lg">
            <h2 class="text-xl font-bold mb-4">User Details Without a Manager</h2>

            <!-- Search input -->
            <div class="mb-4">
                <input v-model="searchQuery" type="text" placeholder="Search by Name or Email..."
                    class="px-4 py-2 border border-gray-300 rounded-md w-full sm:w-1/3 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                    @input="filterData" />
            </div>

            <!-- Table -->
            <table class="w-full border-collapse border border-gray-300 text-left">
                <thead>
                    <tr class="bg-gray-100">
                        <th class="border border-gray-300 px-4 py-2">ID</th>
                        <th class="border border-gray-300 px-4 py-2">Name</th>
                        <th class="border border-gray-300 px-4 py-2">Email</th>
                        <th class="border border-gray-300 px-4 py-2">Department</th>
                        <th class="border border-gray-300 px-4 py-2">Gender</th>
                        <th class="border border-gray-300 px-4 py-2 text-center">Assign Manager</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in filteredEmployeeData" :key="item.id" class="hover:bg-gray-50 transition-all">
                        <td class="border border-gray-300 px-4 py-2">{{ item.id }}</td>
                        <td class="border border-gray-300 px-4 py-2">{{ item.userName }}</td>
                        <td class="border border-gray-300 px-4 py-2">{{ item.email }}</td>
                        <td class="border border-gray-300 px-4 py-2">{{ item.departmentName }}</td>
                        <td class="border border-gray-300 px-4 py-2">{{ item.genderName }}</td>
                        <td class="border border-gray-300 px-4 py-2 text-center">
                            <RouterLink :to="`/admin/user/${item.id}`" class="text-indigo-600 hover:text-indigo-500">
                                <i class="bx bxs-edit-alt text-lg"></i>
                            </RouterLink>
                        </td>
                    </tr>
                </tbody>
            </table>

            <!-- Pagination -->
            <div class="flex justify-between items-center mt-4">
                <button @click="previousPage" :disabled="currentPage === 1"
                    class="px-4 py-2 bg-gray-300 text-gray-700 rounded-md shadow-md hover:bg-gray-200 disabled:opacity-50">
                    Previous
                </button>
                <span class="text-gray-700">Page {{ currentPage }} of {{ totalPages }}</span>
                <button @click="nextPage" :disabled="currentPage === totalPages"
                    class="px-4 py-2 bg-gray-300 text-gray-700 rounded-md shadow-md hover:bg-gray-200 disabled:opacity-50">
                    Next
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import { getUserWithNoManager } from '@/scripts/Employee';

export default {
    name: "AssignManager",

    data() {
        return {
            employeeData: [],
            filteredEmployeeData: [],
            searchQuery: "",
            currentPage: 1,
            pageSize: 10,
            totalPages: 1,
        };
    },
    async mounted() {
        await this.fetchUserData();
    },
    methods: {
        async fetchUserData() {
            try {
                const data = await getUserWithNoManager(this.currentPage, this.pageSize);
                this.employeeData = data.data.data.filter((r) => r.department !== 7);
                this.totalPages = Math.ceil(data.data.totalCount / this.pageSize);
                this.filteredEmployeeData = this.employeeData;
            } catch (error) {
                console.error("Error fetching user data:", error);
            }
        },

        filterData() {
            if (this.searchQuery === "") {
                this.filteredEmployeeData = this.employeeData;
            } else {
                this.filteredEmployeeData = this.employeeData.filter(item => {
                    const queryLower = this.searchQuery.toLowerCase();
                    return (
                        item.userName.toLowerCase().includes(queryLower) ||
                        item.email.toLowerCase().includes(queryLower)
                    );
                });
            }
        },

        async nextPage() {
            if (this.currentPage < this.totalPages) {
                this.currentPage++;
                await this.fetchUserData();
            }
        },
        async previousPage() {
            if (this.currentPage > 1) {
                this.currentPage--;
                await this.fetchUserData();
            }
        },
    },
};
</script>

<style scoped></style>
