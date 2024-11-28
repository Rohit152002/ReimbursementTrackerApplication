<template>
    <div class="flex flex-col items-center justify-center min-h-screen bg-gray-100 text-gray-800">
        <div class="text-center">
            <h1 class="text-9xl font-extrabold text-blue-500">404</h1>
            <p class="text-2xl md:text-3xl font-bold mb-4">
                Oops! The page you’re looking for isn’t here.
            </p>
            <p class="mb-8 text-gray-500">
                You might have the wrong address, or the page may have moved.
            </p>
            <div>
                <router-link to="/"
                    class="px-6 py-3 bg-blue-500 text-white font-bold rounded hover:bg-blue-600 transition">
                    Go Home
                </router-link>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: "NotFound",
};
</script>

<style scoped>
/* Scoped styles are not necessary here as Tailwind handles the design */
</style>
