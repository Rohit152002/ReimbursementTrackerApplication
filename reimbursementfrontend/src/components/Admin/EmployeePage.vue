<template>
    <div class="mt-20 w-full px-28">
        <div class="flex gap-12 pb-28">

            <RouterLink to="assign" class="border-black border border-solid rounded-md px-8 py-4 w-fit">Assign Manager
            </RouterLink>
            <RouterLink to="users" class="border-black border border-solid rounded-md px-8 py-4 w-fit">User List
            </RouterLink>
        </div>
        <h2>Employee and Manager Details</h2>
        <table>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Employee Name</th>
                    <th>Employee Email</th>
                    <th>Employee Department</th>
                    <th>Manager Name</th>
                    <th>Manager Email</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in employeeData" :key="item.id">
                    <td>{{ item.id }}</td>
                    <td>{{ item.employee.userName }}</td>
                    <td>{{ item.employee.email }}</td>
                    <td>{{ item.employee.departmentName }}</td>
                    <td>{{ item.manager.userName }}</td>
                    <td>{{ item.manager.email }}</td>
                </tr>
            </tbody>
        </table>
        <div class="pagination">
            <button @click="previousPage" :disabled="currentPage === 1">Previous</button>
            <span>Page {{ currentPage }} of {{ totalPages }}</span>
            <button @click="nextPage" :disabled="currentPage === totalPages">Next</button>
        </div>
    </div>
</template>

<script>
import { getEmployee } from '@/scripts/Employee';


export default {
    name: "EmployeePage",
    data() {
        return {
            employeeData: [], // Employee and Manager details
            currentPage: 1, // Current page number
            pageSize: 10, // Number of items per page
            totalPages: 1, // Total number of pages
        };
    },
    async mounted() {
        try {
            await this.fetchEmployeeData();

        }
        catch (err) {
            console.log(err)
        }

    },
    methods: {
        async fetchEmployeeData() {
            try {
                const data = await getEmployee(this.currentPage, this.pageSize);
                this.employeeData = data.data.data;
                console.log(this.employeeData)
                this.totalPages = Math.ceil(data.data.totalCount / this.pageSize);
            } catch (error) {
                console.error("Error fetching employee data:", error);
            }
        },
        async nextPage() {
            if (this.currentPage < this.totalPages) {
                this.currentPage++;
                await this.fetchEmployeeData();
            }
        },
        async previousPage() {
            if (this.currentPage > 1) {
                this.currentPage--;
                await this.fetchEmployeeData();
            }
        },
    },
};
</script>

<style>
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
