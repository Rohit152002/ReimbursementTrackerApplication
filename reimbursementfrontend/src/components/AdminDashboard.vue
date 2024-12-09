<template>
    <section class="flex">
        <AdminNavbar />
        <div class="router-view-container w-full overflow-y-auto max-h-screen">
            <RouterView />
        </div>

    </section>
</template>

<script>

import AdminNavbar from './Admin/AdminNavbar.vue';

export default {
    name: " AdminDashboard", components: { AdminNavbar, },
} </script>

<style>
.router-view-container {
    height: calc(100vh);
    overflow-y: auto;
}
</style>
