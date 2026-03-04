<template>
    <!--<div>
      <h1 class="text-2xl font-bold text-gray-900 mb-2">
        {{ $t('pages.dashboard.welcome') }}, {{ personStore.person.fullName || userStore.user.fullName }}
      </h1>

    </div>-->
    <template v-if="userStore">
        <AdminAccount v-if="userStore.hasRole(Role.Admin)" />
        <!--<MemberAccount v-else-if="userStore.hasRole(Role.Member)" />-->
        <MemberHome v-if="userStore.hasRole(Role.Member)" />
    </template>
</template>

<script lang="ts" setup>
import { Role } from "@/types/enums";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";

const userStore = useUserStore();
const personStore = usePersonStore();
</script>
