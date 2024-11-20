<template>
    <section class="flex">
        <AdminNavbar />
        <RouterView />

    </section>
</template>

<script>

import AdminNavbar from './Admin/AdminNavbar.vue';

export default {
    name: " AdminDashboard", components: { AdminNavbar, },
} </script>

<style></style>
