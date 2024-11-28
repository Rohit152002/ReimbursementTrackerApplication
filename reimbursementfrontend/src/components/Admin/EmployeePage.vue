<template>
    <div class="mt-20 w-full px-10">
        <!-- Navigation Buttons -->
        <div class="flex gap-12 pb-10">
            <RouterLink to="assign"
                class="border border-blue-500 rounded-md px-8 py-4 bg-blue-500 text-white hover:bg-blue-600 transition">
                Assign Manager
            </RouterLink>
            <RouterLink to="users"
                class="border border-blue-500 rounded-md px-8 py-4 bg-blue-500 text-white hover:bg-blue-600 transition">
                User List
            </RouterLink>
            <RouterLink to="/setup"
                class="border border-blue-500 rounded-md px-8 py-4 bg-blue-500 text-white hover:bg-blue-600 transition">
                Add Admin
            </RouterLink>
        </div>

        <!-- Table Heading -->
        <h2 class="text-2xl font-bold mb-6 text-gray-800">Employee and Manager Details</h2>

        <!-- Employee Table -->
        <div class="overflow-x-auto bg-white shadow-md rounded-md">
            <table class="min-w-full border border-gray-200">
                <thead class="bg-gray-100">
                    <tr>
                        <th class="px-4 py-2 border border-gray-200">ID</th>
                        <th class="px-4 py-2 border border-gray-200">Employee Name</th>
                        <th class="px-4 py-2 border border-gray-200">Employee Email</th>
                        <th class="px-4 py-2 border border-gray-200">Employee Department</th>
                        <th class="px-4 py-2 border border-gray-200">Manager Name</th>
                        <th class="px-4 py-2 border border-gray-200">Manager Email</th>
                        <!-- <th class="px-4 py-2 border border-gray-200">Reimbursement</th> -->
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in employeeData" :key="item.id" class="hover:bg-gray-50">
                        <td class="px-4 py-2 border border-gray-200">{{ item.id }}</td>
                        <td class="px-4 py-2 border border-gray-200 font-medium text-gray-800">
                            {{ item.employee.userName }}
                        </td>
                        <td class="px-4 py-2 border border-gray-200">{{ item.employee.email }}</td>
                        <td class="px-4 py-2 border border-gray-200">{{ item.employee.departmentName }}</td>
                        <td class="px-4 py-2 border border-gray-200">{{ item.manager.userName }}</td>
                        <td class="px-4 py-2 border border-gray-200">{{ item.manager.email }}</td>
                        <!-- <td class="px-4 py-2 border border-gray-200 text-center">
                            <button class="px-3 py-2 bg-green-500 text-white rounded hover:bg-green-600 transition"
                                @click="viewReimbursement(item.id)">
                                View Requests
                            </button>
                        </td> -->
                    </tr>
                    <tr v-if="employeeData.length === 0">
                        <td colspan="7" class="px-4 py-2 text-center text-gray-500 italic">
                            No employee data available.
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Pagination -->
        <div class="pagination flex justify-between items-center mt-6">
            <button class="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400 transition"
                @click="previousPage" :disabled="currentPage === 1">
                Previous
            </button>
            <span class="text-gray-700 font-medium">Page {{ currentPage }} of {{ totalPages }}</span>
            <button class="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400 transition" @click="nextPage"
                :disabled="currentPage === totalPages">
                Next
            </button>
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
        }, viewReimbursement(employeeId) {
            alert(`Viewing reimbursement requests for employee ID: ${employeeId}`);
            // Logic to navigate or fetch reimbursement requests can be added here
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
