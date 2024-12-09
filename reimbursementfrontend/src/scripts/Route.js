import LoginRegister from "@/components/LoginRegister.vue";
import HomePage from "@/components/HomePage.vue";
import UserProfile from "@/components/UserProfile.vue";

import { createRouter, createWebHistory } from "vue-router";
import ReimbursementRequest from "@/components/ReimbursementRequest.vue";
// import HRDashboard from "@/components/HRDashboard.vue";
import AdminDashboard from "@/components/AdminDashboard.vue";
// import FinanceDashboard from "@/components/FinanceDashboard.vue";
import DashboardData from "@/components/Admin/DashboardData.vue";
// import EmployeePage from "@/components/Admin/EmployeePage.vue";
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
import EmployeeRequest from "@/components/EmployeeRequest.vue";
import EmployeePage from "@/components/Admin/EmployeePage.vue";
import ReimbursementRequestById from "@/components/ReimbursementRequestById.vue";
import PoliciesComponent from "@/components/PoliciesComponent.vue";
import BankComponent from "@/components/BankComponent.vue";
import PaymentPage from "@/components/Admin/PaymentPage.vue";
import { jwtDecode } from "jwt-decode";
import PaymentBank from "@/components/PaymentBank.vue";

const routes = [
  { path: "/login", component: LoginRegister },
  {
    path: "/",
    component: HomePage,
    children: [
      {
        path: "/",
        component: ReimbursementRequest,
      },
      {
        path: "/employee",
        component: EmployeeRequest,
      },
      {
        path: "/policies",
        component: PoliciesComponent,
      },
      {
        path: "/request/:id",
        component: ReimbursementRequestById,
      },
      {
        path: "/bank",
        component: BankComponent,
      },
      {
        path: "/payment",
        component: PaymentBank,
      },
      {
        path: "/profile",
        component: UserProfile,
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
  // {
  //   path: "/profile",
  //   component: UserProfile,
  //   beforeEnter: (to, from, next) => {
  //     if (sessionStorage.getItem("token")) {
  //       next();
  //     } else {
  //       next("/login");
  //     }
  //   },
  // },
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
  // {
  //   path: "/hr",
  //   component: HRDashboard,
  //   beforeEnter: (to, from, next) => {
  //     if (
  //       jwtDecode(sessionStorage.getItem("token"))[
  //         "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
  //       ] === "HR"
  //     ) {
  //       next();
  //     } else {
  //       next("/login");
  //     }
  //   },
  // },
  {
    path: "/admin",
    component: AdminDashboard,
    children: [
      {
        path: "/admin/",
        component: DashboardData,
      },
      {
        path: "/admin/employees",
        component: EmployeePage,
      },
      {
        path: "/admin/requests",
        component: RequestPage,
      },
      {
        path: "/admin/banks",
        component: BankDetails,
      },
      {
        path: "/admin/approvals",
        component: ApprovalsPage,
      },
      {
        path: "/admin/category",
        component: CategoriesPage,
      },
      {
        path: "/admin/policy",
        component: PoliciesPage,
      },
      {
        path: "/admin/users",
        component: UserList,
      },
      {
        path: "/admin/assign",
        component: AssignManager,
      },
      {
        path: "/admin/user/:id",
        component: UserProfileById,
      },
      {
        path: "/admin/request/:id",
        component: ReimbursementRequestById,
      },
      {
        path: "/admin/payments",
        component: PaymentPage,
      },
    ],
    beforeEnter: (to, from, next) => {
      // "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      if (
        jwtDecode(sessionStorage.getItem("token"))[
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ] === "Admin"
      ) {
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

  // {
  //   path: "/finance",
  //   component: FinanceDashboard,
  //   beforeEnter: (to, from, next) => {
  //     if (sessionStorage.getItem("token")) {
  //       next();
  //     } else {
  //       next("/login");
  //     }
  //   },
  // },
  { path: "/:pathMatch(.*)*", component: NotFound },
  { path: "/setup", component: RegistrationForm },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
