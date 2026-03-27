<template>
  <div>
    <router-view :key="$route.fullPath" />
  </div>
</template>

<script lang="ts" setup>
import { useRouter } from "vue-router";

const router = useRouter();

function isActive(section: string): boolean {
  const name = (router.currentRoute.value.name as string) || "";
  return name.startsWith(`admin.children.${section}`);
}
</script>
