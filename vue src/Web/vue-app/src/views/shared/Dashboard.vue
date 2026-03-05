<template>
    <template v-if="userStore.user?.email">
        <AdminAccount v-if="userStore.hasRole(Role.Admin)" />
        <MemberHome v-else-if="userStore.hasRole(Role.Member)" />
    </template>
</template>

<script lang="ts" setup>import AdminAccount from "@/views/admin/AdminAccount.vue";
import MemberHome from "@/views/member/MemberHome.vue";
import { Role } from "@/types/enums";
import { useUserStore } from "@/stores/userStore";
import { usePersonStore } from "@/stores/personStore";

const userStore = useUserStore();
const personStore = usePersonStore();</script>