<template>
    <div class="p-4 max-w-4xl mx-auto">
        <div v-if="bank && bank.id" class="bg-white shadow-md rounded-lg p-6">
            <h2 class="text-2xl font-semibold mb-4">Bank Details</h2>
            <div class="space-y-2">
                <p><span class="font-bold">Account Number:</span> {{ bank.accNo }}</p>
                <p><span class="font-bold">Branch Name:</span> {{ bank.branchName }}</p>
                <p><span class="font-bold">IFSC Code:</span> {{ bank.ifscCode }}</p>
                <p><span class="font-bold">Branch Address:</span> {{ bank.branchAddress }}</p>
            </div>
            <button @click="() => showUpdateModel = true"
                class="bg-blue-700 text-white px-4 py-2 rounded-lg">Update</button>
        </div>

        <div v-else class="bg-white shadow-md rounded-lg p-6">
            <h2 class="text-2xl font-semibold mb-4">Add Your Bank Details</h2>
            <form @submit.prevent="submitBankDetails" class="space-y-4">
                <div>
                    <label for="accNo" class="block text-sm font-medium text-gray-700">Account Number</label>
                    <input type="text" id="accNo" v-model="form.accNo"
                        class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
                        placeholder="Enter your account number" required />
                </div>
                <div>
                    <label for="branchName" class="block text-sm font-medium text-gray-700">Branch Name</label>
                    <input type="text" id="branchName" v-model="form.branchName"
                        class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
                        placeholder="Enter branch name" required />
                </div>
                <div>
                    <label for="ifscCode" class="block text-sm font-medium text-gray-700">IFSC Code</label>
                    <input type="text" id="ifscCode" v-model="form.ifscCode"
                        class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
                        placeholder="Enter IFSC code" required />
                </div>
                <div>
                    <label for="branchAddress" class="block text-sm font-medium text-gray-700">Branch Address</label>
                    <input type="text" id="branchAddress" v-model="form.branchAddress"
                        class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
                        placeholder="Enter branch address" required />
                </div>
                <button type="submit" class="bg-blue-500 text-white px-4 py-2 rounded-md shadow hover:bg-blue-600">
                    Submit
                </button>
            </form>
        </div>
        <div v-if="showUpdateModel"
            class="fixed top-0 left-0 w-full h-full bg-gray-800 bg-opacity-50 flex items-center justify-center z-50">
            <div class="bg-white w-3/4 max-w-lg p-6 rounded-lg shadow-lg relative overflow-y-auto max-h-[90vh]">
                <button @click="() => showUpdateModel = false"
                    class="absolute top-3 right-3 text-gray-500 hover:text-gray-800">
                    ✖
                </button>
                <form @submit.prevent="updateBankDetails" class="space-y-4">
                    <div>
                        <label for="accNo" class="block text-sm font-medium text-gray-700">Account Number</label>
                        <input type="text" id="accNo" v-model="bank.accNo"
                            class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm" value="fsdljf"
                            required />
                    </div>
                    <div>
                        <label for="branchName" class="block text-sm font-medium text-gray-700">Branch Name</label>
                        <input type="text" id="branchName" v-model="bank.branchName"
                            class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm" value="fdsklajf"
                            placeholder="Enter branch name" required />
                    </div>
                    <div>
                        <label for="ifscCode" class="block text-sm font-medium text-gray-700">IFSC Code</label>
                        <input type="text" id="ifscCode" v-model="bank.ifscCode"
                            class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
                            placeholder="Enter IFSC code" required />
                    </div>
                    <div>
                        <label for="branchAddress" class="block text-sm font-medium text-gray-700">Branch
                            Address</label>
                        <input type="text" id="branchAddress" v-model="bank.branchAddress"
                            class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
                            placeholder="Enter branch address" required />
                    </div>
                    <button type="submit" class="bg-blue-500 text-white px-4 py-2 rounded-md shadow hover:bg-blue-600">
                        Update
                    </button>
                </form>
            </div>
        </div>
    </div>

</template>

<script>
import { addBank, getBankByUser, updateBank } from "@/scripts/Bank";
import { jwtDecode } from "jwt-decode";
import { useToast } from "vue-toastification";

export default {
    name: "BankComponent",
    data() {
        return {
            bank: null,
            form: {
                accNo: "",
                branchName: "",
                ifscCode: "",
                branchAddress: "",
            },
            showUpdateModel: false,
            toast: useToast()
        };
    },
    methods: {
        async getBank() {
            try {
                const id = jwtDecode(sessionStorage.getItem("token"))["Id"];
                const res = await getBankByUser(+id);
                this.bank = res.data.data;
            } catch (err) {
                console.log("Error fetching bank details:", err);
            }
        },
        async submitBankDetails() {
            try {

                event.preventDefault()
                console.log("Bank details submitted:", this.form);
                const formData = JSON.parse(JSON.stringify(this.form))

                this.form = {
                    accNo: "",
                    branchName: "",
                    ifscCode: "",
                    branchAddress: "",
                };

                const id = jwtDecode(sessionStorage.getItem("token"))["Id"];
                const res = await addBank(id, formData.accNo, formData.branchName, formData.ifscCode, formData.branchAddress)
                console.log(res);

                this.toast.success("Added successfully");
                await this.getBank();
            } catch (err) {
                console.error(err);
            }
        },
        async updateBankDetails() {
            try {

                const id = jwtDecode(sessionStorage.getItem("token"))["Id"];
                const updated = await updateBank(this.bank.id, id, this.bank.accNo, this.bank.branchName, this.bank.ifscCode, this.bank.branchAddress)
                if (updated) {
                    this.toast.success("Updated succeffull");
                    this.getBank()
                    this.showUpdateModel = false

                }
            } catch (err) {
                console.error(err.message)
            }
        }
    },
    async mounted() {
        await this.getBank();
    },
};
</script>

<style scoped></style>
