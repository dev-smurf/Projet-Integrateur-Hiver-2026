
<template>
    <div class="min-h-screen bg-gray-100">
        <Transition enter-active-class="transition-opacity duration-200"
                    enter-from-class="opacity-0"
                    enter-to-class="opacity-100"
                    leave-active-class="transition-opacity duration-200"
                    leave-from-class="opacity-100"
                    leave-to-class="opacity-0">
            <div v-if="sidebarOpen"
                 class="fixed inset-0 bg-black/50 z-40 lg:hidden"
                 @click="sidebarOpen = false" />
        </Transition>
        <aside :class="sidebarOpen ? 'translate-x-0' : '-translate-x-full'"
               class="fixed inset-y-0 left-0 z-50 w-60 bg-brand-900 flex flex-col transition-transform duration-200 lg:translate-x-0">
            <div class="px-5 py-5 flex items-center gap-3 border-b border-white/10">
                <div class="w-9 h-9 rounded-xl bg-brand-600 flex items-center justify-center">
                    <LayoutDashboard class="w-5 h-5 text-white" />
                </div>
                <div class="leading-tight">
                    <span class="text-white font-semibold text-sm tracking-wide">Garneau</span>
                    <span class="block text-gray-500 text-[11px]">Plateforme intégrée</span>
                </div>
            </div>

            <!-- Navigation -->
            <nav class="flex-1 overflow-y-auto px-3 py-4 space-y-6">
                <div v-if="userStore.hasRole(Role.Member)">
                    <p class="px-3 mb-2 text-[11px] font-semibold uppercase tracking-widest text-gray-500">
                        {{ $t('routes.dashboard.name') }}
                    </p>
                    <ul class="space-y-0.5">
                        <li>
                            <router-link :to="{ name: 'dashboard' }"
                                         class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                                         :class="isActive('dashboard') ? 'text-white bg-brand-600' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                                         @click="sidebarOpen = false">
                                <LayoutDashboard class="w-4 h-4" />
                                {{ $t('routes.dashboard.name') }}
                            </router-link>
                        </li>
                        <!--li>
                            <router-link :to="{ name: 'books' }"
                                         class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                                         :class="isActive('books') ? 'text-white bg-brand-600' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                                         @click="sidebarOpen = false">
                                <BookOpen class="w-4 h-4" />
                                {{ $t('routes.books.name') }}
                            </router-link>
                        </li-->
                        <li>
                            <router-link :to="{ name: 'equipe' }"
                                         class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                                         :class="isActive('equipe') ? 'text-white bg-brand-600' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                                         @click="sidebarOpen = false">
                                <UsersRound class="w-4 h-4" />
                                {{ $t('routes.equipe.name') }}
                            </router-link>
                        </li>
                        <li>
                            <router-link :to="{ name: 'quiz' }"
                                         class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                                         :class="isActive('quiz') ? 'text-white bg-brand-600' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                                         @click="sidebarOpen = false">
                                <ClipboardCheck class="w-4 h-4" />
                                {{ $t('routes.quiz.name') }}
                            </router-link>
                        </li>
                    </ul>
                </div>

                <!-- Admin section -->
                <div v-if="userStore.hasRole(Role.Admin)">
                    <p class="px-3 mb-2 text-[11px] font-semibold uppercase tracking-widest text-gray-500">
                        {{ $t('routes.admin.name') }}
                    </p>
                    <ul class="space-y-0.5">
                        <li>
                            <router-link :to="{ name: 'adminDashboard' }"
                                         class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                                         :class="isActive('adminDashboard') ? 'text-white bg-brand-600' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                                         @click="sidebarOpen = false">
                                <Shield class="w-4 h-4" />
                                {{ $t('routes.adminDashboard.name') }}
                            </router-link>
                        </li>
                        <li>
                            <router-link :to="{ name: 'admin.children.members.index' }"
                                         class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                                         :class="isActive('admin.children.members') ? 'text-white bg-brand-600' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                                         @click="sidebarOpen = false">
                                <Users class="w-4 h-4" />
                                {{ $t('routes.admin.children.members.name') }}
                            </router-link>
                        </li>
                        <li>
                            <router-link :to="{ name: 'admin.children.modules.index' }"
                                         class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                                         :class="isActive('admin.children.modules') ? 'text-white bg-brand-600' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                                         @click="sidebarOpen = false">
                                <Layers class="w-4 h-4" />
                                {{ $t('routes.admin.children.modules.name') }}
                            </router-link>
                        </li>
                        <li>
                            <router-link :to="{ name: 'admin.children.equipes.index' }"
                                         class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                                         :class="isActive('admin.children.equipes.index') ? 'text-white bg-brand-600' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                                         @click="sidebarOpen = false">
                                <UsersRound class="w-4 h-4" />
                                {{ $t("routes.admin.children.equipes.name") }}
                            </router-link>
                        </li>
                    </ul>
                </div>
            </nav>

            <div class="border-t border-white/10 px-3 py-3 space-y-2">
                <div class="relative">
                    <button @click="langOpen = !langOpen"
                            class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition text-gray-400 hover:text-white hover:bg-white/5 w-full cursor-pointer">
                        <Languages class="w-4 h-4" />
                        {{ $t('global.changeLanguage') }}
                    </button>
                    <div v-if="langOpen"
                         class="absolute bottom-full left-0 mb-1 w-full bg-brand-800 rounded-lg shadow-lg py-1 z-50">
                        <button v-for="loc in LOCALES"
                                :key="loc.value"
                                @click="switchLanguage(loc.value)"
                                class="w-full text-left px-3 py-2 text-sm transition cursor-pointer"
                                :class="currentLocale === loc.value ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'">
                            {{ loc.caption }}
                        </button>
                    </div>
                </div>

                <!-- Profile link -->
                <router-link :to="{ name: 'account' }"
                             class="flex items-center gap-3 px-3 py-2.5 rounded-lg hover:bg-white/5 transition group"
                             @click="sidebarOpen = false">
                    <div class="w-8 h-8 rounded-full bg-brand-600 text-white flex items-center justify-center text-xs font-semibold shrink-0">
                        {{ initials }}
                    </div>
                    <div class="flex-1 min-w-0">
                        <p class="text-sm font-medium text-white truncate">{{ personStore.person.firstName }} {{ personStore.person.lastName }}</p>
                        <p class="text-[11px] text-gray-500 truncate">{{ $t('routes.account.name') }}</p>
                    </div>
                    <button @click.prevent.stop="handleLogout"
                            class="p-1.5 text-gray-500 hover:text-brand-400 rounded-lg hover:bg-white/5 transition cursor-pointer opacity-0 group-hover:opacity-100"
                            :title="$t('global.logout')">
                        <LogOut class="w-4 h-4" />
                    </button>
                </router-link>
            </div>
        </aside>

        <!-- Main area -->
        <div class="flex-1 flex flex-col min-h-screen min-w-0 lg:ml-60">

            <main class="flex-1 max-w-7xl w-full mx-auto px-6 py-8">
                <router-view />
            </main>
        </div>

        <!-- Notifications -->
        <notifications position="bottom right" :duration="4000" :speed="300" width="360">
            <template #body="{ item, close }">
                <div class="mb-3 mr-3 rounded-xl shadow-lg overflow-hidden backdrop-blur-sm"
                     :class="item.type === 'success' ? 'bg-white border border-green-200' : 'bg-white border border-red-200'">
                    <div class="flex items-start gap-3 px-4 py-3">
                        <div class="mt-0.5 w-5 h-5 rounded-full flex items-center justify-center shrink-0"
                             :class="item.type === 'success' ? 'bg-green-100 text-green-600' : 'bg-red-100 text-red-600'">
                            <CheckCircle2 v-if="item.type === 'success'" class="w-3.5 h-3.5" />
                            <XCircle v-else class="w-3.5 h-3.5" />
                        </div>
                        <p class="text-sm text-gray-700 flex-1 leading-snug">{{ item.text }}</p>
                        <button @click="close" class="text-gray-300 hover:text-gray-500 transition shrink-0 mt-0.5">
                            <X class="w-3.5 h-3.5" />
                        </button>
                    </div>
                    <div class="h-0.5 w-full" :class="item.type === 'success' ? 'bg-green-100' : 'bg-red-100'">
                        <div class="h-full toast-progress"
                             :class="item.type === 'success' ? 'bg-green-500' : 'bg-red-500'" />
                    </div>
                </div>
            </template>
        </notifications>

        <ChatBubble />
    </div>
</template>

<script lang="ts" setup>
    import { computed, ref, onMounted, onUnmounted } from "vue";
    import { useRouter } from "vue-router";
    import { useI18n } from "vue3-i18n";
    import Cookies from "universal-cookie";
    import {
        LayoutDashboard, BookOpen, Shield, LogOut, Languages,
        CheckCircle2, XCircle, X, Users, Layers, UsersRound,
        ClipboardCheck, Menu
    } from "lucide-vue-next";
    import { useUserStore } from "@/stores/userStore";
    import { usePersonStore } from "@/stores/personStore";
    import ChatBubble from "@/components/chat/ChatBubble.vue";
    import { useMemberService, useAdministratorService, useAuthenticationService, useConversationService } from "@/inversify.config";
    import { useChatStore } from "@/stores/chatStore";
    import { useSignalR } from "@/composables/useSignalR";
    import { Role } from "@/types/enums";
    import { LOCALES } from "@/locales";

    const router = useRouter();
    const i18nInstance = useI18n();
    const userStore = useUserStore();
    const personStore = usePersonStore();
    const memberService = useMemberService();
    const adminService = useAdministratorService();
    const authService = useAuthenticationService();
    const conversationService = useConversationService();
    const chatStore = useChatStore();
    const { connect: connectSignalR, disconnect: disconnectSignalR } = useSignalR();

    const sidebarOpen = ref(false);
    const langOpen = ref(false);
    const currentLocale = ref(i18nInstance.getLocale());

    const initials = computed(() => {
        const first = personStore.person.firstName || "";
        const last = personStore.person.lastName || "";
        return ((first[0] || "") + (last[0] || "")).toUpperCase();
    });

    function isActive(routePrefix: string): boolean {
        const name = router.currentRoute.value.name as string || "";
        return name === routePrefix || name.startsWith(routePrefix + ".");
    }

    function switchLanguage(lang: string) {
        i18nInstance.setLocale(lang);
        currentLocale.value = lang;
        const cookies = new Cookies();
        cookies.set("lang", lang, { path: "/" });
        langOpen.value = false;
    }

    function handleClickOutside(e: MouseEvent) {
        if (langOpen.value && !(e.target as HTMLElement).closest(".relative")) {
            langOpen.value = false;
        }
    }

    async function handleLogout() {
        chatStore.reset();
        await disconnectSignalR();
        await authService.logout();
        userStore.reset();
        personStore.reset();
        await router.push({ name: "login" });
    }

    onMounted(async () => {
        document.addEventListener("click", handleClickOutside);
        try {
            if (userStore.hasRole(Role.Admin)) {
                const admin = await adminService.getAuthenticated();
                if (admin) personStore.setPerson(admin);
            } else if (userStore.hasRole(Role.Member)) {
                const member = await memberService.getAuthenticated();
                if (member) personStore.setPerson(member);
            }
        } catch {
            // API failed — personStore already has persisted data from login
        }

        // Initialize chat
        await connectSignalR();
        try {
            const unreadCount = await conversationService.getUnreadCount();
            chatStore.setUnreadCount(unreadCount);
        } catch {
            // Chat unavailable — non-blocking
        }
    });

    onUnmounted(async () => {
        document.removeEventListener("click", handleClickOutside);
        await disconnectSignalR();
    });
</script>