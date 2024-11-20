<template>
    <div class="min-h-screen bg-gray-100 flex items-center justify-center">
        <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
            <h1 class="text-2xl font-bold text-center mb-6">Admin Registration Form</h1>

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
                        <option value="0">Male</option>
                        <option value="1">Female</option>
                    </select>
                </div>

                <!-- Date of Birth -->
                <div>
                    <label for="dateOfBirth" class="block text-sm font-medium text-gray-700">Date of Birth</label>
                    <input v-model="dateOfBirth" type="date" id="dateOfBirth"
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
                        placeholder="Enter your username" required />
                </div>

                <!-- Submit Button -->
                <div>
                    <button @click="registerMethod" type="submit"
                        class="w-full bg-blue-600 text-white font-medium py-2 px-4 rounded-md hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50">
                        Submit
                    </button>
                </div>
            </form>
        </div>
    </div>
</template>

<script>
import router from '@/scripts/Route';
import { register } from '@/scripts/User';

// import { register } from '@/scripts/User';

export default {
    name: "RegistrationForm",
    data() {
        return {
            shown: false,
            username: "",
            email: "",
            password: "",
            gender: "",
            dateOfBirth: "",
            address: ""

        }
    },
    methods: {
        toggleShown() {
            this.shown = !this.shown
        },
        registerMethod() {
            event.preventDefault();
            console.log(this.username, this.email, this.password, this.dateOfBirth)
            const date = new Date(this.dateOfBirth).toISOString()
            register(this.username, this.email, this.password, this.gender, 7, date, this.address)
                .then((res) => {
                    alert("registration successfull");
                    sessionStorage.setItem("token", res.data.token)
                    router.push('/admin')

                }, err => {
                    console.log(err)
                    alert(err);
                })
        }
    }
}
</script>

<style scoped></style>
