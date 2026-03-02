import { Role } from "@/types/enums";
import { createRouter, createWebHistory } from "vue-router";

// Imports des vues
import Login from "@/views/Login.vue";
import TwoFactor from "@/views/TwoFactor.vue";
import ForgotPassword from "@/views/ForgotPassword.vue";
import ResetPassword from "@/views/ResetPassword.vue";
import Account from "@/views/shared/Account.vue";

// Administration
import Admin from "../views/admin/Admin.vue";
import AdminMemberIndex from "@/views/admin/members/AdminMemberIndex.vue";
import AdminAddMemberForm from "@/views/admin/members/AdminAddMemberForm.vue";
import AdminEditMemberForm from "@/views/admin/members/AdminEditMemberForm.vue";
import AdminModuleList from "@/views/admin/members/AdminModuleList.vue";
import addModule from "@/views/admin/members/AdminAddModule.vue";
import EditModuleForm from "@/views/admin/members/EditModuleForm.vue";

// Livres
import Books from "../views/member/Books.vue";
import BookIndex from "@/views/member/BookIndex.vue";
import AddBookForm from "@/views/member/AddBookForm.vue";
import EditBookForm from "@/views/member/EditBookForm.vue";

import { useUserStore } from "@/stores/userStore";

const router = createRouter({
    scrollBehavior(to, from, savedPosition) {
        return { top: 0 };
    },
    history: createWebHistory(),
    routes: [
        {
            path: "/connexion",
            name: "login",
            component: Login,
            meta: { title: "routes.login.name" }
        },
        {
            path: "/ajouter-module",
            name: "addModule",
            component: addModule,
            meta: { title: "routes.addModule.name" }
        },
        {
            path: "/authentification-a-deux-facteurs",
            name: "twoFactor",
            component: TwoFactor,
            meta: { title: "routes.twoFactor.name" }
        },
        {
            path: "/mon-compte",
            name: "account",
            component: Account,
            meta: { title: "routes.account.name" }
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
                // SECTION MEMBRES
                {
                    path: "membres",
                    name: "admin.children.members",
                    children: [
                        {
                            path: "",
                            name: "admin.children.members.index",
                            component: AdminMemberIndex,
                        },
                        {
                            path: "ajouter",
                            name: "admin.children.members.add",
                            component: AdminAddMemberForm,
                        },
                        {
                            path: ":id/modifier",
                            name: "admin.children.members.edit",
                            component: AdminEditMemberForm,
                            props: true
                        }
                    ],
                },
                // SECTION MODULES
                {
                    path: "modules",
                    name: "admin.children.modules",
                    children: [
                        {
                            path: "",
                            name: "admin.children.modules.index", // Match avec routes.admin.children.modules.index.name
                            component: AdminModuleList,
                        },
                        {
                            path: "modifier/:id",
                            name: "admin.children.modules.edit", // Match avec routes.admin.children.modules.edit.name
                            component: EditModuleForm,
                            props: true
                        }
                    ]
                }
            ]
        },
        {
            path: "/livres",
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
                },
                {
                    path: "ajouter",
                    name: "books.children.add",
                    component: AddBookForm,
                },
                {
                    path: ":id/modifier",
                    name: "books.children.edit",
                    component: EditBookForm,
                    props: true
                }
            ]
        },
        // Redirection par d�faut vers l'accueil/login
        {
            path: "/:pathMatch(.*)*",
            redirect: "/connexion"
        }
    ]
});

router.beforeEach(async (to, from) => {
    const userStore = useUserStore();

    // Redirection de la racine
    if (to.path === "/") {
        return userStore.user?.email ? { name: "account" } : { name: "login" };
    }

    // Protection des routes par r�le
    if (!to.meta.requiredRole) return;

    const hasRole = userStore.hasRole(to.meta.requiredRole as Role);

    if (!hasRole) {
        return { name: "account" };
    }
});

export const Router = router;
export default router;