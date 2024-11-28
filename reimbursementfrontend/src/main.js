import { createApp } from "vue";
import App from "./App.vue";
import router from "./scripts/Route";
import Toast from "vue-toastification";
import "vue-toastification/dist/index.css";
import { createPinia } from "pinia";

import axiosInstance from "./scripts/Interceptor";
import "./index.css";
router.beforeEach(async (to, from, next) => {
  try {
    const result = await axiosInstance.get("/api/User/users", {
      params: {
        pageNumber: 1,
        pageSize: 100,
      },
    });
    const data = result.data.data;

    if (data.length === 0) {
      // Only redirect to /setup if not already there
      if (to.path !== "/setup") {
        return next("/setup");
      }
    }

    next(); // Proceed to the route if no redirection needed
  } catch (err) {
    console.error("Error: ", err);
    next("/error"); // Redirect to a fallback error page if needed
  }
});

const options = {
  transition: "Vue-Toastification__bounce",
  maxToasts: 20,
  newestOnTop: true,
};

const pinia = createPinia();
createApp(App).use(pinia).use(Toast, options).use(router).mount("#app");
