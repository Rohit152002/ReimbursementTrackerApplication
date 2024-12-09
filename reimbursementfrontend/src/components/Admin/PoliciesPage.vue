<template>
    <div class=" mx-auto w-full p-4">
        <h1 class="text-2xl font-bold text-center mb-6">Company Policies</h1>

        <!-- Company Description -->
        <div class="bg-blue-50 p-4 rounded-md shadow mb-6">
            <h2 class="text-xl font-semibold text-blue-600">About Our Policies</h2>
            <p class="text-gray-700 mt-2">
                Our policies are designed to ensure smooth operations while maintaining fairness and transparency for
                all employees.
            </p>
        </div>

        <!-- Add Policy Button -->
        <div class="flex justify-between items-center mb-4">
            <input type="text" v-model="searchQuery" placeholder="Search policies..."
                class="w-2/3 p-3 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500" />
            <button class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600" @click="openAddPolicyModal">
                Add Policy
            </button>
        </div>

        <!-- Policies Table -->
        <div class="overflow-x-auto">
            <table class="min-w-full border border-gray-200">
                <thead class="bg-gray-100">
                    <tr>
                        <th class="px-4 py-2 text-left border border-gray-200">#</th>
                        <th class="px-4 py-2 text-left border border-gray-200">Policy Name</th>
                        <th class="px-4 py-2 text-left border border-gray-200">Max Amount</th>
                        <th class="px-4 py-2 text-left border border-gray-200">Description</th>
                        <th class="px-4 py-2 text-center border border-gray-200">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(policy, index) in filteredPolicies" :key="policy.id" class="hover:bg-gray-50">
                        <td class="px-4 py-2 w-fit border border-gray-200">{{ index + 1 }}</td>
                        <td class="px-4 py-2 w-fit border border-gray-200 font-semibold">{{ policy.policyName }}</td>
                        <td class="px-4 py-2 border border-gray-200">{{ policy.maxAmount }}</td>
                        <td class="px-4 py-2 border border-gray-200">{{ policy.policyDescription }}</td>
                        <td class="px-4 py-2 border border-gray-200 text-center">
                            <div class="flex justify-center items-center space-x-2">
                                <!-- <button class="px-3 py-2 bg-green-500 text-white rounded hover:bg-green-600"
                                    @click="viewDetails(policy)">
                                    View
                                </button> -->
                                <button class="px-3 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
                                    @click="editPolicyButton(policy)">
                                    Edit
                                </button>
                                <button class="px-3 py-2 bg-red-500 text-white rounded hover:bg-red-600"
                                    @click="deletePolicy(policy.id)">
                                    Delete
                                </button>
                            </div>
                        </td>
                    </tr>
                    <tr v-if="filteredPolicies.length === 0">
                        <td colspan="5" class="px-4 py-2 text-center text-gray-500 italic">
                            No policies found.
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>


        <!-- Policy Details Modal -->
        <div v-if="selectedPolicy" class="fixed inset-0 bg-gray-800 bg-opacity-50 flex items-center justify-center">
            <div class="bg-white p-6 rounded shadow-md w-3/4 max-w-md">
                <h2 class="text-xl font-bold mb-4">{{ selectedPolicy.policyName }}</h2>
                <p class="mb-2"><strong>Max Amount:</strong> {{ selectedPolicy.maxAmount }}</p>
                <p class="mb-2"><strong>Description:</strong> {{ selectedPolicy.policyDescription }}</p>
                <button class="mt-4 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600" @click="closeModal">
                    Close
                </button>
            </div>
        </div>

        <!-- Add Policy Modal -->
        <div v-if="showAddPolicyModal" class="fixed inset-0 bg-gray-800 bg-opacity-50 flex items-center justify-center">
            <div class="bg-white p-6 rounded shadow-md w-3/4 max-w-md">
                <h2 class="text-xl font-bold mb-4">Add New Policy</h2>
                <form @submit.prevent="addPolicy">
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Policy Name</label>
                        <input type="text" v-model="newPolicy.policyName"
                            class="w-full p-2 border border-gray-300 rounded" required />
                    </div>
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Max Amount</label>
                        <input type="number" v-model="newPolicy.maxAmount"
                            class="w-full p-2 border border-gray-300 rounded" required />
                    </div>
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Description</label>
                        <textarea v-model="newPolicy.policyDescription"
                            class="w-full p-2 border border-gray-300 rounded" rows="3" required></textarea>
                    </div>
                    <div class="flex  space-x-4">
                        <button type="button" class="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600"
                            @click="closeAddPolicyModal">
                            Cancel
                        </button>
                        <button type="submit" class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                            Add Policy
                        </button>
                    </div>
                </form>
            </div>
        </div>
        <div v-if="editPolicyModel" class="fixed inset-0 bg-gray-800 bg-opacity-50 flex items-center justify-center">
            <div class="bg-white p-6 rounded shadow-md w-3/4 max-w-md">
                <h2 class="text-xl font-bold mb-4">Edit Policy</h2>
                <form @submit.prevent="updatePolicySubmit">
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Policy Name</label>
                        <input type="text" v-model="updatePolicy.policyName"
                            class="w-full p-2 border border-gray-300 rounded" required />
                    </div>
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Max Amount</label>
                        <input type="number" v-model="updatePolicy.maxAmount"
                            class="w-full p-2 border border-gray-300 rounded" required />
                    </div>
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Description</label>
                        <textarea v-model="updatePolicy.policyDescription"
                            class="w-full p-2 border border-gray-300 rounded" rows="3" required></textarea>
                    </div>
                    <div class="flex  space-x-4">
                        <button type="button" class="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600"
                            @click="() => editPolicyModel = false">
                            Cancel
                        </button>
                        <button type="submit" class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                            Submit
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
import { AddPolicy, DeletePolicy, getAllPolicy, UpdatePolicy } from '@/scripts/Policy';
import { useToast } from 'vue-toastification';

export default {
    name: "PoliesPage",
    data() {
        return {
            policies: [],
            selectedPolicy: null,
            showAddPolicyModal: false,
            editPolicyModel: false,
            newPolicy: {
                policyName: "",
                maxAmount: null,
                policyDescription: "",
            },
            updatePolicy: {
                id: 0,
                policyName: "",
                maxAmount: null,
                policyDescription: "",
            },
            searchQuery: "",
            toast: useToast()

        }
    },
    computed: {
        filteredPolicies() {
            if (!this.searchQuery) return this.policies;
            const query = this.searchQuery.toLowerCase();
            return this.policies.filter((policy) => {
                return (
                    policy.policyName.toLowerCase().includes(query) ||
                    policy.policyDescription.toLowerCase().includes(query)
                );
            });
        },
    },
    methods: {
        editPolicyButton(policy) {
            this.editPolicyModel = true
            this.updatePolicy.id = policy.id
            this.updatePolicy.policyName = policy.policyName
            this.updatePolicy.policyDescription = policy.policyDescription
            this.updatePolicy.maxAmount = policy.maxAmount



        },
        viewDetails(policy) {
            this.selectedPolicy = policy;
        },
        async deletePolicy(policyId) {
            if (confirm("Are you sure you want to delete this policy?")) {
                this.policies = this.policies.filter((policy) => policy.id !== policyId);
                const result = await DeletePolicy(policyId);
                if (result.status === 200)
                    this.toast.success("policy delete successfully ")
            }
        },
        openAddPolicyModal() {
            this.showAddPolicyModal = true;
        },
        closeAddPolicyModal() {
            this.showAddPolicyModal = false;
            this.resetNewPolicy();
        },
        async addPolicy() {
            const newPolicy = { ...this.newPolicy, id: Date.now() };
            this.policies.push(newPolicy);
            await AddPolicy(newPolicy.policyName, newPolicy.maxAmount, newPolicy.policyDescription)
            this.closeAddPolicyModal();
        },
        resetNewPolicy() {
            this.newPolicy = { policyName: "", maxAmount: null, policyDescription: "" };
        },
        closeModal() {
            this.selectedPolicy = null;
        },
        async getPolicies() {
            const res = await getAllPolicy(1, 10);
            console.log(res);
            this.policies = res.data.data
        },
        async updatePolicySubmit() {
            try {
                const response = await UpdatePolicy(this.updatePolicy.id, this.updatePolicy.policyName, this.updatePolicy.maxAmount, this.updatePolicy.policyDescription);
                if (response.status === 200) {
                    this.toast.success("successfully updated")
                }
            } catch (err) {
                this.toast.error("Update Policy Failed")
            }
        }
    },
    async mounted() {
        await this.getPolicies()
    }
}
</script>

<style scoped></style>
