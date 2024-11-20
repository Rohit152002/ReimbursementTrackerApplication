import LoginRegister from "@/components/LoginRegister.vue";
import HomePage from "@/components/HomePage.vue";
import UserProfile from "@/components/UserProfile.vue";

import { createRouter, createWebHistory } from "vue-router";
import ReimbursementRequest from "@/components/ReimbursementRequest.vue";
import HRDashboard from "@/components/HRDashboard.vue";
import AdminDashboard from "@/components/AdminDashboard.vue";
import FinanceDashboard from "@/components/FinanceDashboard.vue";
import DashboardData from "@/components/Admin/DashboardData.vue";
import EmployeePage from "@/components/Admin/EmployeePage.vue";
import RequestPage from "@/components/Admin/RequestPage.vue";
import BankDetails from "@/components/Admin/BankDetails.vue";
import ApprovalsPage from "@/components/Admin/ApprovalsPage.vue";
import CategoriesPage from "@/components/Admin/CategoriesPage.vue";
import PoliciesPage from "@/components/Admin/PoliciesPage.vue";
import NotFound from "@/components/NotFound.vue";
import RegistrationForm from "@/components/Admin/RegistrationForm.vue";
import UserFormFillUp from "@/components/Form/UserFormFillUp.vue";
import UserList from "@/components/Admin/UserList.vue";
import AssignManager from "@/components/Admin/AssignManager.vue";
import UserProfileById from "@/components/UserProfileById.vue";

const routes = [
  { path: "/login", component: LoginRegister },
  {
    path: "/",
    component: HomePage,
    beforeEnter: (to, from, next) => {
      if (sessionStorage.getItem("token")) {
        next();
      } else {
        next("/login");
      }
    },
  },
  {
    path: "/profile",
    component: UserProfile,
    beforeEnter: (to, from, next) => {
      if (sessionStorage.getItem("token")) {
        next();
      } else {
        next("/login");
      }
    },
  },
  {
    path: "/request",
    component: ReimbursementRequest,
    beforeEnter: (to, from, next) => {
      if (sessionStorage.getItem("token")) {
        next();
      } else {
        next("/login");
      }
    },
  },
  {
    path: "/hr",
    component: HRDashboard,
    beforeEnter: (to, from, next) => {
      if (sessionStorage.getItem("token")) {
        next();
      } else {
        next("/login");
      }
    },
  },
  {
    path: "/admin",
    component: AdminDashboard,
    children: [
      {
        path: "",
        component: DashboardData,
      },
      {
        path: "/employees",
        component: EmployeePage,
      },
      {
        path: "/requests",
        component: RequestPage,
      },
      {
        path: "/banks",
        component: BankDetails,
      },
      {
        path: "/approvals",
        component: ApprovalsPage,
      },
      {
        path: "/category",
        component: CategoriesPage,
      },
      {
        path: "/policy",
        component: PoliciesPage,
      },
      {
        path: "/users",
        component: UserList,
      },
      {
        path: "/assign",
        component: AssignManager,
      },
      {
        path: "/user/:id",
        component: UserProfileById,
      },
    ],
    beforeEnter: (to, from, next) => {
      if (sessionStorage.getItem("token")) {
        next();
      } else {
        next("/login");
      }
    },
  },
  {
    path: "/userform",
    component: UserFormFillUp,
  },

  {
    path: "/finance",
    component: FinanceDashboard,
    beforeEnter: (to, from, next) => {
      if (sessionStorage.getItem("token")) {
        next();
      } else {
        next("/login");
      }
    },
  },
  { path: "/:pathMatch(.*)*", component: NotFound },
  { path: "/setup", component: RegistrationForm },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
