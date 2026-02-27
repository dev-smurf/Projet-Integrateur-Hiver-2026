<template>
  <div>
    <Notifications :is-in-page-flow="true" />

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
  </div>
</template>
<script setup lang="ts">
import Loader from "@/components/layouts/items/Loader.vue";
import Notifications from "@/components/layouts/items/Notifications.vue";
</script>
