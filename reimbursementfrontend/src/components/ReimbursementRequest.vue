<template>
    <div class="min-h-screen bg-gray-50 p-6">
        <div class="flex justify-between items-center bg-white p-4 shadow rounded-lg">
            <h2 class="text-xl font-semibold text-gray-800">Welcome, {{ username }}</h2>
            <button @click="goToNewRequest"
                class="px-4 py-2 bg-blue-600 text-white font-medium rounded hover:bg-blue-700 transition">
                + Add New Request
            </button>
        </div>

        <div class="mt-4 flex justify-between items-center">
            <input v-model="searchQuery" @input="filterRequests" placeholder="Search requests..."
                class="border border-gray-300 rounded-lg p-2 w-1/2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
            <div class="flex gap-4">

                <button @click="toggleStatusFilter('Pending')" :class="getStatusButtonClass('Pending')">
                    Pending
                </button>
                <button @click="toggleStatusFilter('Passed')" :class="getStatusButtonClass('Passed')">
                    Passed
                </button>
                <button @click="toggleStatusFilter('Rejected')" :class="getStatusButtonClass('Rejected')">
                    Rejected
                </button>

            </div>

        </div>

        <div class="overflow-x-auto mt-4 bg-white shadow rounded-lg">
            <table class="w-full table-auto">
                <thead class="bg-gray-200 text-gray-700">
                    <tr>
                        <th class="px-4 py-2 text-left">Policy Name</th>
                        <th class="px-4 py-2 text-left">Total Amount</th>
                        <th class="px-4 py-2 text-left">Title</th>
                        <th class="px-4 py-2 text-left">Status</th>
                        <th class="px-4 py-2 text-left">Stage</th>
                        <th class="px-4 py-2 text-left">Reqeusted At</th>
                        <th class="px-4 py-2 text-left">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="request in filteredRequests" :key="request.id" class="hover:bg-gray-50">
                        <td class="px-4 py-2">{{ request.policyName }}</td>
                        <td class="px-4 py-2">{{ request.totalAmount }}</td>
                        <td class="px-4 py-2">{{ request.comments }}</td>
                        <td class="px-4 py-2 flex justify-center items-center">
                            <span :class="{
                                'bg-yellow-100 text-yellow-700 ': request.statusName === 'Pending',
                                'bg-green-100 text-green-700': request.statusName === 'Passed',
                                'bg-red-100 text-red-700': request.statusName === 'Rejected'
                            }" class="px-2 py-1 text-sm rounded text-center block w-24
                            ">
                                {{ request.statusName }}
                            </span>
                        </td>
                        <td class="px-4 py-2">{{ request.stageName }}</td>
                        <td class="px-4 py-2">

                            {{ new Date(request.dateTime).toLocaleDateString() }}
                        </td>
                        <td class="px-4 py-2">
                            <button @click="viewDetails(request.id)"
                                class="px-3 py-1 text-sm text-white bg-blue-600 rounded hover:bg-blue-700">
                                View Details
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>

            <div v-if="filteredRequests.length === 0" class="text-center text-gray-600 py-6">
                No data available
            </div>
        </div>

        <div class="flex justify-between items-center mt-4">
            <button @click="changePage(-1)" :disabled="pageNumber === 1"
                class="px-4 py-2 bg-gray-300 text-gray-600 rounded disabled:opacity-50">
                Previous
            </button>
            <span class="text-gray-700">Page {{ pageNumber }}</span>
            <button @click="changePage(1)" :disabled="pageNumber === totalPages"
                class="px-4 py-2 bg-gray-300 text-gray-600 rounded disabled:opacity-50">
                Next
            </button>
        </div>
    </div>
</template>

<script>
import { getRequestByUserId } from "@/scripts/Request";
import { jwtDecode } from "jwt-decode";
import { useToggleStore } from "@/store/toggle";
import router from "@/scripts/Route";

export default {
    name: "ReimbursementRequest",
    data() {
        return {
            requestData: [],
            filteredRequests: [],
            username: "",
            pageNumber: 1,
            pageSize: 2,
            searchQuery: "",
            sortKey: "policyName",
            totalPages: 1,
            statusFilter: [],
            toggleSubmitForm: useToggleStore()
        };
    },
    methods: {
        async getRequestsByID() {
            try {
                const token = jwtDecode(sessionStorage.getItem("token"));
                console.log(token);

                const id = +token["Id"];
                this.username = token.given_name;

                const result = await getRequestByUserId(id, this.pageNumber, this.pageSize);
                console.log(result);

                this.requestData = result.data.data.sort((a, b) => {
                    const dateA = new Date(a.dateTime);
                    const dateB = new Date(b.dateTime);

                    // Sorting in descending order
                    return dateB - dateA;
                });
                this.filteredRequests = [...this.requestData];
                this.totalPages = result.data.totalPages;
            } catch (err) {
                console.error(err);
            }
        },
        toggleStatusFilter(status) {
            if (this.statusFilter.includes(status)) {
                this.statusFilter = this.statusFilter.filter((s) => s !== status);
            } else {
                this.statusFilter.push(status);
            }
            this.filterRequests();
        },
        filterRequests() {
            const query = this.searchQuery.toLowerCase();
            this.filteredRequests = this.requestData.filter((req) => {
                const matchesSearch = req.policyName.toLowerCase().includes(query) ||
                    req.comments.toLowerCase().includes(query) ||
                    req.statusName.toLowerCase().includes(query);

                const matchesStatus =
                    this.statusFilter.length === 0 || this.statusFilter.includes(req.statusName);

                return matchesSearch && matchesStatus;
            });
        },
        getStatusButtonClass(status) {
            const baseClass =
                "px-4 py-2 font-medium rounded transition";
            const activeClass =
                "bg-opacity-100 text-white underline decoration-double";
            const inactiveClass =
                "bg-opacity-50 text-gray-700 hover:bg-opacity-75";

            const colorClass = {
                Pending: "bg-yellow-500",
                Passed: "bg-green-500",
                Rejected: "bg-red-500",
            }[status];

            return `${baseClass} ${colorClass} ${this.statusFilter.includes(status) ? activeClass : inactiveClass
                }`;
        },
        sortRequests(key) {
            this.filteredRequests.sort((a, b) => {

                if (key === "totalAmount") {

                    return a[key] > b[key] ? -1 : 1
                }
                return a[key] > b[key] ? 1 : -1
            }
            );
        },
        changePage(direction) {
            this.pageNumber += direction;
            this.getRequestsByID();
        },
        viewDetails(requestId) {
            router.push(`/request/${requestId}`)

        },
        goToNewRequest() {
            this.toggleSubmitForm.toggle();
        },
    },
    computed: {
        isToggled() {
            return this.toggleSubmitForm.isToggled;
        }
    },
    async mounted() {
        await this.getRequestsByID();
    },
};
</script>
