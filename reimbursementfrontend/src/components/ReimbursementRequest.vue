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
            <select v-model="sortKey" @change="sortRequests"
                class="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500">
                <option value="policyName">Policy Name</option>
                <option value="totalAmount">Total Amount</option>
                <option value="statusName">Status</option>
            </select>
        </div>

        <div class="overflow-x-auto mt-4 bg-white shadow rounded-lg">
            <table class="w-full table-auto">
                <thead class="bg-gray-200 text-gray-700">
                    <tr>
                        <th class="px-4 py-2 text-left">Policy Name</th>
                        <th class="px-4 py-2 text-left">Total Amount</th>
                        <th class="px-4 py-2 text-left">Comments</th>
                        <th class="px-4 py-2 text-left">Status</th>
                        <th class="px-4 py-2 text-left">Stage</th>
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
    <!-- <div v-if="isToggled"
        class="absolute top-0 left-0 w-full h-full bg-gray-800 bg-opacity-50 flex items-center justify-center">
        <div class="bg-white w-3/4 max-w-lg p-6 rounded-lg shadow-lg relative">
            <button @click="goToNewRequest" class="absolute top-3 right-3 text-gray-500 hover:text-gray-800">
                ✖
            </button>
            <ReimbursementRequestForm />
        </div>
    </div> -->

</template>

<script>
import { getRequestByUserId } from "@/scripts/Request";
import { jwtDecode } from "jwt-decode";
// import ReimbursementRequestForm from "./Form/ReimbursementRequestForm.vue";
import { useToggleStore } from "@/store/toggle";
import router from "@/scripts/Route";

export default {
    name: "ReimbursementRequest",
    components: {
        // ReimbursementRequestForm
    },
    data() {
        return {
            requestData: [],
            filteredRequests: [],
            username: "",
            pageNumber: 1,
            pageSize: 10,
            searchQuery: "",
            sortKey: "policyName",
            totalPages: 1,
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

                this.requestData = result.data.data;
                this.filteredRequests = [...this.requestData];
                this.totalPages = result.data.totalPages;
            } catch (err) {
                console.error(err);
            }
        },
        filterRequests() {
            const query = this.searchQuery.toLowerCase();
            this.filteredRequests = this.requestData.filter(
                (req) =>
                    req.policyName.toLowerCase().includes(query) ||
                    req.comments.toLowerCase().includes(query) ||
                    req.statusName.toLowerCase().includes(query)
            );
        },
        sortRequests() {
            this.filteredRequests.sort((a, b) => {

                if (this.sortKey === "totalAmount") {

                    return a[this.sortKey] > b[this.sortKey] ? -1 : 1
                }
                return a[this.sortKey] > b[this.sortKey] ? 1 : -1
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
            // alert("Navigate to Add New Request Page");
            this.toggleSubmitForm.toggle();
            // Implement navigation logic to the add request page
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

<style>
/* No additional styles; everything is handled by Tailwind CSS */
</style>
