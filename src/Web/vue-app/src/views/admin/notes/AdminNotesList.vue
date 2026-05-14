<template>
  <div class="space-y-6">
    <div class="flex flex-wrap items-center justify-between gap-3">
      <div>
<<<<<<< HEAD
        <h1 class="text-2xl font-bold text-gray-900">{{ t('pages.adminNotes.title') }}</h1>
        <p class="text-sm text-gray-500">{{ t('pages.adminNotes.subtitle') }}</p>
      </div>
      <button @click="showAddModal = true" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm">
        + {{ t('pages.adminNotes.addNote') }}
      </button>
    </div>

    <!-- Filter Section -->
    <div class="bg-white border border-gray-200 rounded-2xl p-4 shadow-sm flex flex-wrap items-end gap-4">
      <div class="flex-1 min-w-[250px] max-w-md">
        <label class="block text-xs font-medium text-gray-500 mb-1">{{ t('pages.adminNotes.filterByMember') }}</label>
        <AsyncMemberSelect 
          v-model="filterMemberId" 
          :placeholder="t('pages.adminNotes.allMembersPlaceholder')"
        />
      </div>
      <div class="flex gap-4">
        <div>
          <label class="block text-xs font-medium text-gray-500 mb-1">{{ t('pages.adminNotes.from') }}</label>
          <input type="date" v-model="filterStartDate" class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm transition text-gray-700">
        </div>
        <div>
          <label class="block text-xs font-medium text-gray-500 mb-1">{{ t('pages.adminNotes.to') }}</label>
          <input type="date" v-model="filterEndDate" class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm transition text-gray-700">
        </div>
        <div class="flex items-end">
          <button v-if="filterStartDate || filterEndDate" @click="filterStartDate = ''; filterEndDate = ''" class="mb-1 text-xs text-gray-400 hover:text-gray-600 transition underline">
            {{ t('pages.adminNotes.clearDates') }}
          </button>
=======
        <h1 class="text-2xl font-bold text-gray-900">Notes administratives</h1>
        <p class="text-sm text-gray-500">Gérez les notes internes pour les membres et les équipes.</p>
      </div>
      <button @click="openAddModal" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm flex items-center gap-2">
        <Plus class="w-4 h-4" />
        Ajouter une note
      </button>
    </div>

    <!-- Accordion Section: Member Notes -->
    <div class="border border-gray-200 rounded-2xl bg-white shadow-sm overflow-hidden">
      <button 
        @click="memberAccordionOpen = !memberAccordionOpen"
        class="w-full px-6 py-4 flex items-center justify-between hover:bg-gray-50 transition"
      >
        <div class="flex items-center gap-3">
          <div class="p-2 bg-brand-50 text-brand-600 rounded-lg">
            <User class="w-5 h-5" />
          </div>
          <div class="text-left">
            <h2 class="text-lg font-bold text-gray-900">Notes des membres</h2>
            <p class="text-xs text-gray-500">{{ memberNotes.length }} notes enregistrées</p>
          </div>
        </div>
        <ChevronDown :class="['w-5 h-5 text-gray-400 transition-transform duration-300', memberAccordionOpen ? 'rotate-180' : '']" />
      </button>

      <div v-show="memberAccordionOpen" class="border-t border-gray-100 p-6 space-y-4">
        <!-- Member Filters -->
        <div class="bg-gray-50 rounded-xl p-4 flex flex-wrap items-end gap-4">
          <div class="flex-1 min-w-[250px] max-w-sm">
            <label class="block text-xs font-medium text-gray-500 mb-1">Filtrer par membre</label>
            <AsyncMemberSelect v-model="filterMemberId" placeholder="Tous les membres..." />
          </div>
          <div class="w-40">
            <label class="block text-xs font-medium text-gray-500 mb-1">Visibilité</label>
            <select v-model="filterMemberVisibility" class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500">
              <option value="all">Toutes</option>
              <option value="private">Privées</option>
              <option value="public">Publiques</option>
            </select>
          </div>
          <div class="flex gap-2">
            <div class="w-32">
              <label class="block text-xs font-medium text-gray-500 mb-1">Du</label>
              <input type="date" v-model="filterMemberStartDate" class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500" />
            </div>
            <div class="w-32">
              <label class="block text-xs font-medium text-gray-500 mb-1">Au</label>
              <input type="date" v-model="filterMemberEndDate" class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500" />
            </div>
          </div>
          <button v-if="hasMemberFilters" @click="resetMemberFilters" class="mb-2 text-xs text-brand-600 hover:underline">Réinitialiser</button>
        </div>

        <!-- Member Notes Table -->
        <div class="overflow-x-auto border border-gray-200 rounded-xl">
          <table class="w-full text-left">
            <thead class="bg-gray-50 border-b border-gray-200">
              <tr>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase">Membre</th>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase">Auteur</th>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase">Note</th>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase">Visibilité</th>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase text-right">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-100">
              <tr v-for="note in filteredMemberNotes" :key="note.id" class="hover:bg-gray-50 transition">
                <td class="py-4 px-4 text-sm font-medium text-gray-900">{{ note.memberName }}</td>
                <td class="py-4 px-4 text-sm text-gray-600">{{ note.createdByAdminName }}</td>
                <td class="py-4 px-4 text-sm text-gray-600 max-w-xs truncate">{{ note.content }}</td>
                <td class="py-4 px-4 text-sm">
                  <span :class="['px-2 py-0.5 rounded-full text-[10px] font-bold uppercase', note.isPrivate ? 'bg-amber-50 text-amber-600' : 'bg-emerald-50 text-emerald-600']">
                    {{ note.isPrivate ? 'Privée' : 'Publique' }}
                  </span>
                </td>
                <td class="py-4 px-4 text-sm text-right">
                  <button @click="openDetails(note, 'member')" class="text-brand-600 hover:underline">Détails</button>
                </td>
              </tr>
              <tr v-if="filteredMemberNotes.length === 0">
                <td colspan="5" class="py-12 text-center text-gray-400 text-sm">Aucune note trouvée.</td>
              </tr>
            </tbody>
          </table>
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
        </div>
      </div>
    </div>

    <!-- Accordion Section: Equipe Notes -->
    <div class="border border-gray-200 rounded-2xl bg-white shadow-sm overflow-hidden">
      <button 
        @click="equipeAccordionOpen = !equipeAccordionOpen"
        class="w-full px-6 py-4 flex items-center justify-between hover:bg-gray-50 transition"
      >
        <div class="flex items-center gap-3">
          <div class="p-2 bg-sky-50 text-sky-600 rounded-lg">
            <Users class="w-5 h-5" />
          </div>
          <div class="text-left">
            <h2 class="text-lg font-bold text-gray-900">Notes des équipes</h2>
            <p class="text-xs text-gray-500">{{ equipeNotes.length }} notes enregistrées</p>
          </div>
        </div>
        <ChevronDown :class="['w-5 h-5 text-gray-400 transition-transform duration-300', equipeAccordionOpen ? 'rotate-180' : '']" />
      </button>

<<<<<<< HEAD
    <div v-else class="bg-white border border-gray-200 rounded-2xl overflow-hidden shadow-sm">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-gray-50 border-b border-gray-200">
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">{{ t('pages.adminNotes.table.member') }}</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">{{ t('pages.adminNotes.table.author') }}</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">{{ t('pages.adminNotes.table.note') }}</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">{{ t('pages.adminNotes.table.visibility') }}</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">{{ t('pages.adminNotes.table.date') }}</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider text-right">{{ t('pages.adminNotes.table.actions') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-100">
            <tr v-for="note in filteredNotes" :key="note.id" class="hover:bg-gray-50 transition">
              <td class="py-4 px-4 text-sm font-medium text-gray-900">{{ note.memberName }}</td>
              <td class="py-4 px-4 text-sm text-gray-600">{{ note.createdByAdminName }}</td>
              <td class="py-4 px-4 text-sm text-gray-600 max-w-xs truncate" :title="note.content">
                {{ note.content }}
              </td>
              <td class="py-4 px-4 text-sm">
                <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                      :class="note.isPrivate ? 'bg-amber-50 text-amber-700' : 'bg-emerald-50 text-emerald-700'">
                  {{ note.isPrivate ? t('pages.adminNotes.private') : t('pages.adminNotes.shared') }}
                </span>
              </td>
              <td class="py-4 px-4 text-sm text-gray-500">
                {{ new Date(note.created).toLocaleDateString() }}
              </td>
              <td class="py-4 px-4 text-sm text-right">
                <button @click="openDetails(note)" class="text-brand-600 hover:text-brand-800 font-medium transition mr-3">{{ t('pages.adminNotes.details') }}</button>
              </td>
            </tr>
            <tr v-if="filteredNotes.length === 0">
              <td colspan="5" class="py-12 px-4 text-center text-sm text-gray-500">
                <div class="flex flex-col items-center justify-center gap-2">
                  <svg class="w-8 h-8 text-gray-300" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" /></svg>
                  {{ t('pages.adminNotes.noNotes') }}
                </div>
              </td>
            </tr>
          </tbody>
        </table>
=======
      <div v-show="equipeAccordionOpen" class="border-t border-gray-100 p-6 space-y-4">
        <!-- Equipe Filters -->
        <div class="bg-gray-50 rounded-xl p-4 flex flex-wrap items-end gap-4">
          <div class="flex-1 min-w-[250px] max-w-sm">
            <label class="block text-xs font-medium text-gray-500 mb-1">Filtrer par équipe/sous-équipe</label>
            <AsyncEquipeSelect v-model="filterEquipeId" placeholder="Toutes les équipes..." />
          </div>
          <div class="w-40">
            <label class="block text-xs font-medium text-gray-500 mb-1">Visibilité</label>
            <select v-model="filterEquipeVisibility" class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500">
              <option value="all">Toutes</option>
              <option value="private">Privées</option>
              <option value="public">Publiques</option>
            </select>
          </div>
          <div class="flex gap-2">
            <div class="w-32">
              <label class="block text-xs font-medium text-gray-500 mb-1">Du</label>
              <input type="date" v-model="filterEquipeStartDate" class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500" />
            </div>
            <div class="w-32">
              <label class="block text-xs font-medium text-gray-500 mb-1">Au</label>
              <input type="date" v-model="filterEquipeEndDate" class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500" />
            </div>
          </div>
          <button v-if="hasEquipeFilters" @click="resetEquipeFilters" class="mb-2 text-xs text-brand-600 hover:underline">Réinitialiser</button>
        </div>

        <!-- Equipe Notes Table -->
        <div class="overflow-x-auto border border-gray-200 rounded-xl">
          <table class="w-full text-left">
            <thead class="bg-gray-50 border-b border-gray-200">
              <tr>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase">Équipe</th>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase">Auteur</th>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase">Note</th>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase">Visibilité</th>
                <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase text-right">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-100">
              <tr v-for="note in filteredEquipeNotes" :key="note.id" class="hover:bg-gray-50 transition">
                <td class="py-4 px-4 text-sm font-medium text-gray-900">{{ note.equipeName }}</td>
                <td class="py-4 px-4 text-sm text-gray-600">{{ note.createdByAdminName }}</td>
                <td class="py-4 px-4 text-sm text-gray-600 max-w-xs truncate">{{ note.content }}</td>
                <td class="py-4 px-4 text-sm">
                  <span :class="['px-2 py-0.5 rounded-full text-[10px] font-bold uppercase', note.isPrivate ? 'bg-amber-50 text-amber-600' : 'bg-emerald-50 text-emerald-600']">
                    {{ note.isPrivate ? 'Privée' : 'Publique' }}
                  </span>
                </td>
                <td class="py-4 px-4 text-sm text-right">
                  <button @click="openDetails(note, 'equipe')" class="text-brand-600 hover:underline">Détails</button>
                </td>
              </tr>
              <tr v-if="filteredEquipeNotes.length === 0">
                <td colspan="5" class="py-12 text-center text-gray-400 text-sm">Aucune note trouvée.</td>
              </tr>
            </tbody>
          </table>
        </div>
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
      </div>
    </div>

    <!-- Add Note Modal -->
    <div v-if="showAddModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-md overflow-hidden animate-fade-up">
        <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
          <h3 class="text-lg font-semibold text-gray-900">{{ t('pages.adminNotes.addNote') }}</h3>
          <button @click="showAddModal = false" class="text-gray-400 hover:text-gray-600 transition">
            <X class="w-5 h-5" />
          </button>
        </div>

        <div class="p-6 space-y-5">
<<<<<<< HEAD
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">{{ t('pages.adminNotes.concernedMember') }}</label>
            <AsyncMemberSelect 
              v-model="newNote.memberId" 
              :placeholder="t('pages.adminNotes.searchMemberPlaceholder')"
            />
=======
          <!-- Note Type Selector -->
          <div class="flex p-1 bg-gray-100 rounded-xl">
            <button 
              @click="addNoteType = 'member'"
              :class="['flex-1 py-2 text-sm font-medium rounded-lg transition', addNoteType === 'member' ? 'bg-white shadow text-brand-700' : 'text-gray-500 hover:text-gray-700']"
            >
              Membre
            </button>
            <button 
              @click="addNoteType = 'equipe'"
              :class="['flex-1 py-2 text-sm font-medium rounded-lg transition', addNoteType === 'equipe' ? 'bg-white shadow text-brand-700' : 'text-gray-500 hover:text-gray-700']"
            >
              Équipe
            </button>
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
          </div>

          <div v-if="addNoteType === 'member'">
            <label class="block text-sm font-medium text-gray-700 mb-1">Membre concerné</label>
            <AsyncMemberSelect v-model="newNote.memberId" />
          </div>
          <div v-else>
            <label class="block text-sm font-medium text-gray-700 mb-1">Équipe concernée</label>
            <AsyncEquipeSelect v-model="newNote.equipeId" />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">{{ t('pages.adminNotes.noteContent') }}</label>
            <textarea v-model="newNote.content" rows="4" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 outline-none text-sm transition resize-none" :placeholder="t('pages.adminNotes.notePlaceholder')"></textarea>
          </div>
<<<<<<< HEAD
          <div class="flex items-start gap-3 mt-2 bg-gray-50 p-3 rounded-xl border border-gray-100">
            <input type="checkbox" id="isPrivate" v-model="newNote.isPrivate" class="mt-0.5 rounded text-brand-600 focus:ring-brand-500 transition">
            <label for="isPrivate" class="text-sm text-gray-700 cursor-pointer">
              <span class="font-medium text-gray-900 block mb-0.5">{{ t('pages.adminNotes.privateNote') }}</span>
              {{ t('pages.adminNotes.privateHelp') }}
            </label>
=======

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Visibilité</label>
            <div class="flex items-center gap-6 bg-gray-50 p-3 rounded-xl border border-gray-100">
              <label class="flex items-center gap-2 cursor-pointer">
                <input type="radio" v-model="newNote.isPrivate" :value="true" class="w-4 h-4 text-brand-600 border-gray-300">
                <span class="text-sm font-medium text-gray-700">Privée</span>
              </label>
              <label class="flex items-center gap-2 cursor-pointer">
                <input type="radio" v-model="newNote.isPrivate" :value="false" class="w-4 h-4 text-brand-600 border-gray-300">
                <span class="text-sm font-medium text-gray-700">Publique</span>
              </label>
            </div>
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
          </div>
        </div>

        <div class="px-6 py-4 bg-gray-50 flex justify-end gap-3 border-t border-gray-100">
<<<<<<< HEAD
          <button @click="showAddModal = false" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 hover:bg-gray-50 rounded-lg transition shadow-sm">{{ t('global.cancel') }}</button>
          <button @click="submitNote" :disabled="submitting || !newNote.memberId || !newNote.content" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm disabled:opacity-50 flex items-center gap-2">
            <svg v-if="submitting" class="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ submitting ? t('pages.adminNotes.adding') : t('pages.adminNotes.addNote') }}
=======
          <button @click="showAddModal = false" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition">Annuler</button>
          <button 
            @click="submitNote" 
            :disabled="submitting || !newNote.content || (addNoteType === 'member' ? !newNote.memberId : !newNote.equipeId)"
            class="px-4 py-2 text-sm font-medium text-white bg-brand-600 rounded-lg hover:bg-brand-700 transition shadow-sm disabled:opacity-50 flex items-center gap-2"
          >
            <Loader2 v-if="submitting" class="w-4 h-4 animate-spin" />
            {{ submitting ? 'Envoi...' : 'Ajouter la note' }}
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
          </button>
        </div>
      </div>
    </div>

    <!-- Details/Edit Modal -->
    <div v-if="selectedNote" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-md overflow-hidden animate-fade-up">
        <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
<<<<<<< HEAD
          <h3 class="text-lg font-semibold text-gray-900">
            {{ isEditing ? t('pages.adminNotes.editNote') : t('pages.adminNotes.noteDetails') }}
          </h3>
          <button @click="closeDetails" class="text-gray-400 hover:text-gray-600 transition">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
=======
          <h3 class="text-lg font-semibold text-gray-900">{{ isEditing ? 'Modifier la note' : 'Détails de la note' }}</h3>
          <button @click="closeDetails" class="text-gray-400 hover:text-gray-600 transition"><X class="w-5 h-5" /></button>
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
        </div>

        <div class="p-6 space-y-5">
          <div v-if="!isEditing" class="space-y-4">
            <div>
<<<<<<< HEAD
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">{{ t('pages.adminNotes.concernedMember') }}</span>
              <p class="text-sm font-medium text-gray-900">{{ selectedNote.memberName }}</p>
=======
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">{{ selectedNoteType === 'member' ? 'Membre' : 'Équipe' }}</span>
              <p class="text-sm font-medium text-gray-900">{{ selectedNoteType === 'member' ? selectedNote.memberName : (selectedNote as any).equipeName }}</p>
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
            </div>
            <div>
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">{{ t('pages.adminNotes.table.author') }}</span>
              <p class="text-sm text-gray-700">{{ selectedNote.createdByAdminName }}</p>
            </div>
            <div>
<<<<<<< HEAD
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">{{ t('pages.adminNotes.table.visibility') }}</span>
              <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                    :class="selectedNote.isPrivate ? 'bg-amber-50 text-amber-700' : 'bg-emerald-50 text-emerald-700'">
                {{ selectedNote.isPrivate ? 'Privée' : 'Partagée' }}
=======
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Visibilité</span>
              <span :class="['px-2 py-0.5 rounded-full text-[10px] font-bold uppercase', selectedNote.isPrivate ? 'bg-amber-50 text-amber-600' : 'bg-emerald-50 text-emerald-600']">
                {{ selectedNote.isPrivate ? 'Privée' : 'Publique' }}
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
              </span>
            </div>
            <div>
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-2">{{ t('pages.adminNotes.content') }}</span>
              <div class="bg-gray-50 p-3 rounded-xl border border-gray-100 text-sm text-gray-800 whitespace-pre-wrap">{{ selectedNote.content }}</div>
            </div>
<<<<<<< HEAD
            <div>
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">{{ t('pages.adminNotes.createdDate') }}</span>
              <p class="text-sm text-gray-700">{{ new Date(selectedNote.created).toLocaleString() }}</p>
            </div>
=======
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
          </div>

          <div v-else class="space-y-4">
            <div>
<<<<<<< HEAD
              <span class="block text-sm font-medium text-gray-700 mb-1">{{ t('pages.adminNotes.table.member') }}</span>
              <p class="text-sm font-medium text-gray-900 px-3 py-2 bg-gray-50 rounded-lg border border-gray-200">{{ selectedNote.memberName }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ t('pages.adminNotes.noteContent') }}</label>
              <textarea v-model="editData.content" rows="4" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 outline-none text-sm transition resize-none"></textarea>
            </div>
            <div class="flex items-start gap-3 mt-2 bg-gray-50 p-3 rounded-xl border border-gray-100">
              <input type="checkbox" id="editIsPrivate" v-model="editData.isPrivate" class="mt-0.5 rounded text-brand-600 focus:ring-brand-500 transition">
              <label for="editIsPrivate" class="text-sm text-gray-700 cursor-pointer">
                <span class="font-medium text-gray-900 block mb-0.5">{{ t('pages.adminNotes.privateNote') }}</span>
                {{ t('pages.adminNotes.privateOnlyHelp') }}
              </label>
=======
              <label class="block text-sm font-medium text-gray-700 mb-1">Contenu</label>
              <textarea v-model="editData.content" rows="4" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 outline-none text-sm transition resize-none"></textarea>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Visibilité</label>
              <div class="flex items-center gap-6 bg-gray-50 p-3 rounded-xl border border-gray-100">
                <label class="flex items-center gap-2 cursor-pointer">
                  <input type="radio" v-model="editData.isPrivate" :value="true" class="w-4 h-4 text-brand-600 border-gray-300">
                  <span class="text-sm font-medium text-gray-700">Privée</span>
                </label>
                <label class="flex items-center gap-2 cursor-pointer">
                  <input type="radio" v-model="editData.isPrivate" :value="false" class="w-4 h-4 text-brand-600 border-gray-300">
                  <span class="text-sm font-medium text-gray-700">Publique</span>
                </label>
              </div>
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
            </div>
          </div>
        </div>

        <div class="px-6 py-4 bg-gray-50 flex items-center justify-between border-t border-gray-100">
<<<<<<< HEAD
          <div v-if="!isEditing">
            <button @click="confirmDelete" :disabled="submitting" class="text-sm font-medium text-red-600 hover:text-red-700 transition">{{ t('global.delete') }}</button>
          </div>
          <div v-else></div>

          <div class="flex gap-3">
            <button v-if="!isEditing" @click="isEditing = true" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 hover:bg-gray-50 rounded-lg transition shadow-sm">{{ t('global.edit') }}</button>
            <button v-if="isEditing" @click="isEditing = false" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 hover:bg-gray-50 rounded-lg transition shadow-sm">{{ t('global.cancel') }}</button>
            
            <button v-if="!isEditing" @click="closeDetails" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm">{{ t('pages.adminNotes.close') }}</button>
            <button v-if="isEditing" @click="submitEdit" :disabled="submitting || !editData.content" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm disabled:opacity-50">
              {{ submitting ? t('pages.adminNotes.saving') : t('global.save') }}
=======
          <button v-if="!isEditing" @click="confirmDelete" class="text-sm font-medium text-red-600 hover:text-red-700 transition">Supprimer</button>
          <div v-else></div>

          <div class="flex gap-3">
            <button v-if="!isEditing" @click="isEditing = true" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">Modifier</button>
            <button v-if="isEditing" @click="isEditing = false" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">Annuler</button>
            
            <button v-if="!isEditing" @click="closeDetails" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 rounded-lg hover:bg-brand-700">Fermer</button>
            <button v-if="isEditing" @click="submitEdit" :disabled="submitting || !editData.content" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 rounded-lg hover:bg-brand-700 disabled:opacity-50">
              {{ submitting ? 'Envoi...' : 'Sauvegarder' }}
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';
import type { NoteDto, EquipeNoteDto } from '@/services/NotesService';
import { useNotesService } from '@/inversify.config';
import { useNotification } from '@kyvg/vue3-notification';
import AsyncMemberSelect from '@/components/forms/AsyncMemberSelect.vue';
<<<<<<< HEAD
import { useI18n } from 'vue3-i18n';
=======
import AsyncEquipeSelect from '@/components/forms/AsyncEquipeSelect.vue';
import { 
  Plus, 
  User, 
  Users, 
  ChevronDown, 
  X, 
  Loader2 
} from 'lucide-vue-next';
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3

const route = useRoute();
const notesService = useNotesService();
const { notify } = useNotification();
const { t } = useI18n();

const loading = ref(true);
const memberNotes = ref<NoteDto[]>([]);
const equipeNotes = ref<EquipeNoteDto[]>([]);

const memberAccordionOpen = ref(true);
const equipeAccordionOpen = ref(false);

// Filters
const filterMemberId = ref("");
const filterMemberVisibility = ref("all");
const filterMemberStartDate = ref("");
const filterMemberEndDate = ref("");

const filterEquipeId = ref("");
const filterEquipeVisibility = ref("all");
const filterEquipeStartDate = ref("");
const filterEquipeEndDate = ref("");

// Add Note
const showAddModal = ref(false);
const submitting = ref(false);
const addNoteType = ref<'member' | 'equipe'>('member');
const newNote = ref({
  memberId: "",
  equipeId: "",
  content: "",
  isPrivate: true
});

// Details/Edit
const selectedNote = ref<NoteDto | EquipeNoteDto | null>(null);
const selectedNoteType = ref<'member' | 'equipe'>('member');
const isEditing = ref(false);
const editData = ref({ content: "", isPrivate: true });

// Computed Filters
const hasMemberFilters = computed(() => filterMemberId.value || filterMemberVisibility.value !== 'all' || filterMemberStartDate.value || filterMemberEndDate.value);
const hasEquipeFilters = computed(() => filterEquipeId.value || filterEquipeVisibility.value !== 'all' || filterEquipeStartDate.value || filterEquipeEndDate.value);

const filteredMemberNotes = computed(() => applyFilters(memberNotes.value, 'member'));
const filteredEquipeNotes = computed(() => applyFilters(equipeNotes.value, 'equipe'));

function applyFilters(notes: any[], type: 'member' | 'equipe') {
  let result = [...notes];
  const id = type === 'member' ? filterMemberId.value : filterEquipeId.value;
  const visibility = type === 'member' ? filterMemberVisibility.value : filterEquipeVisibility.value;
  const start = type === 'member' ? filterMemberStartDate.value : filterEquipeStartDate.value;
  const end = type === 'member' ? filterMemberEndDate.value : filterEquipeEndDate.value;

  if (id) result = result.filter(n => (type === 'member' ? n.memberId : n.equipeId) === id);
  if (visibility !== "all") result = result.filter(n => n.isPrivate === (visibility === "private"));
  if (start) {
    const d = new Date(start); d.setHours(0, 0, 0, 0);
    result = result.filter(n => new Date(n.created) >= d);
  }
  if (end) {
    const d = new Date(end); d.setHours(23, 59, 59, 999);
    result = result.filter(n => new Date(n.created) <= d);
  }
  return result;
}

function resetMemberFilters() {
  filterMemberId.value = "";
  filterMemberVisibility.value = "all";
  filterMemberStartDate.value = "";
  filterMemberEndDate.value = "";
}

function resetEquipeFilters() {
  filterEquipeId.value = "";
  filterEquipeVisibility.value = "all";
  filterEquipeStartDate.value = "";
  filterEquipeEndDate.value = "";
}

async function loadData() {
  loading.value = true;
  try {
    const [mNotes, eNotes] = await Promise.all([
      notesService.getAllNotes(),
      notesService.getAllEquipeNotes()
    ]);
    memberNotes.value = mNotes.sort((a, b) => new Date(b.created).getTime() - new Date(a.created).getTime());
    equipeNotes.value = eNotes.sort((a, b) => new Date(b.created).getTime() - new Date(a.created).getTime());

    // Deep Link from Route
    if (route.query.memberId) {
      filterMemberId.value = String(route.query.memberId);
      memberAccordionOpen.value = true;
      equipeAccordionOpen.value = false;
    } else if (route.query.equipeId) {
      filterEquipeId.value = String(route.query.equipeId);
      equipeAccordionOpen.value = true;
      memberAccordionOpen.value = false;
    }
  } catch (err) {
<<<<<<< HEAD
    notify({ type: 'error', text: t('pages.adminNotes.notify.loadError') });
=======
    notify({ type: 'error', text: 'Erreur de chargement.' });
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
  } finally {
    loading.value = false;
  }
}

function openAddModal() {
  newNote.value = { memberId: filterMemberId.value, equipeId: filterEquipeId.value, content: "", isPrivate: true };
  showAddModal.value = true;
}

async function submitNote() {
  submitting.value = true;
  try {
<<<<<<< HEAD
    const created = await notesService.createNote(newNote.value);
    if (created) {
      notify({ type: 'success', text: t('pages.adminNotes.notify.addSuccess') });
      notes.value.unshift(created);
      showAddModal.value = false;
      newNote.value.content = "";
      newNote.value.isPrivate = true;
    } else {
      notify({ type: 'error', text: t('pages.adminNotes.notify.addImpossible') });
    }
  } catch (err) {
    notify({ type: 'error', text: t('pages.adminNotes.notify.addError') });
=======
    if (addNoteType.value === 'member') {
      const res = await notesService.createNote({ memberId: newNote.value.memberId, content: newNote.value.content, isPrivate: newNote.value.isPrivate });
      if (res) memberNotes.value.unshift(res);
    } else {
      const res = await notesService.createEquipeNote({ equipeId: newNote.value.equipeId, content: newNote.value.content, isPrivate: newNote.value.isPrivate });
      if (res) equipeNotes.value.unshift(res);
    }
    notify({ type: 'success', text: 'Note ajoutée.' });
    showAddModal.value = false;
  } catch {
    notify({ type: 'error', text: 'Erreur lors de l\'ajout.' });
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
  } finally {
    submitting.value = false;
  }
}

function openDetails(note: any, type: 'member' | 'equipe') {
  selectedNote.value = note;
  selectedNoteType.value = type;
  isEditing.value = false;
  editData.value = { content: note.content, isPrivate: note.isPrivate };
}

function closeDetails() {
  selectedNote.value = null;
}

async function submitEdit() {
  if (!selectedNote.value) return;
  submitting.value = true;
  try {
    const updated = selectedNoteType.value === 'member' 
      ? await notesService.updateNote(selectedNote.value.id, editData.value)
      : await notesService.updateEquipeNote(selectedNote.value.id, editData.value);
    
    if (updated) {
<<<<<<< HEAD
      notify({ type: 'success', text: t('pages.adminNotes.notify.editSuccess') });
      // Replace note in array
      const idx = notes.value.findIndex(n => n.id === updated.id);
      if (idx !== -1) notes.value[idx] = updated;
      closeDetails();
    } else {
      notify({ type: 'error', text: t('pages.adminNotes.notify.editImpossible') });
    }
  } catch (err) {
    notify({ type: 'error', text: t('pages.adminNotes.notify.editError') });
=======
      if (selectedNoteType.value === 'member') {
        const idx = memberNotes.value.findIndex(n => n.id === updated.id);
        if (idx !== -1) memberNotes.value[idx] = updated as NoteDto;
      } else {
        const idx = equipeNotes.value.findIndex(n => n.id === updated.id);
        if (idx !== -1) equipeNotes.value[idx] = updated as EquipeNoteDto;
      }
      notify({ type: 'success', text: 'Note modifiée.' });
      closeDetails();
    }
  } catch {
    notify({ type: 'error', text: 'Erreur de modification.' });
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
  } finally {
    submitting.value = false;
  }
}

async function confirmDelete() {
<<<<<<< HEAD
  if (!selectedNote.value) return;
  if (!confirm(t('pages.adminNotes.confirmDelete'))) return;
  
=======
  if (!selectedNote.value || !confirm("Supprimer cette note ?")) return;
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
  submitting.value = true;
  try {
    const success = selectedNoteType.value === 'member'
      ? await notesService.deleteNote(selectedNote.value.id)
      : await notesService.deleteEquipeNote(selectedNote.value.id);
    
    if (success) {
<<<<<<< HEAD
      notify({ type: 'success', text: t('pages.adminNotes.notify.deleteSuccess') });
      notes.value = notes.value.filter(n => n.id !== selectedNote.value!.id);
      closeDetails();
    } else {
      notify({ type: 'error', text: t('pages.adminNotes.notify.deleteImpossible') });
    }
  } catch (err) {
    notify({ type: 'error', text: t('pages.adminNotes.notify.deleteError') });
=======
      if (selectedNoteType.value === 'member') memberNotes.value = memberNotes.value.filter(n => n.id !== selectedNote.value!.id);
      else equipeNotes.value = equipeNotes.value.filter(n => n.id !== selectedNote.value!.id);
      notify({ type: 'success', text: 'Note supprimée.' });
      closeDetails();
    }
  } catch {
    notify({ type: 'error', text: 'Erreur de suppression.' });
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3
  } finally {
    submitting.value = false;
  }
}

onMounted(loadData);
</script>

<style scoped>
.animate-fade-up {
  animation: fade-up 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
@keyframes fade-up {
  from { opacity: 0; transform: translateY(20px) scale(0.97); }
  to { opacity: 1; transform: translateY(0) scale(1); }
}
</style>
