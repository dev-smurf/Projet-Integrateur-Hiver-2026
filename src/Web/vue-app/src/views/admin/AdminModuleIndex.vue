<template>
  <div class="content-grid content-grid--subpage content-grid--subpage-table">
    <div class="content-grid__header">
      <h1>{{ t("routes.admin.children.modules.name") }}</h1>
    </div>

      <div class="breadcrumb">
        <router-link :to="{ name: 'admin' }">{{ t('routes.admin.name') }}</router-link>
        <span class="sep">/</span>
        <span class="current">{{ t('routes.admin.children.modules.name') }}</span>
      </div>

      <Loader v-if="moduleStore.loading" />

      <div v-if="moduleStore.error" class="error-message">
        {{ moduleStore.error }}
      </div>

      <div v-if="!moduleStore.loading && moduleStore.modules.length > 0" class="modules-grid">
        <div v-for="mod in modulesWithImages" :key="mod.id" class="module-card">
          <div class="module-image">
            <img :src="mod.imageUrl" :alt="mod.name" />
          </div>
          <div class="module-content">
            <h3 class="module-title">{{ mod.name }}</h3>
            <p class="module-desc">{{ mod.description }}</p>
            <div class="module-actions">
              <button class="btn-detail" @click="openDetail(mod)">Detail</button>
            </div>
          </div>
        </div>
      </div>

      <div v-else-if="!moduleStore.loading" class="empty-message">
        Aucun module trouvé.
      </div>

     
      <button class="fab" aria-label="Ajouter un module">+</button>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from "vue";
import { useI18n } from "vue3-i18n";
import { useModuleStore } from "@/stores/moduleStore";
import Loader from "@/components/layouts/items/Loader.vue";
import { useRouter } from "vue-router";
import "../../sass/blocks/AdminModuleIndex.scss";

const { t } = useI18n();
const moduleStore = useModuleStore();

const router = useRouter();

const modulesWithImages = computed(() => {
 
  return moduleStore.modules.map((m, idx) => ({
    ...m,
    imageUrl:
      m.imageUrl ||
      
      [
        "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=1200&auto=format&fit=crop&q=80",
        "https://images.unsplash.com/photo-1512820790803-83ca734da794?w=1200&auto=format&fit=crop&q=80",
        "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=1200&auto=format&fit=crop&q=80",
        "https://images.unsplash.com/photo-1518779578993-ec3579fee39f?w=1200&auto=format&fit=crop&q=80",
        "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=1200&auto=format&fit=crop&q=80",
      ][idx % 5],
  }));
});

function openDetail(mod: any) {

  router.push({ path: `/administration/modules#${mod.id}` }).catch(() => {});
}

onMounted(async () => {
  await moduleStore.loadModules();
});
</script>

