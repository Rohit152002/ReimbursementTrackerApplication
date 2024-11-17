<template>

  <RouterView v-if="isLoggedIn"/>
  <LoginRegister v-else/>
</template>

<script>

import LoginRegister from   './components/LoginRegister.vue';
// import router from './scripts/Route';
export default {
  name: 'App',
  components: {

    LoginRegister
  },
  data()
  {
    return {
      isLoggedIn:false
    }
  },
  created()
  {
    const token = sessionStorage.getItem('token');
    if(token)
  {
    this.isLoggedIn=true;

  }
  else{
    this.isLoggedIn=false
  }
  }
}
</script>

<style>

</style>
