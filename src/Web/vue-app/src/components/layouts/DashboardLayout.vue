<template>
  <div class="app-layout">
    <!-- Top navbar -->
    <header class="app-header">
      <div class="app-header__inner">
        <div class="app-header__left">
          <nav class="app-header__nav" v-if="!isMobile && !userIsLoading">
            <AdminNavbarLinks v-if="userStore.hasRole(Role.Admin)"/>
            <MemberNavbarLinks v-if="userStore.hasRole(Role.Member)"/>
          </nav>
        </div>

        <div class="app-header__right">
          <LangSwitcher/>
          <div class="app-header__user" v-if="!isMobile">
            <UserAvatar/>
          </div>
          <LogoutButton classes="app-header__logout"/>
        </div>
      </div>

      <!-- Mobile nav -->
      <nav class="app-header__mobile-nav" v-if="isMobile && !userIsLoading">
        <AdminNavbarLinks v-if="userStore.hasRole(Role.Admin)"/>
        <MemberNavbarLinks v-if="userStore.hasRole(Role.Member)"/>
      </nav>
    </header>

    <!-- Main content -->
    <main class="app-main">
      <LogoutPopup/>
      <Notifications/>

      <RouterView v-slot="{Component}">
        <template v-if="Component">
          <Suspense>
            <component :is="Component"/>
            <template #fallback>
              <Loader/>
            </template>
          </Suspense>
        </template>
      </RouterView>
    </main>
  </div>
</template>

<script setup lang="ts">
import {onMounted, ref, computed} from "vue";
import {useMemberStore} from "@/stores/memberStore";
import {useAdministratorService, useMemberService} from "@/inversify.config";
import LogoutPopup from "@/components/layouts/items/LogoutPopup.vue";
import Notifications from "@/components/layouts/items/Notifications.vue";
import Loader from "@/components/layouts/items/Loader.vue";
import {useWindowSize} from "vue-window-size";
import UserAvatar from "@/components/account/UserAvatar.vue";
import LangSwitcher from "@/components/layouts/items/LangSwitcher.vue";
import LogoutButton from "@/components/navigation/LogoutButton.vue";
import AdminNavbarLinks from "@/components/navigation/AdminNavbarLinks.vue";
import MemberNavbarLinks from "@/components/navigation/MemberNavbarLinks.vue";
import {Administrator, Member} from "@/types";
import {Role} from "@/types/enums";
import {useAdministratorStore} from "@/stores/administratorStore";
import {usePersonStore} from "@/stores/personStore";
import {useUserStore} from "@/stores/userStore";

const userStore = useUserStore()
const personStore = usePersonStore()
const memberStore = useMemberStore()
const administratorStore = useAdministratorStore()

const memberService = useMemberService();
const administratorService = useAdministratorService();

const userIsLoading = ref(true)

const {width} = useWindowSize();
const isMobile = computed(() => width.value < 768);

onMounted(async () => {
  userIsLoading.value = true
  if (userStore.hasRole(Role.Member)) {
    let member = await memberService.getAuthenticated() as Member;
    personStore.setPerson(member)
    memberStore.setMember(member)
  } else {
    let administrator = await administratorService.getAuthenticated() as Administrator;
    personStore.setPerson(administrator)
    administratorStore.setAdministrator(administrator)
  }
  userIsLoading.value = false
});
</script>

<style scoped lang="scss">
@use "../../sass/tools" as *;

.app-layout {
  min-height: 100vh;
  background: $color-grey-lighter;
}

.app-header {
  background: $color-black !important;
  border-bottom: 1px solid $color-grey-dark !important;
  position: sticky;
  top: 0;
  z-index: 1000;

  &__inner {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 24px;
    height: 60px;
    max-width: 1200px;
    margin: 0 auto;

    @media (min-width: 768px) {
      padding: 0 40px;
    }
  }

  &__left {
    display: flex;
    align-items: center;
    gap: 32px;
  }

  &__logo {
    font-family: $font-montserrat;
    font-weight: 800;
    font-size: 20px;
    color: $color-green !important;
    letter-spacing: -0.03em;
  }

  &__nav {
    display: flex;
    align-items: center;
    gap: 4px;
  }

  &__right {
    display: flex;
    align-items: center;
    gap: 20px;
  }

  &__user {
    color: $color-grey;
  }

  &__logout {
    font-size: 13px;
    font-weight: 600;
    color: $color-grey-medium !important;
    transition: color 0.15s;
    cursor: pointer;

    &:hover {
      color: $color-white !important;
    }
  }

  &__mobile-nav {
    display: flex;
    gap: 4px;
    padding: 8px 24px 12px;
    overflow-x: auto;
  }
}

.app-main {
  max-width: 1200px;
  margin: 0 auto;
  padding: 32px 24px 48px;

  @media (min-width: 768px) {
    padding: 40px 40px 60px;
  }
}
</style>
