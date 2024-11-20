<template>

    <div class="w-full flex items-center justify-center">
        <div class="w-3/4">

            <h2>User Details Who have no manager</h2>
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Department</th>
                        <th>Gender</th>
                        <th>Assign Manager</th>
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

                        <td class="flex items-center justify-center">
                            <RouterLink :to="`/user/${item.id}`">
                                <i class='bx bxs-edit-alt'></i>
                            </RouterLink>
                        </td>

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
import { getUserWithNoManager } from '@/scripts/Employee';

// import "@coreui/coreui/dist/css/coreui.min.css";
// import "bootstrap/dist/css/bootstrap.min.css";


export default {
    name: "AssignManager",

    data() {

        return {
            visibleLiveDemo: false,
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
                const data = await getUserWithNoManager(this.currentPage, this.pageSize);
                this.employeeData = data.data.data.filter((r) => r.department != 7);
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

<style scoped></style>
