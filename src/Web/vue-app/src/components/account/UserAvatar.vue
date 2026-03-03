<template>
  <RouterLink :to="t('routes.account.path')" class="user-avatar">
    <div class="user-avatar__img-container">
      <IconFaceMan class="icon icon--green"/>
    </div>
    <p class="user-avatar__name">{{ userStore.username }}</p>
  </RouterLink>
</template>

<script lang="ts" setup>
import IconFaceMan from 'vue-material-design-icons/FaceMan.vue';
import {useUserStore} from "@/stores/userStore";
import {useI18n} from "vue3-i18n";

const userStore = useUserStore();
const {t} = useI18n();
</script>
