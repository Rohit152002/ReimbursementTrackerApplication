<template>
    <div class="w-full flex items-center mt-8 justify-center">
        <div class="w-3/4 bg-white shadow-md rounded-lg p-6">
            <h2 class="text-2xl font-semibold mb-4 text-gray-800">User Details</h2>
            <div class="flex justify-between items-center pb-4">
                <input type="text" v-model="searchQuery" @input="onSearchInput"
                    placeholder="Search by Name or Department"
                    class="w-1/3 p-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500" />
            </div>
            <table class="w-full border border-gray-200 rounded-lg overflow-hidden">
                <thead class="bg-gray-100 text-gray-700">
                    <tr>
                        <th class="py-3 px-4 text-left font-medium">ID</th>
                        <th class="py-3 px-4 text-left font-medium">Name</th>
                        <th class="py-3 px-4 text-left font-medium">Email</th>
                        <th class="py-3 px-4 text-left font-medium">Department</th>
                        <th class="py-3 px-4 text-left font-medium">Gender</th>
                        <!-- <th class="py-3 px-4 text-left font-medium">Edit</th>
                        <th class="py-3 px-4 text-left font-medium">Delete</th> -->
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in filteredEmployeeData" :key="item.id"
                        class="border-t border-gray-200 hover:bg-gray-50">
                        <td class="py-2 px-4">{{ index + 1 }}</td>
                        <td class="py-2 px-4">{{ item.userName }}</td>
                        <td class="py-2 px-4">{{ item.email }}</td>
                        <td class="py-2 px-4">{{ item.departmentName }}</td>
                        <td class="py-2 px-4">{{ item.genderName }}</td>
                        <!-- <td class="py-2 px-4 text-center text-blue-500 cursor-pointer">
                            <i class='bx bxs-edit-alt'></i>
                        </td>
                        <td @click="deleteUser(item.id)" class="py-2 px-4 text-center text-red-500 cursor-pointer">
                            <i class='bx bxs-x-circle'></i>
                        </td> -->
                    </tr>
                </tbody>
            </table>
            <div class="flex justify-between items-center mt-6">
                <button @click="previousPage" :disabled="currentPage === 1"
                    class="px-4 py-2 bg-blue-500 text-white rounded-lg shadow hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed">
                    Previous
                </button>
                <span class="text-gray-600">Page {{ currentPage }} of {{ totalPages }}</span>
                <button @click="nextPage" :disabled="currentPage === totalPages"
                    class="px-4 py-2 bg-blue-500 text-white rounded-lg shadow hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed">
                    Next
                </button>
            </div>
        </div>
    </div>
</template>


<script>
import { getAllUser } from '@/scripts/User';

export default {
    name: "UserList",
    data() {

        return {
            employeeData: [],
            filteredEmployeeData: [],
            currentPage: 1,
            pageSize: 10,
            totalPages: 1,
            searchQuery: "",

        };
    },
    async mounted() {
        await this.fetchUserData()
    },
    methods: {
        async fetchUserData() {
            try {
                const data = await getAllUser(this.currentPage, this.pageSize);
                this.employeeData = data.data.data;
                this.filteredEmployeeData = this.employeeData;

                this.totalPages = Math.ceil(data.data.totalCount / this.pageSize);
            } catch (error) {
                console.error("Error fetching user data:", error);
            }
        },
        onSearchInput() {

            const query = this.searchQuery.toLowerCase();

            if (query === "") {

                this.filteredEmployeeData = this.employeeData;
            } else {

                this.filteredEmployeeData = this.employeeData.filter(item => {
                    return (
                        item.userName.toLowerCase().includes(query) ||
                        item.departmentName.toLowerCase().includes(query)
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
}
</script>

<style scoped>
table {
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 20px;
}

table th,
table td {
    border: 1px solid #ddd;
    padding: 8px;
    text-align: left;
}

table th {
    background-color: #f4f4f4;
}

table tr:nth-child(even) {
    background-color: #f9f9f9;
}

.pagination {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

button {
    padding: 5px 10px;
    font-size: 14px;
    cursor: pointer;
}

button:disabled {
    cursor: not-allowed;
    opacity: 0.5;
}
</style>
