import { h } from "vue";
import i18n from "@/i18n";
import { getLocalizedRoutes } from "@/locales/helpers";
import { useUserStore } from "@/stores/userStore";
import { Role } from "@/types/enums";
import { createRouter, createWebHistory, RouterView, type Router } from "vue-router";

import Login from "@/views/Login.vue";
import ForgotPassword from "@/views/ForgotPassword.vue";
import ResetPassword from "@/views/ResetPassword.vue";
import Account from "@/views/shared/Account.vue";
import Dashboard from "@/views/shared/Dashboard.vue";
import MemberDashboard from "@/views/member/MemberDashboard.vue";

import Admin from "@/views/admin/Admin.vue";
import AdminAvailability from "@/views/admin/AdminAvailability.vue";
import AdminAddEquipeForm from "@/views/admin/equipe/AdminAddEquipeForm.vue";
import AdminEditEquipeForm from "@/views/admin/equipe/AdminEditEquipeForm.vue";
import AdminEquipeList from "@/views/admin/equipe/EquipesListe.vue";
import AdminAddMemberForm from "@/views/admin/members/AdminAddMemberForm.vue";
import AdminAddModule from "@/views/admin/members/AdminAddModule.vue";
import AdminEditMemberForm from "@/views/admin/members/AdminEditMemberForm.vue";
import AdminMemberDetails from "@/views/admin/members/AdminMemberDetails.vue";
import AdminMemberIndex from "@/views/admin/members/AdminMemberIndex.vue";
import AdminModuleEdit from "@/views/admin/members/AdminModuleEdit.vue";
import AdminModuleList from "@/views/admin/members/AdminModuleList.vue";
import AdminModulePreview from "@/views/admin/members/AdminModulePreview.vue";
import AdminAddQuiz from "@/views/admin/quiz/AdminAddQuiz.vue";
import AdminEditQuiz from "@/views/admin/quiz/AdminEditQuiz.vue";
import AdminQuizIndex from "@/views/admin/quiz/AdminQuizIndex.vue";

import MemberModuleList from "@/views/member/MemberModuleList.vue";
import MemberModuleView from "@/views/member/MemberModuleView.vue";
import QuizList from "@/views/member/quiz/QuizList.vue";
import QuizResults from "@/views/member/quiz/QuizResults.vue";
import QuizTake from "@/views/member/quiz/QuizTake.vue";

import AdminEquipeDetails from "@/views/admin/equipe/AdminEquipeDetails.vue";

let routerInstance: Router | null = null;

export function getRouter(): Router {
  if (routerInstance) {
    return routerInstance;
  }

  routerInstance = createRouter({
    scrollBehavior() {
      return { top: 0 };
    },
    history: createWebHistory(),
    routes: [
      {
        path: i18n.t("routes.login.path"),
        alias: getLocalizedRoutes("routes.login.path"),
        name: "login",
        component: Login,
        meta: {
          title: "routes.login.name",
          guest: true,
        },
      },
      {
        path: i18n.t("routes.forgotPassword.path"),
        alias: getLocalizedRoutes("routes.forgotPassword.path"),
        name: "forgotPassword",
        component: ForgotPassword,
        meta: {
          title: "routes.forgotPassword.name",
          guest: true,
        },
      },
      {
        path: i18n.t("routes.resetPassword.path"),
        alias: getLocalizedRoutes("routes.resetPassword.path"),
        name: "resetPassword",
        component: ResetPassword,
        props: route => ({
          userId: route.query.userId,
          token: route.query.token,
        }),
        meta: {
          title: "routes.resetPassword.name",
          guest: true,
        },
      },
      {
        path: i18n.t("routes.dashboard.path"),
        alias: getLocalizedRoutes("routes.dashboard.path"),
        name: "dashboard",
        component: {
          setup() {
            const userStore = useUserStore();
            const comp = userStore.hasRole(Role.Admin) ? Dashboard : MemberDashboard;
            return () => h(comp);
          },
        },
        meta: {
          title: "routes.dashboard.name",
        },
      },
      {
        path: i18n.t("routes.account.path"),
        alias: getLocalizedRoutes("routes.account.path"),
        name: "account",
        component: Account,
        meta: {
          title: "routes.account.name",
        },
      },
      {
        path: i18n.t("routes.admin.path"),
        name: "admin",
        component: Admin,
        meta: {
          requiredRole: Role.Admin,
          noLinkInBreadcrumbs: true,
        },
        children: [
          {
            path: i18n.t("routes.admin.children.members.path"),
            name: "admin.children.members.index",
            component: AdminMemberIndex,
          },
          {
            path: `${i18n.t("routes.admin.children.members.path")}/${i18n.t("routes.admin.children.members.add.path")}`,
            name: "admin.children.members.add",
            component: AdminAddMemberForm,
          },
          {
            path: `${i18n.t("routes.admin.children.members.path")}/${i18n.t("routes.admin.children.members.edit.path")}`,
            name: "admin.children.members.edit",
            component: AdminEditMemberForm,
            props: true,
          },
          {
            path: `${i18n.t("routes.admin.children.members.path")}/${i18n.t("routes.admin.children.members.details.path")}`,
            name: "admin.children.members.details",
            component: AdminMemberDetails,
            props: true,
          },
          {
            path: i18n.t("routes.admin.children.modules.path"),
            name: "admin.children.modules.index",
            component: AdminModuleList,
          },
          {
            path: `${i18n.t("routes.admin.children.modules.path")}/${i18n.t("routes.admin.children.modules.add.path")}`,
            name: "admin.children.modules.add",
            component: AdminAddModule,
          },
          {
            path: `${i18n.t("routes.admin.children.modules.path")}/${i18n.t("routes.admin.children.modules.edit.path")}`,
            name: "admin.children.modules.edit",
            component: AdminModuleEdit,
            props: true,
          },
          {
            path: `${i18n.t("routes.admin.children.modules.path")}/${i18n.t("routes.admin.children.modules.preview.path")}`,
            name: "admin.children.modules.preview",
            component: AdminModulePreview,
            props: true,
          },
          {
            path: "disponibilites",
            name: "admin.children.availability",
            component: AdminAvailability,
          },
          {
            path: i18n.t("routes.admin.children.equipes.path"),
            name: "admin.children.equipes.index",
            component: AdminEquipeList,
          },
          {
            path: `${i18n.t("routes.admin.children.equipes.path")}/${i18n.t("routes.admin.children.equipes.add.path")}`,
            name: "admin.children.equipes.add",
            component: AdminAddEquipeForm,
          },
          {
            path: `${i18n.t("routes.admin.children.equipes.path")}/${i18n.t("routes.admin.children.equipes.edit.path")}`,
            name: "admin.children.equipes.edit",
            component: AdminEditEquipeForm,
            props: true,
          },
          {
            path: `${i18n.t("routes.admin.children.equipes.path")}/details/:id`,
            name: "admin.children.equipes.details",
            component: AdminEquipeDetails,
            props: true,
          },
          {
            path: i18n.t("routes.admin.children.members.quiz.path"),
            name: "admin.children.quiz.index",
            component: AdminQuizIndex,
          },
          {
            path: `${i18n.t("routes.admin.children.members.quiz.path")}/${i18n.t("routes.admin.children.members.quiz.add.path")}`,
            name: "admin.children.quiz.add",
            component: AdminAddQuiz,
          },
          {
            path: `${i18n.t("routes.admin.children.members.quiz.path")}/${i18n.t("routes.admin.children.members.quiz.edit.path")}`,
            name: "admin.children.quiz.edit",
            component: AdminEditQuiz,
            props: true,
          },
        ],
      },
      {
        path: "/mes-modules",
        component: { render: () => h(RouterView) },
        meta: {
          requiredRole: Role.Member,
          title: "Mes modules",
        },
        children: [
          {
            path: "",
            name: "member.modules.index",
            component: MemberModuleList,
          },
          {
            path: ":moduleId",
            component: MemberModuleView,
            props: true,
          },
        ],
      },
      {
        path: i18n.t("routes.quiz.path"),
        alias: getLocalizedRoutes("routes.quiz.path"),
        name: "quiz",
        component: { render: () => h(RouterView) },
        meta: {
          requiredRole: Role.Member,
          title: "routes.quiz.name",
        },
        children: [
          {
            path: "",
            name: "quiz.list",
            component: QuizList,
            meta: {
              title: "routes.quiz.name",
            },
          },
          {
            path: i18n.t("routes.quiz.children.take.path"),
            alias: getLocalizedRoutes("routes.quiz.children.take.path"),
            name: "quiz.take",
            component: QuizTake,
            props: true,
            meta: {
              title: "routes.quiz.children.take.name",
            },
          },
          {
            path: i18n.t("routes.quiz.children.results.path"),
            alias: getLocalizedRoutes("routes.quiz.children.results.path"),
            name: "quiz.results",
            component: QuizResults,
            props: true,
            meta: {
              title: "routes.quiz.children.results.name",
            },
          },
        ],
      },
    ],
  });

  routerInstance.beforeEach(async to => {
    const userStore = useUserStore();
    const isAuthenticated = !!userStore.user.email;

  // Handle root path redirect
  if (to.path === "/") {
    if (!isAuthenticated) return { name: "login" };
    return userStore.hasRole(Role.Admin)
      ? { name: "dashboard" }
      : { name: "dashboard" };
  }

  // Logged-in users cannot access guest-only pages (login, forgot password, etc.)
  if (to.meta.guest && isAuthenticated) {
    return userStore.hasRole(Role.Admin)
      ? { name: "dashboard" }
      : { name: "dashboard" };
  }

    if (!to.meta.guest && !isAuthenticated) {
      return { name: "login" };
    }

    if (!to.meta.requiredRole) {
      return;
    }

  const isRoleArray = Array.isArray(to.meta.requiredRole);
  const doesNotHaveGivenRole =
    !isRoleArray && !userStore.hasRole(to.meta.requiredRole as Role);
  const hasNoRoleAmongRoleList =
    isRoleArray &&
    !userStore.hasOneOfTheseRoles(to.meta.requiredRole as Role[]);
  if (doesNotHaveGivenRole || hasNoRoleAmongRoleList) {
    if (userStore.hasRole(Role.Admin)) {
      return { name: "dashboard" };
    }
    return { name: "dashboard" };
  }
  });

  return routerInstance;
}

export type { Router };
