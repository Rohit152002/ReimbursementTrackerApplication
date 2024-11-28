<template>
  <!-- <HomePage /> -->
  <!-- <LoginRegister /> -->
  <RouterView />
</template>

<script>


// import HomePage from './components/HomePage.vue';
// import LoginRegister from './components/LoginRegister.vue';
// import router from './scripts/Route';
export default {
  name: 'App',
  components: {
    // LoginRegister
    // HomePage
  },
  data() {
    return {
      isLoggedIn: false
    }
  },
  created() {
    const token = sessionStorage.getItem('token');
    if (token) {
      this.isLoggedIn = true;

    }
    else {
      this.isLoggedIn = false
    }
  }
}
</script>

<style></style>
