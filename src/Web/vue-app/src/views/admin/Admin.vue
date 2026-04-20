<template>
  <div>
    <!-- Tabs -->
      <div>
          <nav class="flex items-center gap-2 mb-6 text-sm">
              <router-link :to="{ name: 'dashboard' }"
                           class="text-gray-500 hover:text-brand-600 transition flex items-center gap-1">
                  <Home class="w-4 h-4" />
                  {{ $t('routes.dashboard.name') }}
              </router-link>

              <ChevronRight class="w-4 h-4 text-gray-300" />

              <span class="text-gray-700 font-medium">
                  {{ currentPageLabel }}
              </span>
          </nav>
      </div>

    <router-view :key="$route.fullPath" />
  </div>
</template>

<script lang="ts" setup>
    import { useRouter } from "vue-router";
    import { computed } from "vue";
    import { useI18n } from "vue3-i18n";
    import { Home, ChevronRight } from "lucide-vue-next";

    const router = useRouter();
    const { t } = useI18n();

    const routeLabels: Record<string, string> = {
        "admin.children.members": t("routes.admin.children.members.name"),
        "admin.children.modules": t("routes.admin.children.modules.name"),
        "admin.children.equipes": t("routes.admin.children.equipes.name"),
        "admin.children.availability": t("appointment.availability"),
        "admin.children.quiz": t("routes.admin.children.members.quiz.name"),
    };

    const currentPageLabel = computed(() => {
        const routeName = (router.currentRoute.value.name as string) || "";

        // Chercher une correspondance dans routeLabels
        for (const [key, label] of Object.entries(routeLabels)) {
            if (routeName.startsWith(key)) {
                return label;
            }
        }

        return "Admin";
    });
</script>
