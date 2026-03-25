<template>
  <div>
    <!-- Tabs -->
    <div class="flex gap-1 mb-6 border-b border-gray-200">
      <router-link
        :to="{ name: 'admin.children.members.index' }"
        class="px-4 py-2.5 text-sm font-medium rounded-t-lg transition -mb-px"
        :class="isActive('members') ? 'text-brand-600 border-b-2 border-brand-600 bg-white' : 'text-gray-500 hover:text-gray-700'"
      >
        {{ $t('routes.admin.children.members.name') }}
      </router-link>
      <router-link
        :to="{ name: 'admin.children.modules.index' }"
        class="px-4 py-2.5 text-sm font-medium rounded-t-lg transition -mb-px"
        :class="isActive('modules') ? 'text-brand-600 border-b-2 border-brand-600 bg-white' : 'text-gray-500 hover:text-gray-700'"
      >
        {{ $t('routes.admin.children.modules.name') }}
      </router-link>
      <router-link
        :to="{ name: 'admin.children.quiz.index' }"
        class="px-4 py-2.5 text-sm font-medium rounded-t-lg transition -mb-px"
        :class="isActive('quiz') ? 'text-brand-600 border-b-2 border-brand-600 bg-white' : 'text-gray-500 hover:text-gray-700'"
      >
        {{ $t('routes.admin.children.members.quiz.name') }}
      </router-link>
    </div>

    <router-view :key="$route.fullPath" />
  </div>
</template>

<script lang="ts" setup>
import {useRouter} from "vue-router";

const router = useRouter();

function isActive(section: string): boolean {
  const name = router.currentRoute.value.name as string || "";
  return name.includes(section);
}
</script>
