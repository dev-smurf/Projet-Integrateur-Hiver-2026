<template>
  <div class="space-y-6">
    <div class="flex flex-wrap items-center justify-between gap-3">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Notes internes</h1>
        <p class="text-sm text-gray-500">Gérez les notes confidentielles ou partagées pour tous les membres.</p>
      </div>
      <button @click="showAddModal = true" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm">
        + Ajouter une note
      </button>
    </div>

    <!-- Filter Section -->
    <div class="bg-white border border-gray-200 rounded-2xl p-4 shadow-sm flex flex-wrap items-end gap-4">
      <div class="flex-1 min-w-[250px] max-w-md">
        <label class="block text-xs font-medium text-gray-500 mb-1">Filtrer par membre</label>
        <AsyncMemberSelect 
          v-model="filterMemberId" 
          placeholder="Tous les membres (rechercher un membre...)"
        />
      </div>
      <div class="flex gap-4">
        <div>
          <label class="block text-xs font-medium text-gray-500 mb-1">Du</label>
          <input type="date" v-model="filterStartDate" class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm transition text-gray-700">
        </div>
        <div>
          <label class="block text-xs font-medium text-gray-500 mb-1">Au</label>
          <input type="date" v-model="filterEndDate" class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm transition text-gray-700">
        </div>
        <div class="flex items-end">
          <button v-if="filterStartDate || filterEndDate" @click="filterStartDate = ''; filterEndDate = ''" class="mb-1 text-xs text-gray-400 hover:text-gray-600 transition underline">
            Effacer dates
          </button>
        </div>
      </div>
    </div>

    <div v-if="loading" class="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm min-h-[300px] flex items-center justify-center">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-brand-600"></div>
    </div>

    <div v-else class="bg-white border border-gray-200 rounded-2xl overflow-hidden shadow-sm">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-gray-50 border-b border-gray-200">
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Membre</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Auteur</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Note</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Visibilité</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Date</th>
              <th class="py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider text-right">Actions</th>
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
                  {{ note.isPrivate ? 'Privée' : 'Partagée' }}
                </span>
              </td>
              <td class="py-4 px-4 text-sm text-gray-500">
                {{ new Date(note.created).toLocaleDateString() }}
              </td>
              <td class="py-4 px-4 text-sm text-right">
                <button @click="openDetails(note)" class="text-brand-600 hover:text-brand-800 font-medium transition mr-3">Détails</button>
              </td>
            </tr>
            <tr v-if="filteredNotes.length === 0">
              <td colspan="5" class="py-12 px-4 text-center text-sm text-gray-500">
                <div class="flex flex-col items-center justify-center gap-2">
                  <svg class="w-8 h-8 text-gray-300" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" /></svg>
                  Aucune note trouvée.
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add Note Modal -->
    <div v-if="showAddModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-md overflow-hidden animate-fade-up">
        <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
          <h3 class="text-lg font-semibold text-gray-900">Ajouter une note</h3>
          <button @click="showAddModal = false" class="text-gray-400 hover:text-gray-600 transition">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <div class="p-6 space-y-5">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Membre concerné</label>
            <AsyncMemberSelect 
              v-model="newNote.memberId" 
              placeholder="Rechercher le membre..."
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Contenu de la note</label>
            <textarea v-model="newNote.content" rows="4" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 outline-none text-sm transition resize-none" placeholder="Saisir la note ici..."></textarea>
          </div>
          <div class="flex items-start gap-3 mt-2 bg-gray-50 p-3 rounded-xl border border-gray-100">
            <input type="checkbox" id="isPrivate" v-model="newNote.isPrivate" class="mt-0.5 rounded text-brand-600 focus:ring-brand-500 transition">
            <label for="isPrivate" class="text-sm text-gray-700 cursor-pointer">
              <span class="font-medium text-gray-900 block mb-0.5">Note privée</span>
              Visible uniquement par les administrateurs. Si décoché, le membre pourra voir cette note.
            </label>
          </div>
        </div>
        <div class="px-6 py-4 bg-gray-50 flex justify-end gap-3 border-t border-gray-100">
          <button @click="showAddModal = false" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 hover:bg-gray-50 rounded-lg transition shadow-sm">Annuler</button>
          <button @click="submitNote" :disabled="submitting || !newNote.memberId || !newNote.content" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm disabled:opacity-50 flex items-center gap-2">
            <svg v-if="submitting" class="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ submitting ? 'Ajout en cours...' : 'Ajouter la note' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Details/Edit Modal -->
    <div v-if="selectedNote" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-md overflow-hidden animate-fade-up">
        <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
          <h3 class="text-lg font-semibold text-gray-900">
            {{ isEditing ? 'Modifier la note' : 'Détails de la note' }}
          </h3>
          <button @click="closeDetails" class="text-gray-400 hover:text-gray-600 transition">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        
        <div class="p-6 space-y-5">
          <!-- Read Mode -->
          <div v-if="!isEditing" class="space-y-4">
            <div>
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Membre concerné</span>
              <p class="text-sm font-medium text-gray-900">{{ selectedNote.memberName }}</p>
            </div>
            <div>
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Auteur</span>
              <p class="text-sm text-gray-700">{{ selectedNote.createdByAdminName }}</p>
            </div>
            <div>
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Visibilité</span>
              <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                    :class="selectedNote.isPrivate ? 'bg-amber-50 text-amber-700' : 'bg-emerald-50 text-emerald-700'">
                {{ selectedNote.isPrivate ? 'Privée' : 'Partagée' }}
              </span>
            </div>
            <div>
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-2">Contenu</span>
              <div class="bg-gray-50 p-3 rounded-xl border border-gray-100 text-sm text-gray-800 whitespace-pre-wrap">{{ selectedNote.content }}</div>
            </div>
            <div>
              <span class="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Date de création</span>
              <p class="text-sm text-gray-700">{{ new Date(selectedNote.created).toLocaleString() }}</p>
            </div>
          </div>

          <!-- Edit Mode -->
          <div v-else class="space-y-4">
            <div>
              <span class="block text-sm font-medium text-gray-700 mb-1">Membre</span>
              <p class="text-sm font-medium text-gray-900 px-3 py-2 bg-gray-50 rounded-lg border border-gray-200">{{ selectedNote.memberName }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Contenu de la note</label>
              <textarea v-model="editData.content" rows="4" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 outline-none text-sm transition resize-none"></textarea>
            </div>
            <div class="flex items-start gap-3 mt-2 bg-gray-50 p-3 rounded-xl border border-gray-100">
              <input type="checkbox" id="editIsPrivate" v-model="editData.isPrivate" class="mt-0.5 rounded text-brand-600 focus:ring-brand-500 transition">
              <label for="editIsPrivate" class="text-sm text-gray-700 cursor-pointer">
                <span class="font-medium text-gray-900 block mb-0.5">Note privée</span>
                Visible uniquement par les administrateurs.
              </label>
            </div>
          </div>
        </div>

        <div class="px-6 py-4 bg-gray-50 flex items-center justify-between border-t border-gray-100">
          <div v-if="!isEditing">
            <button @click="confirmDelete" :disabled="submitting" class="text-sm font-medium text-red-600 hover:text-red-700 transition">Supprimer</button>
          </div>
          <div v-else></div>

          <div class="flex gap-3">
            <button v-if="!isEditing" @click="isEditing = true" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 hover:bg-gray-50 rounded-lg transition shadow-sm">Modifier</button>
            <button v-if="isEditing" @click="isEditing = false" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 hover:bg-gray-50 rounded-lg transition shadow-sm">Annuler</button>
            
            <button v-if="!isEditing" @click="closeDetails" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm">Fermer</button>
            <button v-if="isEditing" @click="submitEdit" :disabled="submitting || !editData.content" class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm disabled:opacity-50">
              {{ submitting ? 'Sauvegarde...' : 'Sauvegarder' }}
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
import type { NoteDto, CreateNoteRequest } from '@/services/NotesService';
import { useNotesService } from '@/inversify.config';
import { useNotification } from '@kyvg/vue3-notification';
import AsyncMemberSelect from '@/components/forms/AsyncMemberSelect.vue';

const route = useRoute();
const notesService = useNotesService();
const { notify } = useNotification();

const loading = ref(true);
const notes = ref<NoteDto[]>([]);

const filterMemberId = ref("");
const filterStartDate = ref("");
const filterEndDate = ref("");

const showAddModal = ref(false);
const submitting = ref(false);
const newNote = ref<CreateNoteRequest>({
  memberId: "",
  content: "",
  isPrivate: true
});

const selectedNote = ref<NoteDto | null>(null);
const isEditing = ref(false);
const editData = ref({ content: "", isPrivate: true });

const filteredNotes = computed(() => {
  let result = notes.value;

  if (filterMemberId.value) {
    result = result.filter(n => n.memberId === filterMemberId.value);
  }

  if (filterStartDate.value) {
    const start = new Date(filterStartDate.value);
    start.setHours(0, 0, 0, 0);
    result = result.filter(n => new Date(n.created) >= start);
  }

  if (filterEndDate.value) {
    const end = new Date(filterEndDate.value);
    end.setHours(23, 59, 59, 999);
    result = result.filter(n => new Date(n.created) <= end);
  }

  return result;
});

async function loadData() {
  loading.value = true;
  try {
    const notesData = await notesService.getAllNotes();
    // Sort notes descending by created date
    notes.value = notesData.sort((a, b) => new Date(b.created).getTime() - new Date(a.created).getTime());
    
    // Check URL query parameter to filter
    if (route.query.memberId) {
      filterMemberId.value = String(route.query.memberId);
      newNote.value.memberId = String(route.query.memberId);
    }
  } catch (err) {
    notify({ type: 'error', text: 'Erreur lors du chargement des données.' });
  } finally {
    loading.value = false;
  }
}

// Ensure the new note memberId is set to the filtered member if they add a note while filtering
watch(filterMemberId, (newVal) => {
  if (newVal) {
    newNote.value.memberId = newVal;
  }
});

async function submitNote() {
  if (!newNote.value.memberId || !newNote.value.content) return;
  submitting.value = true;
  try {
    const created = await notesService.createNote(newNote.value);
    if (created) {
      notify({ type: 'success', text: 'Note ajoutée avec succès.' });
      notes.value.unshift(created);
      showAddModal.value = false;
      newNote.value.content = "";
      newNote.value.isPrivate = true;
    } else {
      notify({ type: 'error', text: 'Impossible d\'ajouter la note.' });
    }
  } catch (err) {
    notify({ type: 'error', text: 'Erreur lors de l\'ajout de la note.' });
  } finally {
    submitting.value = false;
  }
}

function openDetails(note: NoteDto) {
  selectedNote.value = note;
  isEditing.value = false;
  editData.value = { content: note.content, isPrivate: note.isPrivate };
}

function closeDetails() {
  selectedNote.value = null;
  isEditing.value = false;
}

async function submitEdit() {
  if (!selectedNote.value || !editData.value.content) return;
  submitting.value = true;
  try {
    const updated = await notesService.updateNote(selectedNote.value.id, {
      content: editData.value.content,
      isPrivate: editData.value.isPrivate
    });
    if (updated) {
      notify({ type: 'success', text: 'Note modifiée avec succès.' });
      // Replace note in array
      const idx = notes.value.findIndex(n => n.id === updated.id);
      if (idx !== -1) notes.value[idx] = updated;
      closeDetails();
    } else {
      notify({ type: 'error', text: 'Impossible de modifier la note.' });
    }
  } catch (err) {
    notify({ type: 'error', text: 'Erreur lors de la modification.' });
  } finally {
    submitting.value = false;
  }
}

async function confirmDelete() {
  if (!selectedNote.value) return;
  if (!confirm("Voulez-vous vraiment supprimer cette note ? Cette action est irréversible.")) return;
  
  submitting.value = true;
  try {
    const success = await notesService.deleteNote(selectedNote.value.id);
    if (success) {
      notify({ type: 'success', text: 'Note supprimée.' });
      notes.value = notes.value.filter(n => n.id !== selectedNote.value!.id);
      closeDetails();
    } else {
      notify({ type: 'error', text: 'Impossible de supprimer la note.' });
    }
  } catch (err) {
    notify({ type: 'error', text: 'Erreur lors de la suppression.' });
  } finally {
    submitting.value = false;
  }
}

onMounted(() => {
  loadData();
});
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
