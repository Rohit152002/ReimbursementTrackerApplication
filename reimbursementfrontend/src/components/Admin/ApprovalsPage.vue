<template>
    <div class="p-6 w-full max-w-6xl mx-auto">
        <h1 class="text-2xl font-bold text-gray-800 mb-4 text-center">Approvals Page</h1>

        <div class="flex gap-4">

            <!-- Search Input -->
            <input v-model="searchQuery" @input="searchApprovals" type="text"
                placeholder="Search by Request ID, Approval ID, Reviewer, or Comments..."
                class="w-full px-4 py-3 mb-8 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring focus:border-blue-300" />

            <div class="flex mb-8 gap-4">

                <button v-for="status in ['Passed', 'Processing', 'Rejected']" :key="status" :class="[
                    'filter-button',
                    'px-4 py-2 rounded-md',
                    // selectedStatuses.includes(status) ? 'active' : '',
                    // getStatusColor(status),
                    getStatusButtonClass(status)
                ]" @click="toggleStatusFilter(status)">
                    {{ status }}
                </button>

            </div>
        </div>

        <!-- Requests Accordion -->
        <div v-for="request in uniqueRequests" :key="request.id" class="mb-6 border rounded-lg shadow-lg">
            <!-- Accordion Header -->
            <div @click="toggleAccordion(request.id)"
                class="flex justify-between items-center px-4 py-3 bg-gray-100 cursor-pointer hover:bg-gray-200 transition-colors">
                <div class="flex flex-wrap gap-2 items-center">
                    <span class="font-bold text-gray-700">Request ID:</span> {{ request.id }}
                    <span class="ml-4 font-bold text-gray-700">Total Amount:</span> {{ request?.totalAmount }}
                    <span class="ml-4 font-bold text-gray-700">Request By:</span> {{ request.user?.userName }}
                    <span class="ml-4 font-bold text-gray-700">
                        Status:
                        <span class="px-2 py-1 rounded-full text-sm font-semibold"
                            :class="getStatusColor(getRequestStatus(request))">
                            {{ getRequestStatus(request) }}
                        </span>
                    </span>
                </div>
                <span :class="{ 'rotate-180': isAccordionOpen(request.id) }" class="transform transition-transform">
                    ▼
                </span>
            </div>

            <!-- Accordion Content -->
            <div v-if="isAccordionOpen(request.id)" class="px-4 py-4 bg-white">
                <div class="overflow-x-auto">
                    <table class="w-full text-left text-sm">
                        <thead class="bg-gray-200 text-gray-700">
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
        </div>

        <!-- Pagination Controls -->
        <div class="flex items-center justify-between mt-6">
            <button :disabled="pageNumber === 1" @click="prevPage"
                class="px-4 py-2 text-sm font-medium bg-blue-500 text-white rounded-lg disabled:bg-gray-300 hover:bg-blue-600 focus:outline-none focus:ring focus:ring-blue-300">
                Previous
            </button>
            <span class="text-gray-700 text-sm">
                Page {{ pageNumber }} of {{ totalPages }}
            </span>
            <button :disabled="pageNumber === totalPages" @click="nextPage"
                class="px-4 py-2 text-sm font-medium bg-blue-500 text-white rounded-lg disabled:bg-gray-300 hover:bg-blue-600 focus:outline-none focus:ring focus:ring-blue-300">
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
            pageSize: 20,
            totalCount: 0,
            totalPages: 0,
            selectedStatuses: [], // Multi-select for statuses

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
            filterStatus: ""
        };
    },
    methods: {
        getStatusButtonClass(status) {
            const baseClass =
                "px-4 py-2 font-medium rounded transition";
            const activeClass =
                "bg-opacity-100 text-white underline decoration-double";
            const inactiveClass =
                "bg-opacity-50 text-gray-700 hover:bg-opacity-75";

            const colorClass = {
                Processing: "bg-yellow-500",
                Passed: "bg-green-500",
                Rejected: "bg-red-500",
            }[status];

            return `${baseClass} ${colorClass} ${this.selectedStatuses.includes(status) ? activeClass : inactiveClass
                }`;
        },
        toggleStatusFilter(status) {
            const index = this.selectedStatuses.indexOf(status);
            if (index === -1) {
                this.selectedStatuses.push(status);
            } else {
                this.selectedStatuses.splice(index, 1);
            }
            this.searchApprovals();
        },
        filterByStatus(status) {
            this.filterStatus = status;
            this.searchApprovals();
        },
        getRequestStatus(request) {
            const approvals = this.getApprovalsForRequest(request.id);


            const rejected = approvals.some(approval => approval.requestStageName === "Rejected");
            if (rejected) {
                return "Rejected";
            }


            const financialStagePassed = approvals.some(
                approval => approval.stageName === "Financial" && approval.requestStageName === "Approved"
            );

            if (financialStagePassed) {
                return "Passed";
            }

            // Default to processing
            return "Processing";
        }, getStatusColor(status) {
            switch (status) {
                case "Processing":
                    return "bg-yellow-100 text-yellow-800 border-yellow-400";
                case "Passed":
                    return "bg-green-100 text-green-800 border-green-400";
                case "Rejected":
                    return "bg-red-100 text-red-800 border-red-400";
                default:
                    return "bg-gray-100 text-gray-800 border-gray-400";
            }
        },
        searchApprovals() {
            const query = this.searchQuery.toLowerCase();
            console.log(this.selectedStatuses)

            this.filteredApprovals = this.approvals.filter((approval) => {
                const requestStatus = this.getRequestStatus(approval.request);
                const { userName } = approval.request.user; // Requester's name

                const { userName: reviewerName } = approval.review; // Reviewer's name

                return (
                    // Search query matching
                    (!query ||
                        userName.toLowerCase().includes(query) ||
                        reviewerName.toLowerCase().includes(query)) &&
                    // Status filtering
                    (!this.selectedStatuses.length ||
                        this.selectedStatuses.includes(requestStatus))
                );
            });

            // Update the unique requests based on filtered approvals
            this.uniqueRequests = this.getUniqueRequests();
            console.log(this.uniqueRequests)
        },
        async getApprovals() {
            try {
                const result = await getAllApprovals(this.pageNumber, this.pageSize);
                this.approvals = result.data.data;
                this.totalCount = result.data.totalCount;
                this.totalPages = result.data.totalPages;
                this.filteredApprovals = this.approvals;
                this.uniqueRequests = this.getUniqueRequests()
                console.log(this.uniqueRequests)
                // console.log(this.filteredApprovals);
                // console.log(result.data.data);


            } catch (err) {
                console.error(err);
            }
        },
        getUniqueRequests() {
            const requestMap = new Map();
            // console.log(this.filteredApprovals);

            this.filteredApprovals.forEach((approval) => {
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
                const { userName: reviewerName } = approval.review.userName;

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

<style scoped></style>
