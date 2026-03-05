<<<<<<< HEAD
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
=======
<template>
    <header class="hero-header">
        <nav class="navbar navbar-expand-lg navbar-dark fixed-top">
            <div class="container">
                <a class="navbar-brand fw-bold fs-5" href="#">
                    <span style="color: #28a745;"> X </span>
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

    const { t } = useI18n()
</script>

<style lang="scss" scoped>
@use "@/sass/tools" as *;

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
>>>>>>> 194fe738189c29a0db951be3c1c84e6dfcd46df1
</style>
