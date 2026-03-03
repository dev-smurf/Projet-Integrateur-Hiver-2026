<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="bg-brand-900 sticky top-0 z-50">
      <div class="max-w-7xl mx-auto px-6">
        <div class="flex items-center justify-between h-14">
          <!-- Left: nav links -->
          <div class="flex items-center gap-1">
            <router-link
              :to="{ name: 'dashboard' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('dashboard') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <LayoutDashboard class="w-4 h-4" />
              {{ $t('routes.dashboard.name') }}
            </router-link>
            <router-link
              v-if="userStore.hasRole(Role.Member)"
              :to="{ name: 'books' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('books') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <BookOpen class="w-4 h-4" />
              {{ $t('routes.books.name') }}
            </router-link>
          </div>

          <!-- Right: language, admin, profile, logout -->
          <div class="flex items-center gap-3">
            <!-- Language dropdown -->
            <div class="relative">
              <button
                @click="langOpen = !langOpen"
                class="flex items-center gap-1 px-2 py-1.5 text-sm text-gray-400 hover:text-white rounded-lg hover:bg-white/5 transition"
              >
                <Languages class="w-4 h-4" />
                <span class="uppercase text-xs font-semibold">{{ currentLocale }}</span>
              </button>
              <div
                v-if="langOpen"
                class="absolute right-0 mt-1 w-36 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-50"
              >
                <button
                  v-for="loc in LOCALES.filter(l => l.value !== currentLocale)"
                  :key="loc.value"
                  @click="switchLanguage(loc.value)"
                  class="w-full text-left px-3 py-2 text-sm text-gray-700 hover:bg-gray-50 transition"
                >
                  {{ loc.caption }}
                </button>
              </div>
            </div>

            <!-- Admin panel -->
            <router-link
              v-if="userStore.hasRole(Role.Admin)"
              :to="{ name: 'admin.children.members.index' }"
              class="p-1.5 text-gray-400 hover:text-white rounded-lg hover:bg-white/5 transition"
              :class="isActive('admin') ? 'text-white bg-white/10' : ''"
            >
              <Shield class="w-4 h-4" />
            </router-link>

            <!-- Profile -->
            <router-link
              :to="{ name: 'account' }"
              class="flex items-center gap-2 text-sm text-gray-400 hover:text-white transition"
            >
              <div class="w-7 h-7 rounded-full bg-brand-600 text-white flex items-center justify-center text-xs font-semibold">
                {{ initials }}
              </div>
              <span class="hidden sm:inline">{{ personStore.person.firstName }} {{ personStore.person.lastName }}</span>
            </router-link>

            <!-- Logout -->
            <button
              @click="handleLogout"
              class="flex items-center gap-1 text-sm text-gray-500 hover:text-brand-400 transition"
            >
              <LogOut class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>
    </nav>

    <main class="max-w-7xl mx-auto px-6 py-8">
      <router-view />
    </main>

    <notifications position="bottom right" />
  </div>
</template>

<script lang="ts" setup>
import {computed, ref, onMounted, onUnmounted} from "vue";
import {useRouter} from "vue-router";
import {useI18n} from "vue3-i18n";
import Cookies from "universal-cookie";
import {LayoutDashboard, BookOpen, Shield, LogOut, Languages} from "lucide-vue-next";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {useMemberService, useAdministratorService, useAuthenticationService} from "@/inversify.config";
import {Role} from "@/types/enums";
import {LOCALES} from "@/locales";

const router = useRouter();
const i18nInstance = useI18n();
const userStore = useUserStore();
const personStore = usePersonStore();
const memberService = useMemberService();
const adminService = useAdministratorService();
const authService = useAuthenticationService();

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
  cookies.set("lang", lang, {path: "/"});
  langOpen.value = false;
}

function handleClickOutside(e: MouseEvent) {
  if (langOpen.value && !(e.target as HTMLElement).closest(".relative")) {
    langOpen.value = false;
  }
}

async function handleLogout() {
  await authService.logout();
  userStore.reset();
  personStore.reset();
  await router.push({name: "login"});
}

onMounted(async () => {
  document.addEventListener("click", handleClickOutside);
  if (userStore.hasRole(Role.Admin)) {
    const admin = await adminService.getAuthenticated();
    if (admin) personStore.setPerson(admin);
  } else if (userStore.hasRole(Role.Member)) {
    const member = await memberService.getAuthenticated();
    if (member) personStore.setPerson(member);
  }
});

onUnmounted(() => {
  document.removeEventListener("click", handleClickOutside);
});
</script>
