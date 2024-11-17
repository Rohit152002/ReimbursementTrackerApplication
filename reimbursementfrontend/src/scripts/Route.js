import LoginRegister from "@/components/LoginRegister.vue";
import HomePage from "@/components/HomePage.vue";
import UserProfile from "@/components/UserProfile.vue";

import { createRouter,createWebHistory } from "vue-router";
import ReimbursementRequest from "@/components/ReimbursementRequest.vue";

const routes=[
    {path:"/",component:HomePage},
    {path:"/login",component:LoginRegister},
    {path:"/profile",component:UserProfile},
    {path:"/request",component:ReimbursementRequest}

]

const router=createRouter({
    history:createWebHistory(),
    routes
});

export default router;
