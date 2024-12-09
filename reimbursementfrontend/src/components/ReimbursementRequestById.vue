<template>
    <div class="p-8">
        <h1 class="text-2xl font-bold mb-6">Reimbursement Request Details</h1>

        <div v-if="request" class="bg-white shadow-md rounded-lg p-6 mb-6 border border-gray-200">
            <div class="flex items-center justify-between mb-4">
                <h2 class="text-xl font-bold text-gray-800">Reimbursement Details</h2>
                <span :class="statusClass(request?.statusName)"
                    class="px-3 py-1 text-sm rounded-lg font-medium capitalize">
                    {{ request.statusName }}
                </span>
            </div>
            <div class="grid grid-cols-2 gap-4">
                <div>
                    <p class="text-gray-600"><strong class="text-gray-800">Request ID:</strong> {{ request?.id }}</p>
                    <p class="text-gray-600"><strong class="text-gray-800">Employee Name:</strong> {{
                        request.user?.userName }}</p>
                    <p class="text-gray-600"><strong class="text-gray-800">Department:</strong> {{
                        request.user?.departmentName }}</p>
                    <p class="text-gray-600"><strong class="text-gray-800">Policy:</strong> {{ request?.policyName }}
                    </p>
                </div>
                <div>
                    <p class="text-gray-600"><strong class="text-gray-800">Total Amount:</strong> ₹{{
                        request?.totalAmount }}</p>
                    <p class="text-gray-600"><strong class="text-gray-800">Stage:</strong> {{ request?.stageName }}</p>
                    <p class="text-gray-600"><strong class="text-gray-800">Comments:</strong> {{ request?.comments ||
                        "N/A" }}</p>
                </div>
            </div>
            <div v-if="request.items && request.items.length" class="mt-6">
                <h3 class="text-lg font-semibold mb-2 text-gray-800">Items</h3>
                <div class="space-y-4">
                    <div v-for="item in request.items" :key="item.id"
                        class="bg-gray-50 flex gap-32 p-4 rounded-md border border-gray-200">
                        <div>

                            <p class="text-gray-600"><strong class="text-gray-800">Category:</strong> {{
                                item.categoryName
                            }}</p>
                            <p class="text-gray-600"><strong class="text-gray-800">Description:</strong> {{
                                item.description
                            }}</p>
                            <p class="text-gray-600"><strong class="text-gray-800">Amount:</strong> ₹{{ item.amount }}
                            </p>
                        </div>
                        <img :src="'http://localhost:5286/File/' + item.receiptFile" alt="Receipt"
                            class=" h-36 aspect-video object-cover rounded mt-2 cursor-pointer border"
                            @click="openModal(item)" />
                    </div>
                </div>
            </div>

        </div>
        <div v-if="isModelOpen"
            class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50 overflow-auto">
            <div class="bg-white rounded-lg shadow-lg p-6 relative w-3/4 max-w-4xl overflow-auto max-h-[90vh]">
                <button class="absolute top-4 right-4 text-3xl text-gray-600 hover:text-gray-800" @click="closeModal">
                    x
                </button>

                <img :src="'http://localhost:5286/File/' + selectedItem.receiptFile" alt="Receipt Preview"
                    class="w-full h-auto rounded" />
            </div>
        </div>

    </div>
</template>


<script>
import { getRequestById } from "@/scripts/Request";
import { useToast } from "vue-toastification";

export default {
    name: "ReimbursementRequestById",
    data() {
        return {
            request: {},
            toast: useToast(),
            isModelOpen: false,
            selectedItem: null
        };
    },
    methods: {
        openModal(item) {

            this.selectedItem = (JSON.parse(JSON.stringify(item))
            );

            this.isModelOpen = true;
            console.log(this.isModelOpen);

        },
        closeModal() {
            console.log('close')
            this.isModelOpen = false;
            this.selectedItem = null;
        },
        async getRequest(id) {
            try {
                const result = await getRequestById(id);
                console.log(result.data.data);

                if (result.data.isSuccess) {
                    this.request = result.data.data;
                    console.log(this.request);

                } else {
                    this.toast.error(result.message || "Failed to fetch request details.");
                }
            } catch (error) {
                console.error(error);
                this.toast.error("An error occurred while fetching the request. Please try again.");
            }
        },

        statusClass(status) {
            return {
                Pending: "bg-yellow-200 text-yellow-800 px-2 py-1 rounded",
                Passed: "bg-green-200 text-green-800 px-2 py-1 rounded",
                Rejected: "bg-red-200 text-red-800 px-2 py-1 rounded",
            }[status] || "bg-gray-200 text-gray-800 px-2 py-1 rounded";
        },
    },
    async mounted() {
        const id = this.$route.params.id; // Extract ID from route parameters
        await this.getRequest(id);
    },
};
</script>


<style scoped></style>
