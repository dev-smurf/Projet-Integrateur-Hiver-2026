<template>
  <button
    class="logout-btn"
    @click="logout"
    :aria-label="t('global.logout')">
    <IconLogout class="logout-btn__icon" :size="20" />
  </button>
</template>

<script lang="ts" setup>
import {useI18n} from "vue3-i18n";
import {useRouter} from "vue-router";
import {useUserStore} from "@/stores/userStore";
import {useMemberStore} from "@/stores/memberStore";
import {useAuthenticationService} from "@/inversify.config";
import {useAdministratorStore} from "@/stores/administratorStore";
import {usePersonStore} from "@/stores/personStore";
import IconLogout from "vue-material-design-icons/Logout.vue";

const {t} = useI18n()
const router = useRouter()
const userStore = useUserStore();
const personStore = usePersonStore();
const memberStore = useMemberStore();
const administratorStore = useAdministratorStore();
const authenticationService = useAuthenticationService()

async function logout() {
  let succeededOrNotResponse = await authenticationService.logout()
  if (succeededOrNotResponse.succeeded) {
    userStore.reset()
    personStore.reset()
    memberStore.reset()
    administratorStore.reset()
    await router.push(t("routes.login.path"))
  }
}
</script>

<style scoped lang="scss">
@use "../../sass/tools" as *;

.logout-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  background: transparent;
  border: none;

  &:hover {
    background: rgba(255, 255, 255, 0.08);
  }

  &:hover .logout-btn__icon,
  &:hover .logout-btn__icon :deep(*) {
    fill: $color-green !important;
  }

  &__icon,
  &__icon :deep(*) {
    fill: #9ca3af !important;
    transition: fill 0.2s;
  }
}
</style>
