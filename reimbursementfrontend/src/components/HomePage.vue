<template>
  <div>
    <nav class="flex justify-between px-6 py-4 items-center bg-white shadow-md">
      <h1 class="text-lg sm:text-xl text-gray-800 font-bold">
        Reimbursement Tracking, {{ department }}
      </h1>
      <button @click="isMobileMenuOpen = true" class="lg:hidden text-gray-700 focus:outline-none">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
        </svg>
      </button>
      <div class="hidden lg:flex">
        <ul class="flex items-center gap-4">
          <li v-if="!(department === 'HR' || department === 'Finance')">
            <RouterLink to="/" class="font-semibold text-gray-700 hover:text-gray-900">Dashboard</RouterLink>
          </li>
          <li v-if="isManager === 'True' || department === 'HR' || department === 'Finance'"
            class="font-semibold text-gray-700">
            <RouterLink to="/employee" class="hover:text-gray-900">Employee Request</RouterLink>
          </li>
          <li class="font-semibold text-gray-700">
            <RouterLink to="/policies" class="hover:text-gray-900">Policies</RouterLink>
          </li>
          <li class="font-semibold text-gray-700">
            <RouterLink to="/profile" class="hover:text-gray-900">Profile</RouterLink>
          </li>
          <li class="font-semibold text-gray-700">
            <RouterLink to="/payment" class="hover:text-gray-900">Payments</RouterLink>
          </li>
          <li class="font-semibold text-gray-700">
            <RouterLink to="/bank" class="hover:text-gray-900">Bank Details</RouterLink>
          </li>
          <a @click="logout" class="font-semibold text-gray-700 hover:text-gray-900">Logout</a>
        </ul>
      </div>
    </nav>

    <!-- Mobile Full-Screen Menu -->
    <div v-if="isMobileMenuOpen"
      class="fixed inset-0 bg-gray-800 bg-opacity-90 flex flex-col items-center justify-center z-50">
      <button @click="isMobileMenuOpen = false"
        class="absolute top-4 right-6 text-gray-200 hover:text-white focus:outline-none">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
      <ul class="flex flex-col items-center gap-8 text-white text-lg">
        <li v-if="!(isManager === 'True' || department === 'HR' || department === 'Finance')">

          <RouterLink to="/" class="hover:text-gray-300" @click="closeMobileMenu">Dashboard</RouterLink>
        </li>
        <li v-if="isManager === 'True' || department === 'HR' || department === 'Finance'" class="hover:text-gray-300">
          <RouterLink to="/employee" @click="closeMobileMenu">Employee Request</RouterLink>
        </li>
        <RouterLink to="/policies" class="hover:text-gray-300" @click="closeMobileMenu">Policies</RouterLink>
        <RouterLink to="/payment" class="hover:text-gray-300" @click="closeMobileMenu">Payments</RouterLink>
        <RouterLink to="/bank" class="hover:text-gray-300" @click="closeMobileMenu">Bank Details</RouterLink>
        <a @click="logout" class="hover:text-gray-300">Logout</a>
      </ul>
    </div>

    <RouterView />
    <div v-if="isToggled"
      class="fixed top-0 left-0 w-full h-full bg-gray-800 bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white w-3/4 max-w-lg p-6 rounded-lg shadow-lg relative overflow-y-auto max-h-[90vh]">
        <button @click="closeModal" class="absolute top-3 right-3 text-gray-500 hover:text-gray-800">
          ✖
        </button>
        <ReimbursementRequestForm />
      </div>
    </div>
  </div>
</template>

<script>
import { useToggleStore } from "@/store/toggle";
import { jwtDecode } from "jwt-decode";
import router from "@/scripts/Route";
import ReimbursementRequestForm from "./Form/ReimbursementRequestForm.vue";

export default {
  name: "HomePage",
  components: {
    ReimbursementRequestForm
  },
  data() {
    return {
      toggle: useToggleStore(),
      isMobileMenuOpen: false,
      isManager: "",
      department: "",
    };
  },
  computed: {
    isToggled() {
      return this.toggle.isToggled;
    },
  },
  watch: {
    isToggled(newVal) {
      if (newVal) {
        document.body.classList.add('no-scroll');
      } else {
        document.body.classList.remove('no-scroll');
      }
    },
  },
  methods: {
    closeModal() {
      this.toggle.toggle();
    },
    closeMobileMenu() {
      this.isMobileMenuOpen = false;
    },
    logout() {
      sessionStorage.clear();
      router.push("/");
      window.location.reload();
    },
  },
  mounted() {
    const token = jwtDecode(sessionStorage.getItem("token"));
    this.isManager = token.Manager;
    this.department = token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  },
};
</script>

<style scoped>
body.no-scroll {
  overflow: hidden;
}
</style>
