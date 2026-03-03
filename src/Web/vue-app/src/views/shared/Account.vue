<template>
  <template v-if="userStore">
    <AdminAccount v-if="userStore.hasRole(Role.Admin)" />
    <!--<MemberAccount v-else-if="userStore.hasRole(Role.Member)" />-->
    <MemberHome v-if="userStore.hasRole(Role.Member)"/>
  </template>
</template>

<script lang="ts" setup>
import { Role } from "@/types/enums";
    //import MemberAccount from "@/views/member/MemberAccount.vue";
import MemberHome from "@/views/member/MemberHome.vue"
import AdminAccount from "@/views/admin/AdminAccount.vue";
import { useUserStore } from "@/stores/userStore";

const userStore = useUserStore()
</script>