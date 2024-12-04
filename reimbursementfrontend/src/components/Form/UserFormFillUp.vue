<template>
    <div class="min-h-screen w-full bg-gray-100 flex items-center justify-center">
        <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
            <h1 class="text-2xl font-bold text-center mb-6">User Registration Form</h1>

            <form class="space-y-4">
                <!-- Username -->
                <div>
                    <label for="username" class="block text-sm font-medium text-gray-700">Username</label>
                    <input v-model="username" type="text" id="username"
                        class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm p-2"
                        placeholder="Enter your username" required />
                </div>

                <!-- Email -->
                <div>
                    <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
                    <input v-model="email" type="email" id="email"
                        class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm p-2"
                        placeholder="Enter your email" required />
                </div>

                <!-- Gender -->
                <div>
                    <label for="gender" class="block text-sm font-medium text-gray-700">Gender</label>
                    <select v-model="gender" id="gender"
                        class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm p-2"
                        required>
                        <option value="" disabled selected>Select Your Option</option>
                        <option value="0">Male</option>
                        <option value="1">Female</option>
                    </select>
                </div>
                <div>
                    <label for="gender" class="block text-sm font-medium text-gray-700">Department</label>
                    <select
                        class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm p-2"
                        required v-model="department" name="" id="">
                        <option value="" disabled selected>Select Your Option</option>
                        <option value=0>HR</option>
                        <option value=1>Finance</option>
                        <option value=2>IT</option>
                        <option value=3>Sales</option>
                        <option value=4>Marketing</option>
                        <!-- <option value=7>Admin</option> -->
                    </select>

                </div>

                <!-- Date of Birth -->
                <div>
                    <label for="dateOfBirth" class="block text-sm font-medium text-gray-700">Date of Birth</label>
                    <input v-model="dateOfBirth" type="date" id="dateOfBirth" :min="minDate" :max="maxDate"
                        class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm p-2"
                        required />
                </div>

                <!-- Password -->
                <div>
                    <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
                    <div class="flex h-12">

                        <input v-model="password" :type="!shown ? 'text' : 'password'" id="password"
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm p-2"
                            placeholder="Enter your password" required />
                        <button type="button" @click="toggleShown"><img class="h-8 w-8 object-cover"
                                :src="!shown ? 'images/eye.png' : 'images/hide.png'" alt=""></button>
                    </div>
                </div>
                <div>
                    <label for="address" class="block text-sm font-medium text-gray-700">Address</label>
                    <input v-model="address" type="text" id="username"
                        class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm p-2"
                        placeholder="Enter your address" required />
                </div>

                <!-- Submit Button -->
                <div class="flex flex-col gap-4">
                    <button @click="registerMethod" type="submit"
                        class="w-full bg-blue-600 text-white font-medium py-2 px-4 rounded-md hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50">
                        Submit
                    </button>
                    <RouterLink to="/login"
                        class="w-1/3 bg-blue-600 text-white font-medium py-2 px-4 rounded-md hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 text-center">
                        Login
                    </RouterLink>
                </div>
            </form>
        </div>
    </div>
</template>

<script>
import router from '@/scripts/Route';
import { register } from '@/scripts/User';
import { useToast } from 'vue-toastification';

export default {
    name: "UserFormFillUp",
    data() {


        return {
            shown: false,
            username: "",
            email: "",
            password: "",
            gender: "",
            dateOfBirth: "",
            address: "",
            department: "",
            minDate: "", // 18 years ago in yyyy-mm-dd format
            maxDate: "", //
            toast: useToast()


        }
    },
    methods: {
        setStartYear() {
            const today = new Date();
            let year = today.getFullYear();
            // If the user is under 18, set the starting year to 2002
            if (year - 18 < 2002) {
                year = 2002;
            }
            today.setFullYear(year);
            return today;
        },
        toggleShown() {
            this.shown = !this.shown
        },
        registerMethod() {
            const toast_id = this.toast("loading...");
            event.preventDefault();
            console.log(this.username, this.email, this.password, this.dateOfBirth)
            const date = new Date(this.dateOfBirth).toISOString()
            register(this.username, this.email, this.password, this.gender, this.department, date, this.address)
                .then((res) => {
                    // alert("registration successfull");
                    this.toast.update(toast_id, {
                        content: "Registration Successfull",
                        options: {
                            type: "success"
                        }
                    })
                    sessionStorage.setItem("token", res.data.token)
                    if (res.data.department === 0 || res.data.department === 1) {
                        router.push('/employee')
                    } else
                        router.push('/')

                }, err => {
                    console.log(err)
                    alert(err);
                })
        }
    },
    mounted() {
        // const today = new Date();
        // const minDate = new Date(today.setFullYear(today.getFullYear() - 18)).toISOString().split('T')[0];

        // Get today's date (max date)
        const maxDate = new Date().toISOString().split('T')[0]; // Get the date in YYYY-MM-DD format

        // Set the calculated minDate and maxDate to the data
        // this.minDate = minDate;
        this.maxDate = maxDate;
    }
}
</script>

<style></style>
