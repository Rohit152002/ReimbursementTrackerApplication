<template>
    <div class="p-4 bg-gray-100 min-h-screen">
        <h1 class="text-2xl font-bold mb-4 text-center text-gray-800">Company Policies</h1>

        <!-- Policies Grid -->
        <div v-if="policies.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div v-for="policy in policies" :key="policy.id"
                class="bg-white shadow-md rounded-lg p-6 border border-gray-200 hover:shadow-lg transform hover:scale-105 transition-all">
                <h2 class="text-xl font-semibold text-gray-800 mb-2">{{ policy.policyName }}</h2>
                <p class="text-gray-600 mb-4">{{ policy.policyDescription }}</p>
                <div class="text-gray-700 font-medium">
                    <span class="text-green-500">Max Amount:</span> ₹{{ policy.maxAmount }}
                </div>
            </div>
        </div>

        <!-- No Data Message -->
        <div v-else class="text-center text-gray-600 mt-10">
            No policies available.
        </div>

        <!-- Pagination Controls -->
        <div class="flex justify-center items-center mt-6 space-x-4" v-if="totalPages > 1">
            <button @click="changePage(pageNumber - 1)" :disabled="pageNumber === 1"
                class="px-4 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 disabled:opacity-50">
                Previous
            </button>
            <span class="px-4 py-2 text-gray-700 font-semibold">
                Page {{ pageNumber }} of {{ totalPages }}
            </span>
            <button @click="changePage(pageNumber + 1)" :disabled="pageNumber === totalPages"
                class="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:opacity-50">
                Next
            </button>
        </div>
    </div>
</template>

<script>
import { getAllPolicy } from '@/scripts/Policy';

export default {
    name: "PoliciesComponent",
    data() {
        return {
            policies: [],
            pageNumber: 1,
            pageSize: 10,
            totalPages: 0,
        };
    },
    methods: {
        async getPolicies() {
            try {
                const res = await getAllPolicy(this.pageNumber, this.pageSize);
                const responseData = res.data;
                this.policies = responseData.data;
                this.pageNumber = responseData.currentPage;
                this.totalPages = responseData.totalPages;
            } catch (error) {
                console.error("Error fetching policies:", error);
            }
        },
        async changePage(newPage) {
            if (newPage < 1 || newPage > this.totalPages) return;
            this.pageNumber = newPage;
            await this.getPolicies();
        },
    },
    async mounted() {
        await this.getPolicies();
    },
};
</script>

<style scoped></style>
