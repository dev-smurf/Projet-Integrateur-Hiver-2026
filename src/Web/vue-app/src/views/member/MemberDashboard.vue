<template>
    <div class="space-y-8">

        <!-- Bannière d'accueil -->
        <section class="relative overflow-hidden rounded-2xl text-white p-6 sm:p-8 shadow-lg"
                 style="background: linear-gradient(to right, #4c6367, #5a7578, #4c6367);">
            <div class="relative z-10 max-w-3xl">
                <p class="text-sm" style="color: rgba(152,255,152,0.8);">{{ $t("pages.memberDashboard.welcomeLabel") }}</p>
                <h1 class="text-3xl sm:text-4xl font-semibold mt-1" style="color: white;">
                    {{ displayName }}
                </h1>
                <p class="mt-2 text-sm sm:text-base" style="color: rgba(255,255,255,0.9);">
                    {{ $t("pages.memberDashboard.tagline") }}
                </p>
                 <div class="mt-5 flex flex-wrap gap-3">
                     <router-link :to="{ name: 'member.modules.index' }"
                                  class="inline-flex items-center gap-2 rounded-lg px-4 py-2 text-sm font-medium transition"
                                  style="background-color: rgba(152,255,152,0.2); color: #98ff98;"
                                  @mouseover="e => e.currentTarget.style.backgroundColor='rgba(152,255,152,0.3)'"
                                  @mouseleave="e => e.currentTarget.style.backgroundColor='rgba(152,255,152,0.2)'">
                         <BookOpen class="h-4 w-4" />
                         {{ $t("pages.memberDashboard.viewModules") }}
                     </router-link>
                 </div>
             </div>
             <div class="absolute -right-10 -top-10 h-40 w-40 rounded-full blur-3xl"
                  style="background-color: rgba(152,255,152,0.15);" />
             <div class="absolute right-16 bottom-0 h-24 w-24 rounded-full blur-2xl"
                  style="background-color: rgba(144,114,136,0.2);" />
         </section>

        <!-- Login notifications banner -->
        <section v-if="hasNewAssignments"
                 class="relative rounded-2xl border border-emerald-200 bg-gradient-to-br from-emerald-50 via-white to-emerald-50/40 p-5 shadow-sm">
            <div class="flex items-start gap-4">
                <div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-emerald-100 text-emerald-600">
                    <Sparkles class="h-5 w-5" />
                </div>
                <div class="flex-1 min-w-0">
                    <h3 class="text-sm font-semibold text-emerald-900">
                        {{ $t("pages.loginNotifications.title") }}
                    </h3>
                    <p class="mt-0.5 text-xs text-emerald-700/80">
                        {{ $t("pages.loginNotifications.subtitle") }}
                    </p>

                    <ul class="mt-3 grid gap-2 sm:grid-cols-2">
                        <li v-for="q in loginNotifications.quizzes" :key="`q-${q.assignmentId}`"
                            class="flex items-start gap-2 rounded-lg border border-emerald-100 bg-white/70 px-3 py-2">
                            <ClipboardList class="h-4 w-4 mt-0.5 text-emerald-600 shrink-0" />
                            <div class="min-w-0">
                                <p class="text-xs font-medium uppercase tracking-wide text-emerald-600">
                                    {{ $t("pages.loginNotifications.newQuiz") }}
                                </p>
                                <p class="text-sm font-medium text-gray-900 truncate">{{ q.titre }}</p>
                                <p v-if="q.followUpLabel" class="text-xs text-gray-500 truncate">{{ q.followUpLabel }}</p>
                            </div>
                        </li>
                        <li v-for="m in loginNotifications.modules" :key="`m-${m.moduleId}`"
                            class="flex items-start gap-2 rounded-lg border border-emerald-100 bg-white/70 px-3 py-2">
                            <BookOpen class="h-4 w-4 mt-0.5 text-emerald-600 shrink-0" />
                            <div class="min-w-0">
                                <p class="text-xs font-medium uppercase tracking-wide text-emerald-600">
                                    {{ $t("pages.loginNotifications.newModule") }}
                                </p>
                                <p class="text-sm font-medium text-gray-900 truncate">{{ m.name }}</p>
                            </div>
                        </li>
                    </ul>
                </div>
                <button type="button"
                        class="text-emerald-600 hover:text-emerald-800 transition shrink-0 rounded-md p-1 hover:bg-emerald-100"
                        :aria-label="$t('pages.loginNotifications.dismiss')"
                        @click="dismissLoginNotifications">
                    <X class="h-4 w-4" />
                </button>
            </div>
        </section>

        <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 gap-4">
            <div class="bg-white border border-gray-200 rounded-xl p-4">
                <div class="flex items-center justify-between">
                    <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.stats.modules") }}</p>
                    <Layers class="h-4 w-4" style="color: #4c6367;" />
                </div>
                <p class="text-2xl font-semibold text-gray-900 mt-3">{{ totalModules }}</p>
                <p class="text-xs text-gray-400 mt-1">{{ $t("pages.memberDashboard.stats.modulesHint") }}</p>
            </div>
            <div class="bg-white border border-gray-200 rounded-xl p-4">
                <div class="flex items-center justify-between">
                    <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.stats.completed") }}</p>
                    <CheckCircle class="h-4 w-4 text-emerald-500" />
                </div>
                <p class="text-2xl font-semibold text-gray-900 mt-3">{{ completedModules }}</p>
                <p class="text-xs text-gray-400 mt-1">{{ $t("pages.memberDashboard.stats.completedHint") }}</p>
            </div>
        </section>

        <!-- Modules + Sidebar -->
        <section class="grid grid-cols-1 lg:grid-cols-3 gap-6">

            <div class="lg:col-span-2 space-y-4">
                <div class="flex items-center justify-between">
                    <h2 class="text-lg font-semibold text-gray-900">{{ $t("pages.memberDashboard.modulesTitle") }}</h2>
                    <span class="text-sm text-gray-500">{{ totalModules }} {{ $t("pages.memberDashboard.modulesCount") }}</span>
                </div>

                <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div v-for="n in 4" :key="n" class="bg-white rounded-xl border border-gray-200 p-4 animate-pulse">
                        <div class="h-24 bg-gray-100 rounded-lg mb-4" />
                        <div class="h-4 bg-gray-200 rounded w-3/4 mb-2" />
                        <div class="h-3 bg-gray-200 rounded w-1/2" />
                        <div class="h-2 bg-gray-200 rounded mt-4" />
                    </div>
                </div>

                <div v-else-if="!pendingModuleCards.length" class="text-center py-10 text-gray-500">
                    {{ $t("pages.memberDashboard.emptyModules") }}
                 </div>
 
                <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <router-link v-for="mod in pendingModuleCards"
                         :key="mod.id"
                         :to="'/mes-modules/' + mod.id"
                         class="bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-md transition cursor-pointer">
                        <div class="h-28 bg-gray-50 flex items-center justify-center overflow-hidden">
                            <img v-if="mod.imageUrl"
                                 :src="mod.imageUrl"
                                 :alt="mod.name"
                                 class="h-full w-full object-cover" />
                            <BookOpen v-else class="h-8 w-8" style="color: #4c6367;" />
                        </div>
                        <div class="p-4">
                            <div class="flex items-start justify-between gap-3">
                                <div>
                                    <h3 class="font-semibold text-gray-900 line-clamp-1">{{ mod.name }}</h3>
                                    <p class="text-sm text-gray-500 line-clamp-2 mt-1">{{ mod.subject }}</p>
                                </div>
                                <span class="text-xs font-medium px-2 py-1 rounded-full"
                                      :style="mod.isCompleted
                                      ? 'background-color: rgba(152,255,152,0.15); color: #2d8f4e;'
                                      : mod.progressPercent > 0
                                      ? 'background-color: rgba(144,114,136,0.15); color: #907288;'
                                      : 'background-color: #f3f4f6; color: #6b7280;'"
                                    >
                                    {{ mod.statusLabel }}
                                </span>
                            </div>
                            <div class="mt-4">
                                <div class="flex items-center justify-between text-xs text-gray-500 mb-1">
                                    <span>{{ $t("pages.memberDashboard.progressLabel") }}</span>
                                    <span>{{ mod.progressPercent }}%</span>
                                </div>
                                <div class="h-2 rounded-full bg-gray-100">
                                    <div class="h-full rounded-full transition-all"
                                         style="background-color: #98ff98;"
                                         :style="{ width: mod.progressPercent + '%', backgroundColor: '#98ff98' }" />
                                </div>
                            </div>
                            <div class="mt-4 flex items-center justify-between">
                                <span class="text-xs text-gray-400">{{ $t("pages.memberDashboard.lastUpdate") }}</span>
                                <span class="text-sm font-medium transition"
                                      style="color: #4c6367;">
                                    {{ $t("pages.memberDashboard.continue") }}
                                </span>
                            </div>
                        </div>
                    </router-link>
                </div>
            </div>

            <div class="space-y-4">

                <div class="bg-white border border-gray-200 rounded-xl p-5">
                    <div class="flex items-center justify-between mb-3">
                        <h3 class="font-semibold text-gray-900">{{ $t("pages.memberDashboard.quizzesTitle") }}</h3>
                        <ClipboardList class="h-4 w-4" style="color: #4c6367;" />
                    </div>
                    <p class="text-sm text-gray-500 mb-4">{{ $t("pages.memberDashboard.quizzesHint") }}</p>

                    <div v-if="quizzesLoading" class="space-y-3">
                        <div class="h-4 bg-gray-200 rounded w-3/4 animate-pulse" />
                        <div class="h-3 bg-gray-200 rounded w-1/2 animate-pulse" />
                        <div class="h-9 bg-gray-200 rounded animate-pulse" />
                    </div>

                    <div v-else-if="!pendingQuizzes.length" class="text-sm text-gray-500">
                        {{ $t("pages.memberDashboard.quizzesEmpty") }}
                    </div>

                    <div v-else class="space-y-3">
                        <div v-for="quiz in pendingQuizzes.slice(0, 3)" :key="quiz.id" class="border border-gray-200 rounded-lg p-3">
                            <div class="flex items-start justify-between gap-3">
                                <div class="min-w-0">
                                    <p class="font-semibold text-gray-900 line-clamp-1">{{ quiz.titre }}</p>
                                    <p v-if="quiz.dueDate" class="text-xs text-gray-500 mt-1">
                                        {{ $t("quiz.dueDate") }}: {{ formatDate(quiz.dueDate) }}
                                    </p>
                                </div>
                                <span class="text-xs font-medium px-2 py-1 rounded-full"
                                      style="background-color: rgba(144,114,136,0.15); color: #907288;">
                                    {{ $t("quiz.pending") }}
                                </span>
                            </div>
                            <button
                                class="mt-3 w-full rounded-lg px-3 py-2 text-sm font-medium transition"
                                style="background-color: rgba(152,255,152,0.2); color: #2d8f4e;"
                                @click="startQuiz(quiz.quizId)"
                                @mouseover="e => e.currentTarget.style.backgroundColor='rgba(152,255,152,0.3)'"
                                @mouseleave="e => e.currentTarget.style.backgroundColor='rgba(152,255,152,0.2)'"
                            >
                                {{ $t("quiz.startQuiz") }}
                            </button>
                        </div>

                        <router-link :to="{ name: 'quiz.list' }"
                                     class="inline-flex items-center gap-2 text-sm font-medium transition"
                                     style="color: #4c6367;"
                                     @mouseover="e => e.currentTarget.style.color='#98ff98'"
                                     @mouseleave="e => e.currentTarget.style.color='#4c6367'">
                            {{ $t("pages.memberDashboard.viewAllQuizzes") }}

                            <ArrowRight class="h-4 w-4" />
                        </router-link>
                    </div>
                </div>

                <div class="bg-white border border-gray-200 rounded-xl p-5">
                    <div class="flex items-center justify-between mb-3">
                        <h3 class="font-semibold text-gray-900">Notes de l'administration</h3>
                        <FileText class="h-4 w-4" style="color: #4c6367;" />
                    </div>
                    <div v-if="notesLoading" class="space-y-2">
                        <div v-for="n in 2" :key="n" class="h-16 bg-gray-100 rounded animate-pulse" />
                    </div>
                    <div v-else-if="!myNotes.length" class="text-sm text-gray-500 italic">
                        Aucune note publique disponible.
                    </div>
                    <div v-else class="space-y-3 pr-1">
                        <div v-for="note in myNotes.slice(0, 3)" :key="note.id" class="border border-gray-200 rounded-lg p-3 bg-gray-50">
                            <div class="flex items-center justify-between gap-2 mb-2">
                                <span class="text-xs font-semibold text-gray-700">{{ note.createdByAdminName }}</span>
                                <span class="text-[10px] text-gray-400">{{ formatDate(note.created) }}</span>
                            </div>
                            <p class="text-sm text-gray-800 whitespace-pre-wrap">{{ note.content }}</p>
                        </div>
                        
                        <button v-if="myNotes.length > 3" 
                                @click="showNotesModal = true"
                                class="w-full mt-2 text-sm font-medium transition flex items-center justify-center gap-1 p-2 rounded-lg"
                                style="color: #4c6367; background-color: rgba(76, 99, 103, 0.05);"
                                @mouseover="e => e.currentTarget.style.backgroundColor='rgba(76, 99, 103, 0.1)'"
                                @mouseleave="e => e.currentTarget.style.backgroundColor='rgba(76, 99, 103, 0.05)'">
                            Voir plus ({{ myNotes.length }}) <ArrowRight class="h-4 w-4" />
                        </button>
                    </div>
                </div>
            </div>
        </section>

        <!-- Notes Modal -->
        <div v-if="showNotesModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4" @click.self="showNotesModal = false">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-2xl max-h-[90vh] flex flex-col overflow-hidden">
                <div class="p-5 border-b border-gray-100 flex items-center justify-between">
                    <h2 class="text-lg font-semibold text-gray-900">Toutes les notes ({{ filteredNotes.length }})</h2>
                    <button @click="showNotesModal = false" class="text-gray-400 hover:text-gray-600 transition">
                        <span class="text-2xl leading-none">&times;</span>
                    </button>
                </div>
                
                <div class="p-4 sm:p-5 border-b border-gray-100 flex flex-col sm:flex-row gap-4 bg-gray-50">
                    <div class="flex-1">
                        <label class="block text-xs font-medium text-gray-500 mb-1">Rechercher</label>
                        <input type="text" v-model="notesSearchQuery" placeholder="Mots-clés dans le contenu..." class="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-[#4c6367] focus:outline-none" />
                    </div>
                    <div class="flex-1">
                        <label class="block text-xs font-medium text-gray-500 mb-1">Filtrer par date</label>
                        <input type="date" v-model="notesDateFilter" class="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-[#4c6367] focus:outline-none" />
                    </div>
                </div>
                
                <div class="p-5 overflow-y-auto flex-1 space-y-4" style="background-color: #fcfcfc;">
                    <div v-if="filteredNotes.length === 0" class="text-center text-sm text-gray-500 py-10 italic">
                        Aucune note ne correspond aux critères de recherche.
                    </div>
                    <div v-for="note in filteredNotes" :key="note.id" class="border border-gray-200 rounded-xl p-4 bg-white shadow-sm">
                        <div class="flex items-center justify-between gap-2 mb-3 border-b border-gray-50 pb-2">
                            <div class="flex items-center gap-2">
                                <div class="flex h-8 w-8 items-center justify-center rounded-full bg-gray-100 text-gray-600 text-xs font-bold">
                                    {{ note.createdByAdminName.substring(0, 2).toUpperCase() }}
                                </div>
                                <span class="text-sm font-semibold text-gray-800">{{ note.createdByAdminName }}</span>
                            </div>
                            <span class="text-xs font-medium px-2 py-1 rounded-md" style="background-color: rgba(76, 99, 103, 0.1); color: #4c6367;">
                                {{ formatDate(note.created) }}
                            </span>
                        </div>
                        <p class="text-sm text-gray-700 whitespace-pre-wrap">{{ note.content }}</p>
                    </div>
                </div>
                
                <div class="p-4 border-t border-gray-100 flex justify-end bg-gray-50">
                    <button @click="showNotesModal = false" class="px-5 py-2 bg-gray-200 text-gray-800 rounded-lg text-sm font-medium hover:bg-gray-300 transition">
                        Fermer
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

 <script lang="ts" setup>
 import {computed, onMounted, onUnmounted, onActivated, ref} from "vue";
 import {useI18n} from "vue3-i18n";
 import {useRouter} from "vue-router";
 import {ArrowRight, BookOpen, CheckCircle, ClipboardList, Layers, FileText, Sparkles, X} from "lucide-vue-next";
 import {useMemberService, useQuizService, useNotesService} from "@/inversify.config";
 import type {LoginNotifications} from "@/injection/interfaces";
 import {usePersonStore} from "@/stores/personStore";
 import {useUserStore} from "@/stores/userStore";
 import {Role} from "@/types/enums";
 import {notifySuccess} from "@/notify";
 import type {MemberModuleDto} from "@/types/entities";
 import type {AssignedQuiz} from "@/services/quizService";
 import type {NoteDto} from "@/services/NotesService";

const backendUrl = (import.meta.env.VITE_API_BASE_URL || "").replace(/\/api$/, "");

 const {getLocale, t} = useI18n();
 const router = useRouter();
 const memberService = useMemberService();
 const quizService = useQuizService();
 const notesService = useNotesService();
 const personStore = usePersonStore();
 const userStore = useUserStore();

  const loading = ref(true);
  const modules = ref<MemberModuleDto[]>([]);
  const quizzesLoading = ref(true);
  const assignedQuizzes = ref<AssignedQuiz[]>([]);
  const notesLoading = ref(true);
  const myNotes = ref<NoteDto[]>([]);
  const loginNotifications = ref<LoginNotifications>({quizzes: [], modules: []});
  const hasNewAssignments = computed(() =>
    loginNotifications.value.quizzes.length + loginNotifications.value.modules.length > 0
  );

  async function fetchLoginNotifications() {
    if (!userStore.hasRole(Role.Member)) return;
    try {
      loginNotifications.value = await memberService.getLoginNotifications();
    } catch {
      // Best-effort — banner stays hidden if the API is unreachable.
    }
  }

  async function dismissLoginNotifications() {
    const previous = loginNotifications.value;
    loginNotifications.value = {quizzes: [], modules: []};
    try {
      await memberService.dismissLoginNotifications();
    } catch {
      // Restore the banner if the dismiss call failed so the athlete can retry.
      loginNotifications.value = previous;
    }
  }
  const showNotesModal = ref(false);
  const notesSearchQuery = ref("");
  const notesDateFilter = ref("");

 const displayName = computed(() => {
   return personStore.person.fullName || userStore.user.username || t("pages.memberDashboard.defaultName");
 });

 function imageUrl(path?: string): string | undefined {
   if (!path) return undefined;
   if (path.startsWith("http")) return path;
   return backendUrl + path;
 }

 const moduleCards = computed(() => {
   return modules.value.map((mod) => {
     const isFrench = getLocale() === "fr";
     const name =
       mod.name ||
       (isFrench
         ? (mod.nameFr || mod.nameEn || t("pages.memberDashboard.unnamedModule"))
         : (mod.nameEn || mod.nameFr || t("pages.memberDashboard.unnamedModule")));
     const subject =
       mod.subject ||
       (isFrench
         ? (mod.sujetFr || mod.sujetEn || t("pages.memberDashboard.noSubject"))
         : (mod.sujetEn || mod.sujetFr || t("pages.memberDashboard.noSubject")));
     const progressPercent = mod.progressPercent ?? 0;
     const isCompleted = mod.isCompleted || progressPercent >= 100;
     const statusLabel = isCompleted
       ? t("pages.memberDashboard.status.completed")
       : progressPercent > 0
         ? t("pages.memberDashboard.status.inProgress")
         : t("pages.memberDashboard.status.notStarted");

     return {
       id: mod.moduleId,
       name,
       subject,
       imageUrl: imageUrl(mod.cardImageUrl),
       progressPercent,
       isCompleted,
       statusLabel
     };
   });
 });

 const filteredNotes = computed(() => {
     return myNotes.value.filter(note => {
         const matchContent = note.content.toLowerCase().includes(notesSearchQuery.value.toLowerCase());
         const matchDate = !notesDateFilter.value || note.created.startsWith(notesDateFilter.value);
         return matchContent && matchDate;
     });
 });

  const totalModules = computed(() => modules.value.length);
  const completedModules = computed(() => modules.value.filter((mod) => mod.isCompleted || (mod.progressPercent ?? 0) >= 100).length);
  const pendingModuleCards = computed(() => moduleCards.value.filter(mod => !mod.isCompleted));

  const pendingQuizzes = computed(() => assignedQuizzes.value.filter(q => !q.isCompleted));

  const formatDate = (date: Date | string): string => {
    const d = typeof date === "string" ? new Date(date) : date;
    return d.toLocaleDateString();
  };

  const startQuiz = (quizId: string) => {
    router.push({ name: "quiz.take", params: { quizId } });
  };

  async function fetchAll() {
    loading.value = true;
    quizzesLoading.value = true;
    notesLoading.value = true;
    
    try {
      // Refresh profile if needed
      if (!personStore.person.firstName) {
        const profile = await memberService.getAuthenticated();
        if (profile) personStore.setPerson(profile);
      }

      const [modData, quizData, noteData] = await Promise.all([
        memberService.getMyModules(),
        quizService.getAssignedQuizzes(),
        notesService.getMyNotes()
      ]);

      void fetchLoginNotifications();

      modules.value = modData;
      assignedQuizzes.value = quizData;
      
      noteData.sort((a, b) => new Date(b.created).getTime() - new Date(a.created).getTime());
      myNotes.value = noteData;

    } catch (err) {
      console.error("Error fetching dashboard data", err);
    } finally {
      loading.value = false;
      quizzesLoading.value = false;
      notesLoading.value = false;
    }
  }

  let pollingInterval: ReturnType<typeof setInterval> | null = null;

  onMounted(async () => {
    await fetchAll();
    
    pollingInterval = setInterval(async () => {
      if (!notesLoading.value) {
        const oldNotesCount = myNotes.value.length;
        const oldLatestNoteId = oldNotesCount > 0 ? myNotes.value[0].id : null;
        
        try {
          const newNotes = await notesService.getMyNotes();
          newNotes.sort((a, b) => new Date(b.created).getTime() - new Date(a.created).getTime());
          
          if (newNotes.length > 0) {
            const currentLatestNote = newNotes[0];
            const isNewFirstNote = oldNotesCount === 0;
            const isDifferentTopNote = oldNotesCount > 0 && currentLatestNote.id !== oldLatestNoteId;
            
            if (isNewFirstNote || isDifferentTopNote) {
              notifySuccess("Une nouvelle note administrative a été publiée !");
            }
          }
          myNotes.value = newNotes;
        } catch {
          // Silently ignore polling errors
        }
      }
    }, 10000); // 10 secondes
  });

  onActivated(fetchAll);
  
  onUnmounted(() => {
    if (pollingInterval) clearInterval(pollingInterval);
  });
 </script>
