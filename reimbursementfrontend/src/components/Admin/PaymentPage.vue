<template>
    <div class="min-h-screen bg-gray-100 p-4">
        <h1 class="text-2xl font-bold mb-4 text-center">Payment Page</h1>

        <!-- Search Input -->
        <div class="flex justify-center mb-4">
            <input type="text" v-model="searchQuery" @input="applySearch" placeholder="Search by User, Policy, or Email"
                class="w-full p-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500 mx-2" />
        </div>

        <!-- Payment List -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div v-for="payment in filteredPayments" :key="payment.id" class="bg-white rounded shadow-md p-4">
                <h2 class="text-lg font-bold mb-2">Request ID: {{ payment.requestId }}</h2>
                <p><strong>User:</strong> {{ payment.request.user.userName }}</p>
                <p><strong>Email:</strong> {{ payment.request.user.email }}</p>
                <p><strong>Policy:</strong> {{ payment.request.policyName }}</p>
                <p><strong>Total Amount:</strong> ₹{{ payment.request.totalAmount }}</p>
                <p>
                    <strong>Status:</strong>
                    <span class="text-green-500">
                        Paid
                    </span>
                </p>
                <p><strong>Payment Date:</strong> {{ formatDate(payment.paymentDate) }}</p>
                <RouterLink :to="`/admin/request/${payment.requestId}`">
                    <button class="mt-4 w-full px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                        View Details
                    </button>
                </RouterLink>
            </div>
        </div>

        <!-- Pagination -->
        <div class="flex justify-between items-center mt-6">
            <button :disabled="pageNumber === 1" @click="changePage('prev')"
                class="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600 disabled:opacity-50">
                Previous
            </button>
            <span>Page {{ pageNumber }}</span>
            <button :disabled="filteredPayments.length < pageSize" @click="changePage('next')"
                class="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600 disabled:opacity-50">
                Next
            </button>
        </div>
    </div>
</template>


<script>
import { getAllPayment } from "@/scripts/Payment";

export default {
    name: "PaymentPage",
    data() {
        return {
            payment: [],            // Store all payment data
            filteredPayments: [],   // Store filtered payments based on search query
            pageNumber: 1,          // Current page number for pagination
            pageSize: 10,           // Items per page
            searchQuery: "",        // Search query string
        };
    },
    methods: {
        async getPayments() {
            try {
                const res = await getAllPayment(this.pageNumber, this.pageSize);
                this.payment = res.data.data
                    .sort((a, b) => b.paymentStatus - a.paymentStatus); // Payments are already marked as "paid" (status 2)
                this.applySearch();
            } catch (err) {
                console.error(err.response.data.errorMessage);
            }
        },
        applySearch() {
            // Filter payments based on the search query
            const query = this.searchQuery.toLowerCase();
            this.filteredPayments = this.payment.filter((p) => {
                return (
                    p.request.user.userName.toLowerCase().includes(query) ||
                    p.request.user.email.toLowerCase().includes(query) ||
                    p.request.policyName.toLowerCase().includes(query)
                );
            });
        },
        changePage(direction) {
            if (direction === "prev" && this.pageNumber > 1) {
                this.pageNumber--;
            } else if (direction === "next") {
                this.pageNumber++;
            }
            this.getPayments();
        },
        formatDate(date) {
            const options = { year: "numeric", month: "long", day: "numeric" };
            return new Date(date).toLocaleDateString(undefined, options);
        },
    },
    async mounted() {
        await this.getPayments();
    },
};
</script>


<style scoped>
/* Add any additional styling if needed */
</style>
