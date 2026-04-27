<template>
    <div class="min-h-screen bg-gray-100 flex">

        <!-- Sidebar -->
        <nav class="sticky top-0 h-screen w-64 shrink-0 flex flex-col backdrop-blur border-r border-white/10 z-50"
             style="background-color: #4c6367;">

            <div class="flex items-center h-16 px-6 border-b border-white/10">
                <span class="font-semibold text-lg" style="color: #98ff98;">Mon App</span>
            </div>

            <div class="flex-1 overflow-y-auto px-3 py-4 flex flex-col gap-1">
                <router-link :to="{ name: 'dashboard' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('dashboard') ? 'active-link' : 'inactive-link'">
                    <LayoutDashboard class="w-4 h-4 shrink-0" />
                    {{ $t('routes.dashboard.name') }}
                </router-link>

                <router-link v-if="userStore.hasRole(Role.Member)"
                             :to="{ name: 'member.modules.index' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('member.modules') ? 'active-link' : 'inactive-link'">
                    <BookOpen class="w-4 h-4 shrink-0" />
                    Mes modules
                </router-link>

                <router-link v-if="userStore.hasRole(Role.Member)"
                             :to="{ name: 'equipe' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('equipe') ? 'active-link' : 'inactive-link'">
                    <UsersRound class="w-4 h-4 shrink-0" />
                    {{ $t('routes.equipe.name') }}
                </router-link>

                <router-link v-if="userStore.hasRole(Role.Member)"
                             :to="{ name: 'quiz.list' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('quiz') ? 'active-link' : 'inactive-link'">
                    <ClipboardCheck class="w-4 h-4 shrink-0" />
                    {{ $t('routes.quiz.name') }}
                </router-link>

                <div v-if="userStore.hasRole(Role.Admin)" class="my-2 border-t" style="border-color: #907288;" />

                <router-link v-if="userStore.hasRole(Role.Admin)"
                             :to="{ name: 'admin.children.members.index' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('admin.children.members') ? 'active-link' : 'inactive-link'">
                    <Users class="w-4 h-4 shrink-0" />
                    {{ $t('routes.admin.children.members.name') }}
                </router-link>

                <router-link v-if="userStore.hasRole(Role.Admin)"
                             :to="{ name: 'admin.children.modules.index' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('admin.children.modules') ? 'active-link' : 'inactive-link'">
                    <Layers class="w-4 h-4 shrink-0" />
                    {{ $t('routes.admin.children.modules.name') }}
                </router-link>

                <router-link v-if="userStore.hasRole(Role.Admin)"
                             :to="{ name: 'admin.children.equipes.index' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('admin.children.equipes') ? 'active-link' : 'inactive-link'">
                    <UsersRound class="w-4 h-4 shrink-0" />
                    {{ $t("routes.admin.children.equipes.name") }}
                </router-link>
                <router-link v-if="userStore.hasRole(Role.Admin)"
                             :to="{ name: 'admin.children.availability' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('availability') ? 'active-link' : 'inactive-link'">
                    <Calendar class="w-4 h-4 shrink-0" />
                    {{ $t('appointment.availability') }}
                </router-link>
                <router-link v-if="userStore.hasRole(Role.Admin)"
                             :to="{ name: 'admin.children.quiz.index' }"
                             class="flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition"
                             :class="isActive('quiz') ? 'active-link' : 'inactive-link'">
                    <Brain class="w-4 h-4 shrink-0" />
                    {{ $t('routes.admin.children.members.quiz.name') }}
                </router-link>
            </div>

            <div class="border-t px-3 py-4 flex flex-col gap-1" style="border-color: #907288;">

                <div class="relative">
                    <button @click="langOpen = !langOpen"
                            class="w-full flex items-center gap-3 px-3 py-2 rounded-lg transition text-sm font-medium"
                            style="color: #d1d5db;"
                            @mouseover="e => e.currentTarget.style.color='#98ff98'"
                            @mouseleave="e => e.currentTarget.style.color='#d1d5db'">
                        <Languages class="h-4 w-4 shrink-0" />
                        Langue
                    </button>
                    <div v-if="langOpen"
                         class="absolute bottom-full left-0 mb-2 min-w-[120px] rounded-lg py-1 shadow-lg"
                         style="background-color: #3a4f52;">
                        <button v-for="loc in LOCALES"
                                :key="loc.value"
                                @click="switchLanguage(loc.value)"
                                class="w-full px-3 py-2 text-left text-sm transition"
                                :style="currentLocale === loc.value
                ? 'color: #98ff98; background-color: rgba(152,255,152,0.1);'
                : 'color: #d1d5db;'">
                            {{ loc.caption }}
                        </button>
                    </div>
                </div>

                <!-- Profil -->
                <router-link :to="{ name: 'account' }"
                             class="flex items-center gap-3 px-3 py-2 rounded-xl transition hover-profile">
                    <div class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full text-sm font-semibold"
                         style="background-color: #907288; color: white;">
                        {{ initials }}
                    </div>
                    <div class="min-w-0">
                        <p class="truncate text-sm font-medium" style="color: white;">
                            {{ personStore.person.firstName }} {{ personStore.person.lastName }}
                        </p>
                        <p class="truncate text-xs" style="color: #98ff98;">{{ $t('routes.account.name') }}</p>
                    </div>
                </router-link>

                <!-- Logout -->
                <button @click="handleLogout"
                        class="w-full flex items-center gap-3 px-3 py-2 rounded-lg transition text-sm font-medium logout-btn"
                        :title="$t('global.logout')"
                        style="color: #d1d5db;">
                    <LogOut class="h-4 w-4 shrink-0" />
                    {{ $t('global.logout') }}
                </button>
            </div>
        </nav>

        <!-- Contenu principal -->
        <div class="flex-1 min-w-0">
            <main class="w-full px-6 py-8">
                <router-view />
            </main>
        </div>

        <!-- Notifications -->
        <notifications position="bottom right" :duration="4000" :speed="300" width="360">
            <template #body="{ item, close }">
                <div class="mb-3 mr-3 overflow-hidden rounded-xl shadow-lg backdrop-blur-sm"
                     :class="item.type === 'success' ? 'border border-green-200 bg-white' : 'border border-red-200 bg-white'">
                    <div class="flex items-start gap-3 px-4 py-3">
                        <div class="mt-0.5 flex h-5 w-5 shrink-0 items-center justify-center rounded-full"
                             :class="item.type === 'success' ? 'bg-green-100 text-green-600' : 'bg-red-100 text-red-600'">
                            <CheckCircle2 v-if="item.type === 'success'" class="h-3.5 w-3.5" />
                            <XCircle v-else class="h-3.5 w-3.5" />
                        </div>
                        <p class="flex-1 text-sm leading-snug text-gray-700">{{ item.text }}</p>
                        <button @click="close" class="mt-0.5 shrink-0 text-gray-300 transition hover:text-gray-500">
                            <X class="h-3.5 w-3.5" />
                        </button>
                    </div>
                    <div class="h-0.5 w-full" :class="item.type === 'success' ? 'bg-green-100' : 'bg-red-100'">
                        <div class="toast-progress h-full"
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
        LayoutDashboard, BookOpen, Shield, LogOut, Languages, Bell,
        CheckCircle2, XCircle, X, Users, Layers, UsersRound,
        ClipboardCheck,Brain,Calendar
    } from "lucide-vue-next";
    import { useUserStore } from "@/stores/userStore";
    import { usePersonStore } from "@/stores/personStore";
    import ChatBubble from "@/components/chat/ChatBubble.vue";
    import { useMemberService, useAdministratorService, useAuthenticationService, useConversationService } from "@/inversify.config";
    import { useChatStore } from "@/stores/chatStore";
    import { useSignalR } from "@/composables/useSignalR";
    import { Role } from "@/types/enums";
    import { LOCALES } from "@/locales";
    import { hasUnreadMemberAdminNote, MEMBER_ADMIN_NOTE_READ_EVENT } from "@/utils/memberAdminNotes";

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

    const langOpen = ref(false);
    const currentLocale = ref(i18nInstance.getLocale());
    const noteReadVersion = ref(0);

    const initials = computed(() => {
        const first = personStore.person.firstName || "";
        const last = personStore.person.lastName || "";
        return ((first[0] || "") + (last[0] || "")).toUpperCase();
    });

    const memberNotificationCount = computed(() => {
        noteReadVersion.value;
        if (!userStore.hasRole(Role.Member)) return 0;
        const memberIdentifier = userStore.user.email || userStore.username || "";
        return hasUnreadMemberAdminNote(memberIdentifier, personStore.person.visibleAdminNotes) ? 1 : 0;
    });

    function onMemberNoteRead() {
        noteReadVersion.value += 1;
    }

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
        window.addEventListener(MEMBER_ADMIN_NOTE_READ_EVENT, onMemberNoteRead);
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
        window.removeEventListener(MEMBER_ADMIN_NOTE_READ_EVENT, onMemberNoteRead);
        await disconnectSignalR();
    });
</script>
<style scoped>
    .active-link {
        color: #98ff98;
        background-color: rgba(152, 255, 152, 0.15);
    }

    .inactive-link {
        color: #d1d5db;
    }

        .inactive-link:hover {
            color: #98ff98;
            background-color: rgba(144, 114, 136, 0.2);
        }

    .logout-btn:hover {
        color: #98ff98;
        background-color: rgba(144, 114, 136, 0.2);
    }

    .hover-profile:hover {
        background-color: rgba(144, 114, 136, 0.2);
    }
</style>
