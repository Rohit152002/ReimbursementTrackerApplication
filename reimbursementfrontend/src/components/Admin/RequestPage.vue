<template>
    <div class="p-6 bg-gray-100 min-h-screen w-full">
        <div class="max-w-3xl mx-auto">
            <h1 class="text-2xl font-bold mb-6">Reimbursement Requests</h1>

            <!-- Filters -->
            <div class="flex gap-4 mb-4">
                <div>
                    <label for="statusFilter" class="block text-sm font-medium">Filter by Status</label>
                    <select id="statusFilter" v-model="filters.status"
                        class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500">
                        <option value="">All</option>
                        <option value="0">Passed</option>
                        <option value="1">Pending</option>
                        <option value="2">Rejected</option>
                    </select>
                </div>
                <div>
                    <label for="stageFilter" class="block text-sm font-medium">Filter by Stage</label>
                    <select id="stageFilter" v-model="filters.stage"
                        class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500">
                        <option value="">All</option>
                        <option value="0">Manager</option>
                        <option value="1">HR</option>
                        <option value="2">Financial</option>
                    </select>
                </div>
                <button @click="applyFilters"
                    class="mt-auto px-4 py-2 bg-indigo-600 text-white rounded shadow hover:bg-indigo-500">
                    Apply Filters
                </button>
            </div>

            <!-- Requests -->
            <div v-if="requests.length" class="space-y-4">
                <div v-for="request in filteredRequests" :key="request.id" class="bg-white p-4 rounded-lg shadow-md">
                    <div class="flex justify-between items-center">
                        <h2 class="font-semibold text-lg">Request ID: {{ request.id }}</h2>
                        <button @click="toggleAccordion(request.id)" class="text-indigo-600 hover:underline">
                            {{ activeRequest === request.id ? "Hide Details" : "View Details" }}
                        </button>
                    </div>
                    <div>
                        <p><strong>Policy:</strong> {{ request.policyName }}</p>
                        <p><strong>Total Amount:</strong> Rs {{ request.totalAmount }}</p>
                        <p><strong>Status:</strong> {{ request.statusName }}</p>
                        <p><strong>Stage:</strong> {{ request.stageName }}</p>
                    </div>

                    <div v-if="activeRequest === request.id" class="mt-4">
                        <h3 class="font-semibold text-md">Submitted by:</h3>
                        <p>{{ request.user.userName }} ({{ request.user.email }})</p>
                        <p><strong>Department:</strong> {{ request.user.departmentName }}</p>
                        <p><strong>Comments:</strong> {{ request.comments }}</p>

                        <h4 class="mt-2 font-semibold">Items:</h4>
                        <table class="w-full text-left border-collapse border border-gray-300 mt-2">
                            <thead>
                                <tr class="bg-gray-100">
                                    <th class="border border-gray-300 px-2 py-1">ID</th>
                                    <th class="border border-gray-300 px-2 py-1">Category</th>
                                    <th class="border border-gray-300 px-2 py-1">Amount</th>
                                    <th class="border border-gray-300 px-2 py-1">Description</th>
                                    <th class="border border-gray-300 px-2 py-1">Receipt</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="item in request.items" :key="item.id">
                                    <td class="border border-gray-300 px-2 py-1">{{ item.id }}</td>
                                    <td class="border border-gray-300 px-2 py-1">{{ item.categoryName }}</td>
                                    <td class="border border-gray-300 px-2 py-1">Rs{{ item.amount }}</td>
                                    <td class="border border-gray-300 px-2 py-1">{{ item.description }}</td>
                                    <td class="border border-gray-300 px-2 py-1">
                                        <a :href="`http://localhost:5286/File/${item.receiptFile}`" target="_blank"
                                            class="text-indigo-600 hover:underline">
                                            View Receipt
                                        </a>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <!-- Pagination -->
            <div class="flex justify-between items-center mt-6">
                <button @click="previousPage" :disabled="currentPage === 1"
                    class="px-4 py-2 bg-gray-300 rounded shadow hover:bg-gray-200">
                    Previous
                </button>
                <span>Page {{ currentPage }} of {{ totalPages }}</span>
                <button @click="nextPage" :disabled="currentPage === totalPages"
                    class="px-4 py-2 bg-gray-300 rounded shadow hover:bg-gray-200">
                    Next
                </button>
            </div>
        </div>
    </div>
</template>


<script>
import { getRequests } from '@/scripts/Request';

export default {
    name: "RequestPage",
    data() {
        return {
            requests: [], // Array of all requests
            activeRequest: null, // ID of the currently expanded accordion
            currentPage: 1, // Current page number
            pageSize: 10, // Number of items per page
            totalPages: 1, // Total number of pages
            filters: {
                status: "", // Filter for request status
                stage: "", // Filter for request stage
            },
            filteredRequests: [], // Array for filtered requests
        }
    },
    methods: {
        async fetchRequestData() {
            try {
                const response = await getRequests(this.currentPage, this.pageSize);
                this.requests = response.data.data;
                this.totalPages = response.data.totalPages;
                this.filteredRequests = this.requests; // Initialize filtered requests
            } catch (err) {
                console.error("Error fetching requests:", err);
            }
        },
        toggleAccordion(id) {
            this.activeRequest = this.activeRequest === id ? null : id;
        },
        applyFilters() {
            this.filteredRequests = this.requests.filter((request) => {
                const statusMatches =
                    this.filters.status === "" || request.status == this.filters.status;
                const stageMatches =
                    this.filters.stage === "" || request.stage == this.filters.stage;
                return statusMatches && stageMatches;
            });
        },
        async nextPage() {
            if (this.currentPage < this.totalPages) {
                this.currentPage++;
                await this.fetchRequestData();
            }
        },
        async previousPage() {
            if (this.currentPage > 1) {
                this.currentPage--;
                await this.fetchRequestData();
            }
        },
    },
    async mounted() {
        await this.fetchRequestData();
    },
}
</script>

<style>
.accordion {
    border: 1px solid #ddd;
    margin-bottom: 10px;
    border-radius: 5px;
    overflow: hidden;
}

.accordion-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px;
    background-color: #f5f5f5;
    cursor: pointer;
}

.accordion-header:hover {
    background-color: #eaeaea;
}

.accordion-body {
    padding: 10px;
    background-color: #fff;
}

table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 10px;
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
</style>
