<template>
    <form @submit:prevent="submit" class="space-y-6 bg-gray-50 p-6 rounded-lg shadow-lg">
        <!-- Request Name -->
        <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Request Name:</label>
            <input type="text" required v-model="comments"
                class="w-full p-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500" />
        </div>

        <!-- Policy -->
        <div>
            <label for="policy" class="block text-sm font-medium text-gray-700 mb-1">Policy</label>
            <select id="policy" v-model="policyId"
                class="w-full p-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500">
                <option :value="policy.id" :key="policy.id" v-for="policy in policies">
                    {{ policy.policyName }}
                </option>
            </select>
        </div>

        <div v-for="(item, index) in itemsData" :key="item" class="p-4 bg-white rounded-lg shadow space-y-4">
            <!-- Description -->
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Description:</label>
                <input type="text" v-model="item.description"
                    class="w-full p-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500" />
            </div>

            <!-- Category -->
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Category</label>
                <select class="w-full p-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500"
                    v-model="item.categoryId">
                    <option :value="category.id" :key="category.id" v-for="category in categories">
                        {{ category.name }}
                    </option>
                </select>
            </div>

            <!-- Amount -->
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Amount:</label>
                <input type="number" v-model="item.amount"
                    class="w-full p-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500" />
            </div>

            <!-- File -->
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">File:</label>
                <input type="file" @change="handleFileChange($event, index)"
                    class="block w-full text-sm text-gray-600 file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border file:border-gray-300 file:bg-gray-100 file:text-gray-700 hover:file:bg-gray-200" />
            </div>
        </div>
        <div class="w-full flex gap-6">

            <button type="submit" @click="submit"
                class="bg-blue-600 text-white px-4 py-2 rounded-lg shadow hover:bg-blue-700">
                Submit
            </button>
            <button type="button" @click="addItem"
                class="bg-blue-800 text-white px-4 py-2 rounded-lg shadow hover:bg-blue-700">
                Add New Item
            </button>
        </div>
    </form>
</template>

<script>
import { getAllCategories } from '@/scripts/Category';
import { getAllPolicy } from '@/scripts/Policy';
import { submitRequest } from '@/scripts/Request';
import { useToggleStore } from '@/store/toggle';
import { jwtDecode } from 'jwt-decode';
import { useToast } from 'vue-toastification';

export default {
    name: "ReimbursementRequestForm",
    data() {
        return {
            toggle: useToggleStore(),
            policies: [],
            toast: useToast(),
            categories: [],
            items: 1,
            comments: "",
            policyId: "",
            itemsData: [
                {
                    description: "",
                    categoryId: "",
                    amount: 0,
                    receiptFile: null,
                }
            ]
        }
    },
    methods: {
        async getCategory() {
            try {

                const res = await getAllCategories();
                this.categories = res.data.data;
            } catch (err) {
                console.log(err);

            }
        },
        async getPolices() {
            try {

                const res = await getAllPolicy();
                this.policies = res.data.data;
            } catch (err) {
                console.log(err);

            }
        },
        addItem() {
            this.itemsData.push({
                description: "",
                categoryId: "",
                amount: 0,
                receiptFile: null,
            });
        },
        async submit() {
            const toastId = this.toast("loading...");

            try {

                event.preventDefault();
                const formData = new FormData();
                const token = jwtDecode(sessionStorage.getItem('token'));
                const id = token.Id;

                formData.append("userId", Number(id))
                formData.append("policyId", Number(this.policyId));
                formData.append("comments", this.comments);

                this.itemsData.forEach((item, index) => {
                    formData.append(`Items[${index}].description`, item.description);
                    formData.append(`Items[${index}].categoryId`, +item.categoryId);
                    formData.append(`Items[${index}].amount`, +item.amount);
                    if (item.receiptFile) {
                        formData.append(`Items[${index}].receiptFile`, item.receiptFile);
                    }
                    else {
                        console.log('error happen');
                    }
                });
                console.log([...formData.entries()]);


                const res = await submitRequest(formData)
                this.toast.update(toastId, {
                    content: "Request is Submitted",
                    options: {
                        type: "success"
                    }
                })

                this.toggle.toggle();
                window.location.reload();
                console.log(res);
            } catch (err) {
                this.toast.update(toastId, {
                    content: err.response.data.errorMessage,
                    options: {
                        type: "error"
                    }
                })
                console.log(err);

            }


        },
        handleFileChange(event, index) {
            console.log(event.target.files);

            const file = event.target.files[0];
            if (file) {
                this.itemsData[index].receiptFile = file;
            } else {
                console.log(`No file selected for item at index ${index}`);
            }
        }
    },
    async mounted() {
        await this.getCategory();
        await this.getPolices();
    }
}
</script>

<style scoped></style>
