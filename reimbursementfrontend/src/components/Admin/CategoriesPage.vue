<template>
    <div class=" w-full p-8 mx-auto mt-8">
        <!-- Page Heading -->
        <h1 class="text-2xl font-bold mb-4 text-gray-800">Categories</h1>

        <!-- Add Category Button -->
        <div class="mb-4">
            <button class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 transition"
                @click="openAddCategoryModel">
                Add Category
            </button>
        </div>

        <!-- Categories Table -->
        <div class="overflow-x-auto">
            <table class="min-w-fit bg-white border border-gray-200 text-left">
                <thead class="bg-gray-100">
                    <tr>
                        <th class="px-4 py-2 border border-gray-200">ID</th>
                        <th class="px-4 py-2 border border-gray-200">Name</th>
                        <th class="px-4 py-2 border border-gray-200">Description</th>
                        <th class="px-4 py-2 border border-gray-200">Action</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="category in categories" :key="category.id" class="hover:bg-gray-50">
                        <td class="px-4 py-2 border border-gray-200">{{ category.id }}</td>
                        <td class="px-4 py-2 border border-gray-200">{{ category.name }}</td>
                        <td class="px-4 py-2 border border-gray-200">{{ category.description }}</td>
                        <td @click="editButton(category)"
                            class=" text-center border border-gray-200  bg-blue-500 text-white rounded hover:bg-blue-600">
                            Edit</td>
                    </tr>
                    <tr v-if="categories.length === 0">
                        <td colspan="3" class="px-4 py-2 text-center text-gray-500 italic">
                            No categories available.
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-if="showAddCategoryModel"
            class="fixed inset-0 bg-gray-800 bg-opacity-50 flex items-center justify-center">
            <div class="bg-white p-6 rounded shadow-md w-3/4 max-w-md">
                <h2 class="text-xl font-bold mb-4">Add New Category</h2>
                <form @submit.prevent="addCategory">
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Category Name</label>
                        <input type="text" v-model="newCategory.name" class="w-full p-2 border border-gray-300 rounded"
                            required />
                    </div>
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Description</label>
                        <textarea v-model="newCategory.description" class="w-full p-2 border border-gray-300 rounded"
                            rows="3" required></textarea>
                    </div>
                    <div class="flex  space-x-4">
                        <button type="button" class="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600"
                            @click="closeAddCategoryModal">
                            Cancel
                        </button>
                        <button type="submit" class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                            Add Category
                        </button>
                    </div>
                </form>
            </div>
        </div>
        <div v-if="showEditCategoryModel"
            class="fixed inset-0 bg-gray-800 bg-opacity-50 flex items-center justify-center">
            <div class="bg-white p-6 rounded shadow-md w-3/4 max-w-md">
                <h2 class="text-xl font-bold mb-4 px-3 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">Edit
                    Category</h2>
                <form @submit.prevent="updateCategory()">
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Category Name</label>
                        <input type="text" v-model="newCategory.name" class="w-full p-2 border border-gray-300 rounded"
                            required />
                    </div>
                    <div class="mb-4">
                        <label class="block text-gray-700 font-semibold mb-1">Description</label>
                        <textarea v-model="newCategory.description" class="w-full p-2 border border-gray-300 rounded"
                            rows="3" required></textarea>
                    </div>
                    <div class="flex  space-x-4">
                        <button type="button" class="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600"
                            @click="() => showEditCategoryModel = false">
                            Cancel
                        </button>
                        <button type="submit" class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                            Update Category
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
import { addCategory, editCategory, getAllCategories } from '@/scripts/Category';
import { useToast } from 'vue-toastification';

export default {
    name: "CategoriesPage",
    data() {
        return {
            categories: [],
            pageNumber: 1,
            pageSize: 10,
            showAddCategoryModel: false,
            showEditCategoryModel: false,
            categoryId: null,
            newCategory: {
                name: "",
                description: ""
            },
            toast: useToast()
        }
    },
    methods: {
        editButton(category) {
            this.showEditCategoryModel = true;
            this.newCategory.name = category.name;
            this.newCategory.description = category.description;
            this.categoryId = category.id
        },
        async updateCategory() {
            try {
                const result = await editCategory(this.categoryId, this.newCategory.name, this.newCategory.description)
                if (result.status === 200) {
                    this.toast.success("Categories Updated")
                }
            } catch (err) {
                this.toast.error("Update failed")
            }
        },
        async getCategory() {
            try {

                const result = await getAllCategories(this.pageNumber, this.pageSize)
                this.categories = result.data.data
                console.log(result)
            } catch (err) {
                console.log(err)
            }
        },

        async addCategory() {
            try {

                // alert("Add category functionality not implemented yet.");
                const response = await addCategory(this.newCategory.name, this.newCategory.description)
                if (response.status === 200) {
                    this.toast.success("Add Category Successfull")
                }
            } catch (err) {
                this.toast.error("Add Category Failed")
            }
            // Add logic to navigate to a category creation page or show a modal.
        },
        closeAddCategoryModal() {
            this.showAddCategoryModel = false;
        },
        openAddCategoryModel() {
            this.showAddCategoryModel = true;
        }


    },
    async mounted() {
        await this.getCategory();
    },

}
</script>

<style scoped></style>
