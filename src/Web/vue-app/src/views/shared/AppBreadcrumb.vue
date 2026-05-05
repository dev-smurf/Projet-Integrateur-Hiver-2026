<template>
    <nav v-if="breadcrumbs.length > 1"
         class="flex items-center gap-1 text-sm text-gray-500 mb-4 flex-wrap">
        <template v-for="(item, index) in breadcrumbs" :key="index">
            <router-link v-if="item.to"
                         :to="item.to"
                         class="hover:text-brand-600 transition truncate max-w-[200px]">
                {{ item.label }}
            </router-link>
            <span v-else class="text-gray-900 font-medium truncate max-w-[200px]">
                {{ item.label }}
            </span>
            <ChevronRight v-if="index < breadcrumbs.length - 1"
                          class="w-3.5 h-3.5 shrink-0 text-gray-400" />
        </template>
    </nav>
</template>

<script lang="ts" setup>
    import { ChevronRight } from "lucide-vue-next";
    import { useBreadcrumb } from "@/composables/useBreadcrumb";

    const { breadcrumbs } = useBreadcrumb();
</script>