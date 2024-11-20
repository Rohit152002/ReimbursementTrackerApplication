<template>
    <div class="mt-40">
        <h2>Reimbursement Requests</h2>
        <div v-for="request in requests" :key="request.id" class="accordion w-[70rem]">
            <div class="accordion-header" @click="toggleAccordion(request.id)">
                <div>
                    <strong>Request ID:</strong> {{ request.id }} |
                    <strong>Policy:</strong> {{ request.policyName }} |
                    <strong>Total Amount:</strong> ${{ request.totalAmount }}
                </div>
                <div>
                    <button>{{ activeRequest === request.id ? "Hide Details" : "View Details" }}</button>
                </div>
            </div>
            <div v-if="activeRequest === request.id" class="accordion-body">
                <p><strong>Submitted by:</strong> {{ request.user.userName }} ({{ request.user.email }})</p>
                <p><strong>Department:</strong> {{ request.user.departmentName }}</p>
                <p><strong>Comments:</strong> {{ request.comments }}</p>

                <h4>Items:</h4>
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Category</th>
                            <th>Amount</th>
                            <th>Description</th>
                            <th>Receipt</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="item in request.items" :key="item.id">
                            <td>{{ item.id }}</td>
                            <td>{{ item.categoryName }}</td>
                            <td>${{ item.amount }}</td>
                            <td>{{ item.description }}</td>
                            <td>
                                <a target="_blank">View Receipt</a>
                            </td>
                        </tr>
                    </tbody>
                </table>
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
            requests: [], // Array to hold reimbursement request data
            activeRequest: null, // ID of the currently expanded accordion
        };
    },
    methods: {

        toggleAccordion(id) {
            this.activeRequest = this.activeRequest === id ? null : id;
        }
    },
    async mounted() {
        const data = await getRequests(1, 10)
        this.requests = data.data.data
        console.log(data.data.data)
    }
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
</style>
