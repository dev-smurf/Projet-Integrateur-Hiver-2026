<template>
<<<<<<< HEAD
  <div class="content-grid">
    <div class="content-grid__header">
      <h1>{{ t(`routes.account.name`) }}</h1>
    </div>

    <div class="content-grid content-grid--double">
      <div class="content-grid__header">
        <h2>{{t('pages.account.loginInfos')}}</h2>
      </div>

      <p>TODO: Add change password form</p>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useI18n } from "vue3-i18n";

const {t} = useI18n();
</script>
=======
    <!--<div class="content-grid">
        <div class="content-grid__header">
            <h1>{{ t(`routes.account.name`) }}</h1>
        </div>

        <div class="content-grid content-grid--double">
            <div class="content-grid__header">
                <h2>{{t('pages.account.loginInfos')}}</h2>
            </div>

            <p>TODO: Add change password form</p>
        </div>
    </div>-->
    <header class="hero-header">
        <MemberNavbarItems />
        <div class="hero-content">
            <div class="container">
                <h1>
                    L’équilibre mental est le fondement de votre santé.<br>
                    Donnez à votre esprit l’expertise médicale qu’il mérite.
                </h1>
                <a href="#" class="btn btn-start btn-lg rounded-pill mt-3">
                    Get started
                </a>
            </div>
        </div>
    </header>
</template>

<script lang="ts" setup>
    import { useI18n } from "vue3-i18n";
    import MemberNavbarItems from "@/components/navigation/MemberNavbarItems.vue";

    const { t } = useI18n();
</script>
<style>
    .hero-header {
        position: relative;
        min-height: 70vh;
        background: linear-gradient(rgba(0,0,0,0.55), rgba(0,0,0,0.65)), url('https://images.unsplash.com/photo-1516321497487-e288fb19713f?q=80&w=2070&auto=format&fit=crop') center center / cover no-repeat;
        background-attachment: fixed;
    }

    .hero-content {
        min-height: 60vh;
        display: flex;
        align-items: center;
        justify-content: center;
        text-align: center;
        color: white;
        padding: 2rem 1rem;
    }

        .hero-content h1 {
            font-size: 2.6rem;
            font-weight: 700;
            margin-bottom: 1.5rem;
            line-height: 1.2;
            text-shadow: 0 2px 12px rgba(0,0,0,0.7);
        }
</style>
>>>>>>> 194fe738189c29a0db951be3c1c84e6dfcd46df1
