<template>
  <div>
    <nav class="flex justify-between px-20 py-10 items-center bg-white">
      <h1 class="text-xl text-gray-800 font-bold">Reimbursement Tracking , {{ department }}</h1>
      <div class="flex items-center gap-4">
        <!-- <div class="flex items-center border border-solid rounded-lg">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 pt-0.5 text-gray-600" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input class="ml-2 outline-none bg-transparent" type="text" name="search" id="search"
            placeholder="Search..." />
        </div> -->
        <ul class="flex items-center gap-4 cursor-default">
          <RouterLink to="/" class="font-semibold text-gray-700">Dashboard</RouterLink>
          <li v-if="isManager == 'True' || department === 'HR' || department === 'Finance'"
            class="font-semibold text-gray-700">
            <RouterLink to="/employee">Employee Request</RouterLink>
          </li>
          <li class="font-semibold text-gray-700">
            <RouterLink to="/policies">Policies</RouterLink>
          </li>
          <li class="font-semibold text-gray-700">
            <RouterLink to="bank"> Bank Details</RouterLink>
          </li>
          <a @click="logout" class="font-semibold text-gray-700">Logout</a>
          <li>
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path d="M12 14l9-5-9-5-9 5 9 5z" />
              <path
                d="M12 14l6.16-3.422a12.083 12.083 0 01.665 6.479A11.952 11.952 0 0012 20.055a11.952 11.952 0 00-6.824-2.998 12.078 12.078 0 01.665-6.479L12 14z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M12 14l9-5-9-5-9 5-9 5zm0 0l6.16-3.422a12.083 12.083 0 01.665 6.479A11.952 11.952 0 0012 20.055a11.952 11.952 0 00-6.824-2.998 12.078 12.078 0 01.665-6.479L12 14zm-4 6v-7.5l4-2.222" />
            </svg>
          </li>
        </ul>
      </div>
    </nav>

    <!-- <ReimbursementRequest /> -->
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
import { useToggleStore } from '@/store/toggle';
// import ReimbursementRequest from './ReimbursementRequest.vue';
import ReimbursementRequestForm from './Form/ReimbursementRequestForm.vue';
import { jwtDecode } from 'jwt-decode';
import router from '@/scripts/Route';

export default {
  name: 'HomePage',
  data() {
    return {
      toggle: useToggleStore(),
      isManager: "",
      department: ""
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
    logout() {
      sessionStorage.clear();
      router.push('/')
      window.location.reload()
    }
  },
  components: {
    // ReimbursementRequest,
    ReimbursementRequestForm,
  },
  mounted() {
    const token = jwtDecode(sessionStorage.getItem('token'))
    console.log(token);
    this.isManager = token.Manager

    this.department = token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
  }
};
</script>

<style scoped>
/* Prevent background scrolling */
body.no-scroll {
  overflow: hidden;
}

/* Add scrollbar styles if necessary */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-thumb {
  background-color: #cbd5e0;
  /* Light gray */
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background-color: #a0aec0;
  /* Darker gray */
}
</style>
