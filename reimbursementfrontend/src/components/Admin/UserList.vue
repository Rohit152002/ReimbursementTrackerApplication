<template>
    <div class="w-full flex items-center justify-center">
        <div class="w-3/4">
            <h2>User Details</h2>
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Department</th>
                        <th>Gender</th>
                        <th>Edit</th>
                        <th>Delete</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for=" item in employeeData" :key="item.id">
                        <td>{{ item.id }}</td>
                        <td>{{ item.userName }}</td>
                        <td>{{ item.email }}</td>
                        <td>{{ item.departmentName }}</td>
                        <td>{{ item.genderName }}</td>
                        <td><i class='bx bxs-edit-alt'></i></td>
                        <td><i class='bx bxs-x-circle'></i></td>
                    </tr>
                </tbody>
            </table>
            <div class="pagination">
                <button @click="previousPage" :disabled="currentPage === 1">Previous</button>
                <span>Page {{ currentPage }} of {{ totalPages }}</span>
                <button @click="nextPage" :disabled="currentPage === totalPages">Next</button>
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
            employeeData: [], // Employee and Manager details
            currentPage: 1, // Current page number
            pageSize: 10, // Number of items per page
            totalPages: 1, // Total number of pages
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
                this.totalPages = Math.ceil(data.data.totalCount / this.pageSize);
            } catch (error) {
                console.error("Error fetching user data:", error);
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
