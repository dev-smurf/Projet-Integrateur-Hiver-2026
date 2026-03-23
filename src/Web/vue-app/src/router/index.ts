import i18n from "@/i18n";
import {Role} from "@/types/enums";
import {createRouter, createWebHistory, type Router} from "vue-router";

import Login from "@/views/Login.vue";

import ForgotPassword from "@/views/ForgotPassword.vue";
import ResetPassword from "@/views/ResetPassword.vue";
import Account from "@/views/shared/Account.vue";
import Dashboard from "@/views/shared/Dashboard.vue";

import Admin from "../views/admin/Admin.vue";
import AdminMemberIndex from "@/views/admin/members/AdminMemberIndex.vue";
import AdminAddMemberForm from "@/views/admin/members/AdminAddMemberForm.vue";
import AdminEditMemberForm from "@/views/admin/members/AdminEditMemberForm.vue";
import AdminModuleList from "@/views/admin/members/AdminModuleList.vue";
import AdminAddModule from "@/views/admin/members/AdminAddModule.vue";
import AdminModuleEdit from "@/views/admin/members/AdminModuleEdit.vue";
import AdminQuizIndex from "@/views/admin/quiz/AdminQuizIndex.vue";
import AdminAddQuiz from "@/views/admin/quiz/AdminAddQuiz.vue";
import AdminEditQuiz from "@/views/admin/quiz/AdminEditQuiz.vue";

import Books from "../views/member/Books.vue";
import BookIndex from "@/views/member/BookIndex.vue";
import AddBookForm from "@/views/member/AddBookForm.vue";
import EditBookForm from "@/views/member/EditBookForm.vue";

import QuizList from "@/views/member/quiz/QuizList.vue";
import QuizTake from "@/views/member/quiz/QuizTake.vue";
import QuizResults from "@/views/member/quiz/QuizResults.vue";

import {getLocalizedRoutes} from "@/locales/helpers";
import {useUserStore} from "@/stores/userStore";

let Router_instance: Router | null = null;

export function getRouter(): Router {
  if (Router_instance) {
    return Router_instance;
  }

  Router_instance = createRouter({
    scrollBehavior(to, from, savedPosition) {
      // always scroll to top
      return {top: 0};
    },
    history: createWebHistory(),
    routes: [
      {
        path: "/login",
        alias: getLocalizedRoutes("routes.login.path"),
        name: "login",
        component: Login,
        meta: {
          title: "routes.login.name",
          guest: true
        }
      },
      {
        path: "/forgot-password",
        alias: getLocalizedRoutes("routes.forgotPassword.path"),
        name: "forgotPassword",
        component: ForgotPassword,
        meta: {
          title: "routes.forgotPassword.name",
          guest: true
        }
      },
      {
        path: "/reset-password",
        alias: getLocalizedRoutes("routes.resetPassword.path"),
        name: "resetPassword",
        component: ResetPassword,
        props: (route) => ({userId: route.query.userId, token: route.query.token}),
        meta: {
          title: "routes.resetPassword.name",
          guest: true
        }
      },
      {
        path: "/dashboard",
        alias: getLocalizedRoutes("routes.dashboard.path"),
        name: "dashboard",
        component: Dashboard,
        meta: {
          title: "routes.dashboard.name"
        }
      },
      {
        path: "/my-account",
        alias: getLocalizedRoutes("routes.account.path"),
        name: "account",
        component: Account,
        meta: {
          title: "routes.account.name"
        }
      },
      {
        path: "/administration",
        name: "admin",
        component: Admin,
        meta: {
          requiredRole: Role.Admin,
          noLinkInBreadcrumbs: true,
        },
        children: [
          {
            path: "members",
            name: "admin.children.members.index",
            component: AdminMemberIndex,
          },
          {
            path: "members/add",
            name: "admin.children.members.add",
            component: AdminAddMemberForm,
          },
          {
            path: "members/:id/edit",
            name: "admin.children.members.edit",
            component: AdminEditMemberForm,
            props: true
          },
          {
            path: "modules",
            name: "admin.children.modules.index",
            component: AdminModuleList,
          },
          {
            path: "modules/add",
            name: "admin.children.modules.add",
            component: AdminAddModule,
          },
          {
            path: "modules/:id/edit",
            name: "admin.children.modules.edit",
            component: AdminModuleEdit,
            props: true
          },
          {
            path: "quiz",
            name: "admin.children.quiz.index",
            component: AdminQuizIndex,
          },
          {
            path: "quiz/add",
            name: "admin.children.quiz.add",
            component: AdminAddQuiz,
          },
          {
            path: "quiz/:id/edit",
            name: "admin.children.quiz.edit",
            component: AdminEditQuiz,
            props: true
          },
        ]
      },
      {
        path: "/books",
        alias: getLocalizedRoutes("routes.books.path"),
        name: "books",
        component: Books,
        meta: {
          requiredRole: Role.Member,
          title: "routes.books.name"
        },
        children: [
          {
            path: "",
            name: "books.index",
            component: BookIndex,
            meta: {
              title: "routes.books.name"
            }
          },
          {
            path: "add",
            alias: getLocalizedRoutes("routes.books.children.add.path"),
            name: "books.children.add",
            component: AddBookForm,
            meta: {
              title: "routes.books.children.add.name"
            }
          },
          {
            path: ":id/edit",
            alias: getLocalizedRoutes("routes.books.children.edit.path"),
            name: "books.children.edit",
            component: EditBookForm,
            props: true,
            meta: {
              title: "routes.books.children.edit.name"
            }
          }
        ]
      },
      {
        path: "/quiz",
        name: "quiz",
        component: { template: '<router-view />' },
        meta: {
          requiredRole: Role.Member,
          title: "routes.quiz.name"
        },
        children: [
          {
            path: "",
            name: "quiz.list",
            component: QuizList,
            meta: {
              title: "routes.quiz.name"
            }
          },
          {
            path: ":quizId/take",
            name: "quiz.take",
            component: QuizTake,
            props: true,
            meta: {
              title: "routes.quiz.take.name"
            }
          },
          {
            path: ":quizId/results",
            name: "quiz.results",
            component: QuizResults,
            props: true,
            meta: {
              title: "routes.quiz.results.name"
            }
          }
        ]
      },
    ]
  });

  // eslint-disable-next-line
  Router_instance.beforeEach(async (to, from) => {
    const userStore = useUserStore()
    const isAuthenticated = !!userStore.user.email;

    // Handle root path redirect
    if (to.path === "/") {
      return isAuthenticated ? { name: "dashboard" } : { name: "login" };
    }

    // Logged-in users cannot access guest-only pages (login, forgot password, etc.)
    if (to.meta.guest && isAuthenticated) {
      return { name: "dashboard" };
    }

    // Non-authenticated users cannot access protected pages
    if (!to.meta.guest && !isAuthenticated) {
      return { name: "login" };
    }

    // Role-based access control
    if (to.meta.requiredRole && userStore.user.roles && !userStore.user.roles.includes(to.meta.requiredRole as Role)) {
      return { name: "dashboard" };
    }
  });

  return Router_instance;
}

export type { Router };
