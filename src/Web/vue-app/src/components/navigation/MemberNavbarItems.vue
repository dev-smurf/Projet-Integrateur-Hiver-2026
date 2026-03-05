<!--<template>
  <ul class="navbar__nav">
    <li>
      <RouterLink :to="t('routes.account.path')" class="navbar__navlink">
        {{ t('routes.account.name') }}
      </RouterLink>
    </li>
    <li>
      <RouterLink :to="t('routes.books.path')" class="navbar__navlink">
        {{ t('routes.books.name') }}
      </RouterLink>
    </li>
  </ul>
</template>-->
<template>
    <nav class="navbar navbar-expand-lg navbar-dark fixed-top">
        <div class="container">
            <a class="navbar-brand fw-bold fs-5" href="#">
                <span style="color: #28a745;">logo</span> 
            </a>

            <button class="navbar-toggler" type="button" data-bs-toggle="collapse"
                    data-bs-target="#navbarNav" aria-controls="navbarNav"
                    aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>

            <div class="collapse navbar-collapse justify-content-end" id="navbarNav">
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <RouterLink :to="t('routes.membres.navigation.accueil.path')" class="nav-link active">
                            {{t('routes.membres.navigation.accueil.name')}}
                        </RouterLink>
                    </li>
                    <li class="nav-item">
                        <RouterLink :to="t('routes.membres.navigation.progression.path')" class="nav-link">
                            {{t('routes.membres.navigation.progression.name')}}
                        </RouterLink>
                    </li>
                    <li class="nav-item">
                        <RouterLink :to="t('routes.modules.path')" class="nav-link">
                            {{t('routes.modules.name')}}
                        </RouterLink>
                    </li>
                    <li class="nav-item">
                        <RouterLink :to="t('routes.membres.navigation.quiz.path')" class="nav-link">
                            {{t('routes.membres.navigation.quiz.name')}}
                        </RouterLink>
                    </li>
                    <li class="nav-item">
                        <RouterLink :to="t('routes.membres.navigation.notes.path')" class="nav-link active">
                            {{t('routes.membres.navigation.notes.name')}}
                        </RouterLink>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</template>

<script lang="ts" setup>
import { useI18n } from "vue3-i18n";

const {t} = useI18n()
</script>

<style>
    .navbar {
        background-color: rgba(0, 0, 0, 0.75) !important;
        backdrop-filter: blur(6px);
        -webkit-backdrop-filter: blur(6px);
        border-bottom: 1px solid rgba(255,255,255,0.08);
    }

    .nav-link {
        color: #e0e0e0 !important;
        font-weight: 500;
    }

    .nav-link:hover,
        .nav-link.active {
            color: white !important;
        }
</style>
