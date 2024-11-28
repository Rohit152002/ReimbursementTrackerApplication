<template>
    <div class="p-6 w-full mx-auto">
        <h1 class="text-2xl font-bold text-gray-800 mb-4">Approvals Page</h1>

        <!-- Search and Filters -->
        <div class="flex flex-col sm:flex-row gap-4 mb-6">
            <!-- Search Bar -->
            <input v-model="searchQuery" type="text" placeholder="Search approvals..."
                class="w-full sm:w-auto flex-1 px-4 py-2 border rounded-lg focus:ring focus:ring-blue-300"
                @input="filterApprovals" />

            <!-- Department Filter -->
            <select v-model="selectedDepartment" @change="filterApprovals"
                class="w-full sm:w-auto flex-1 px-4 py-2 border rounded-lg focus:ring focus:ring-blue-300">
                <option value="">All Departments</option>
                <option v-for="dept in departments" :key="dept.id" :value="dept.name">
                    {{ dept.name }}
                </option>
            </select>



            <!-- Stage Filter -->
            <select v-model="selectedStage" @change="filterApprovals"
                class="w-full sm:w-auto flex-1 px-4 py-2 border rounded-lg focus:ring focus:ring-blue-300">
                <option value="">All Stages</option>
                <option v-for="stage in stages" :key="stage.id" :value="stage.id">
                    {{ stage.name }}
                </option>
            </select>
        </div>

        <div v-for="request in uniqueRequests" :key="request.id" class="mb-4 border rounded-lg">
            <!-- Accordion Header -->
            <div @click="toggleAccordion(request.id)"
                class="flex justify-between items-center px-4 py-2 bg-gray-200 cursor-pointer">
                <div class="">
                    <span class="font-bold text-gray-700">Request ID:</span> {{ request.id }}
                    <span class="ml-4 font-bold text-gray-700">Total Amount:</span> {{ request?.totalAmount }}
                    <RouterLink v-if="request && request.id" :to="`request/${request.id}`"
                        class="bg-blue-400 ml-8 text-white px-4 py-2 rounded-lg"> View
                        Request</RouterLink>
                </div>
                <span :class="{ 'rotate-180': isAccordionOpen(request.id) }" class="transform transition-transform">
                    ▼
                </span>
            </div>

            <!-- Accordion Content -->
            <div v-if="isAccordionOpen(request.id)" class="px-4 py-2 bg-white">
                <table class="w-full table-auto text-left text-sm">
                    <thead class="bg-gray-100 text-gray-600">
                        <tr>
                            <th class="px-4 py-2">Approval ID</th>
                            <th class="px-4 py-2">Reviewer</th>
                            <th class="px-4 py-2">Comments</th>
                            <th class="px-4 py-2">Stage</th>
                            <th class="px-4 py-2">Status</th>

                            <th class="px-4 py-2">Review Date</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="approval in getApprovalsForRequest(request.id)" :key="approval.id"
                            class="hover:bg-gray-100">
                            <td class="border-t px-4 py-2">{{ approval.id }}</td>
                            <td class="border-t px-4 py-2">{{ approval.review.userName }}</td>
                            <td class="border-t px-4 py-2">{{ approval.comments }}</td>
                            <td class="border-t px-4 py-2">{{ approval.stageName }}</td>
                            <td class="border-t px-4 py-2">{{ approval.requestStageName }}</td>
                            <td class="border-t px-4 py-2">{{ formatDate(approval.reviewDate) }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <!-- Pagination Controls -->
        <div class="flex items-center justify-between mt-6">
            <button :disabled="pageNumber === 1" @click="prevPage"
                class="px-4 py-2 text-sm bg-blue-500 text-white rounded-lg disabled:bg-gray-300">
                Previous
            </button>
            <span class="text-gray-700">
                Page {{ pageNumber }} of {{ totalPages }}
            </span>
            <button :disabled="pageNumber === totalPages" @click="nextPage"
                class="px-4 py-2 text-sm bg-blue-500 text-white rounded-lg disabled:bg-gray-300">
                Next
            </button>
        </div>
    </div>
</template>

<script>
import { getAllApprovals } from "@/scripts/Approvals";

export default {
    name: "ApprovalsPage",
    data() {
        return {
            approvals: [],
            filteredApprovals: [],
            searchQuery: "",
            selectedDepartment: "",
            selectedGender: "",
            selectedStage: "",
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0,
            departments: [
                { id: 0, name: "HR" },
                { id: 1, name: "Finance" },
                { id: 2, name: "IT" },
                { id: 3, name: "Sales" },
                { id: 4, name: "Marketing" },
                { id: 5, name: "Operations" },
                { id: 6, name: "Legal" },
                { id: 7, name: "Admin" },
                { id: 8, name: "CustomerSupport" },
            ],
            stages: [
                { id: 0, name: "Manager" },
                { id: 1, name: "HR" },
                { id: 2, name: "Financial" },
            ],
            uniqueRequests: [],
            openAccordions: [],
        };
    },
    methods: {
        async getApprovals() {
            try {
                const result = await getAllApprovals(this.pageNumber, this.pageSize);
                this.approvals = result.data.data;
                this.totalCount = result.data.totalCount;
                this.totalPages = result.data.totalPages;
                this.filteredApprovals = this.approvals;
                this.uniqueRequests = this.getUniqueRequests()
                console.log(this.filteredApprovals);
                console.log(result.data.data);


            } catch (err) {
                console.error(err);
            }
        },
        getUniqueRequests() {
            const requestMap = new Map();
            console.log(this.filteredApprovals);

            this.approvals.forEach((approval) => {
                const { id, userId, user, policyId, policyName, totalAmount, statusName, stageName, comments } = approval.request;
                if (!requestMap.has(id)) {
                    requestMap.set(id, { id, userId, user, policyId, policyName, totalAmount, statusName, stageName, comments });
                }
            });
            return Array.from(requestMap.values());
        },
        getApprovalsForRequest(requestId) {
            return this.approvals.filter((approval) => approval.requestId === requestId);
        },
        toggleAccordion(requestId) {
            if (this.openAccordions.includes(requestId)) {
                this.openAccordions = this.openAccordions.filter((id) => id !== requestId);
            } else {
                this.openAccordions.push(requestId);
            }
        },
        isAccordionOpen(requestId) {
            return this.openAccordions.includes(requestId);
        },
        filterApprovals() {
            const query = this.searchQuery.toLowerCase();
            this.filteredApprovals = this.approvals.filter((approval) => {
                const { userName } = approval.request.user;
                const { departmentName, genderName } = approval.request.user;
                const { userName: reviewerName } = approval.review;

                return (
                    (!query ||
                        userName.toLowerCase().includes(query) ||
                        reviewerName.toLowerCase().includes(query)) &&
                    (!this.selectedDepartment ||
                        departmentName === this.selectedDepartment) &&
                    (!this.selectedGender || genderName === this.selectedGender) &&
                    (this.selectedStage === "" || approval.stage === this.selectedStage)
                );
            });
        },
        getStageName(stage) {
            const stageObj = this.stages.find((s) => s.id === stage);
            return stageObj ? stageObj.name : "Unknown";
        },
        formatDate(dateString) {
            return new Date(dateString).toLocaleDateString();
        },
        nextPage() {
            if (this.pageNumber < this.totalPages) {
                this.pageNumber++;
                this.getApprovals();
            }
        },
        prevPage() {
            if (this.pageNumber > 1) {
                this.pageNumber--;
                this.getApprovals();
            }
        },
    },
    async mounted() {
        await this.getApprovals();
    },
};
</script>

<style scoped>
/* Add any additional custom styles here if needed */
</style>
