<template>
    <div class="p-4 bg-gray-100 min-h-screen">
        <h1 class="text-2xl font-bold text-center mb-6">Payment Details</h1>

        <div v-if="payments.length" class="overflow-x-auto">
            <table class="min-w-full bg-white border border-gray-300">
                <thead class="bg-gray-200 text-gray-700">
                    <tr>
                        <th class="px-4 py-2 text-left border-b">Request ID</th>
                        <th class="px-4 py-2 text-left border-b">Policy</th>
                        <th class="px-4 py-2 text-left border-b">Total Amount</th>
                        <th class="px-4 py-2 text-left border-b">Status</th>
                        <th class="px-4 py-2 text-left border-b">Payment Status</th>
                        <th class="px-4 py-2 text-left border-b">Payment Method</th>
                        <th class="px-4 py-2 text-left border-b">Payment Date</th>
                        <th class="px-4 py-2 text-left border-b">Amount Paid</th>
                        <th class="px-4 py-2 text-left border-b">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="payment in payments" :key="payment.id" class="hover:bg-gray-100">
                        <td class="px-4 py-2 border-b">{{ payment.requestId }}</td>
                        <td class="px-4 py-2 border-b">{{ payment.request.policyName }}</td>
                        <td class="px-4 py-2 border-b">₹{{ payment.request.totalAmount }}</td>
                        <td class="px-4 py-2 border-b">{{ payment.request.statusName }}</td>
                        <td class="px-4 py-2 border-b">{{ payment.paymentStatusString }}</td>
                        <td class="px-4 py-2 border-b">
                            {{ payment.paymentMethodString || "N/A" }}
                        </td>
                        <td class="px-4 py-2 border-b">
                            {{ new Date(payment.paymentDate).toLocaleDateString() }}
                        </td>
                        <td class="px-4 py-2 border-b">₹{{ payment.amountPaid }}</td>
                        <td class="px-4 py-2 border-b">
                            <RouterLink :to="`/request/${payment.requestId}`" class="text-blue-500 hover:underline">
                                View Details
                            </RouterLink>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- No Payments -->
        <div v-else class="text-center">
            <p class="text-lg text-gray-500">No payment records found.</p>
        </div>
    </div>
</template>

<script>
import { getPaymentByUser } from "@/scripts/Payment";
import { jwtDecode } from "jwt-decode";

export default {
    name: "PaymentBank",
    data() {
        return {
            payments: [],
            pageNumber: 1,
            pageSize: 10,
        };
    },
    methods: {
        async getPaymentUser() {
            try {
                const id = jwtDecode(sessionStorage.getItem("token")).Id;
                const res = await getPaymentByUser(id, this.pageNumber, this.pageSize);
                this.payments = res.data.data;
            } catch (err) {
                console.log(err);
            }
        },
    },
    async mounted() {
        await this.getPaymentUser();
    },
};
</script>

<style scoped>
/* Custom Scrollbar Styling */
</style>
