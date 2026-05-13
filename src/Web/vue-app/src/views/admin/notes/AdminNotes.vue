<template>
  <div class="space-y-6">
    <div class="flex flex-wrap items-center justify-between gap-3">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Notes</h1>
        <p class="mt-1 text-sm text-gray-500">Sélectionnez un membre et ajustez sa note.</p>
      </div>
    </div>

    <div class="grid gap-6 lg:grid-cols-[360px_minmax(0,1fr)]">
      <section class="overflow-hidden rounded-xl border border-gray-200 bg-white">
        <div class="border-b border-gray-200 p-4">
          <div class="relative">
            <Search class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
            <input
              v-model="searchValue"
              type="text"
              :placeholder="t('global.search')"
              class="w-full rounded-lg border border-gray-300 py-2 pl-10 pr-4 text-sm outline-none transition focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
            />
          </div>
        </div>

        <div v-if="loadingMembers" class="space-y-2 p-3">
          <div v-for="n in 6" :key="n" class="h-16 animate-pulse rounded-lg bg-gray-100" />
        </div>
        <div v-else-if="!filteredMembers.length" class="p-8 text-center text-sm text-gray-500">
          Aucun membre trouvé.
        </div>
        <div v-else class="max-h-[640px] overflow-y-auto p-2">
          <button
            v-for="member in filteredMembers"
            :key="member.id"
            type="button"
            class="flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-left transition hover:bg-gray-50"
            :class="selectedMemberId === member.id ? 'bg-brand-50 ring-1 ring-brand-100' : ''"
            @click="selectMember(member.id)"
          >
            <span class="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-gray-100 text-xs font-bold text-gray-700">
              {{ initialsFor(member) }}
            </span>
            <span class="min-w-0">
              <span class="block truncate text-sm font-semibold text-gray-900">{{ fullNameFor(member) }}</span>
              <span class="block truncate text-xs text-gray-500">{{ member.email || "N/A" }}</span>
            </span>
          </button>
        </div>
      </section>

      <section class="rounded-xl border border-gray-200 bg-white p-5">
        <div v-if="loadingMember" class="space-y-4">
          <div class="h-8 w-48 animate-pulse rounded bg-gray-100" />
          <div class="h-36 animate-pulse rounded-lg bg-gray-100" />
          <div class="h-11 w-64 animate-pulse rounded bg-gray-100" />
        </div>

        <div v-else-if="!selectedMember" class="flex min-h-80 items-center justify-center text-sm text-gray-500">
          Choisissez un membre pour ajouter une note.
        </div>

        <div v-else class="space-y-5">
          <div class="flex flex-wrap items-start justify-between gap-3">
            <div>
              <h2 class="text-lg font-semibold text-gray-900">{{ selectedMemberName }}</h2>
              <p class="text-sm text-gray-500">{{ selectedMember.email || "N/A" }}</p>
            </div>
            <span
              class="inline-flex items-center rounded-full px-2.5 py-1 text-xs font-medium"
              :class="notesVisibleToMember ? 'bg-emerald-50 text-emerald-700' : 'bg-gray-100 text-gray-700'"
            >
              {{ notesVisibleToMember ? "Public" : "Privée" }}
            </span>
          </div>

          <label class="block">
            <span class="mb-1 block text-sm font-medium text-gray-700">Note</span>
            <textarea
              v-model="notesText"
              rows="8"
              placeholder="Ajouter une note sur ce membre..."
              class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none transition focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
            />
          </label>

          <div class="grid gap-4 md:grid-cols-[minmax(0,220px)_auto] md:items-end">
            <label class="block text-sm text-gray-700">
              <span class="mb-1 block font-medium">Date édition de la note</span>
              <input
                v-model="notesDate"
                type="date"
                required
                class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none transition focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
              />
            </label>

            <div>
              <span class="mb-1 block text-sm font-medium text-gray-700">Visibilité</span>
              <div class="inline-flex rounded-lg border border-gray-300 bg-white p-1">
                <button
                  type="button"
                  class="rounded-md px-3 py-1.5 text-xs font-semibold transition"
                  :class="!notesVisibleToMember ? 'bg-gray-900 text-white' : 'text-gray-600 hover:bg-gray-50'"
                  @click="notesVisibleToMember = false"
                >
                  Privée
                </button>
                <button
                  type="button"
                  class="rounded-md px-3 py-1.5 text-xs font-semibold transition"
                  :class="notesVisibleToMember ? 'bg-brand-600 text-white' : 'text-gray-600 hover:bg-gray-50'"
                  @click="notesVisibleToMember = true"
                >
                  Public
                </button>
              </div>
            </div>
          </div>

          <div class="flex justify-end">
            <button
              type="button"
              class="inline-flex items-center gap-2 rounded-lg bg-gray-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-black disabled:cursor-not-allowed disabled:opacity-50"
              :disabled="savingNote"
              @click="saveNote"
            >
              <Save class="h-4 w-4" />
              {{ savingNote ? "Enregistrement..." : "Enregistrer" }}
            </button>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from "vue";
import { useNotification } from "@kyvg/vue3-notification";
import { Save, Search } from "lucide-vue-next";
import { useI18n } from "vue3-i18n";
import { useMemberService } from "@/inversify.config";
import type { Member } from "@/types/entities";
import { todayDateInputValue, toDateInputValue } from "@/utils/dateInput";

const { t } = useI18n();
const { notify } = useNotification();
const memberService = useMemberService();

const allMembers = ref<Member[]>([]);
const selectedMember = ref<Member | null>(null);
const selectedMemberId = ref<string | undefined>();
const loadingMembers = ref(true);
const loadingMember = ref(false);
const savingNote = ref(false);
const searchValue = ref("");
const notesText = ref("");
const notesDate = ref(todayDateInputValue());
const notesVisibleToMember = ref(false);

const filteredMembers = computed(() => {
  const q = searchValue.value.toLowerCase().trim();
  if (!q) return allMembers.value;
  return allMembers.value.filter(member =>
    (member.firstName || "").toLowerCase().includes(q) ||
    (member.lastName || "").toLowerCase().includes(q) ||
    (member.email || "").toLowerCase().includes(q)
  );
});

const selectedMemberName = computed(() => selectedMember.value ? fullNameFor(selectedMember.value) : "Membre");

async function fetchMembers() {
  loadingMembers.value = true;
  try {
    const result = await memberService.search(1, 9999, "");
    allMembers.value = result.items || [];
    const firstMemberId = allMembers.value[0]?.id;
    if (firstMemberId) {
      await selectMember(firstMemberId);
    }
  } finally {
    loadingMembers.value = false;
  }
}

async function selectMember(memberId?: string) {
  if (!memberId || loadingMember.value) return;
  selectedMemberId.value = memberId;
  loadingMember.value = true;
  try {
    const member = await memberService.getMember(memberId);
    selectedMember.value = member;
    notesText.value = member.adminNotes ?? "";
    notesVisibleToMember.value = member.adminNotesVisibleToMember ?? false;
    notesDate.value = toDateInputValue(member.adminNotesEditedAt) || todayDateInputValue();
  } catch {
    notify({ type: "error", text: "Impossible de charger les notes du membre." });
  } finally {
    loadingMember.value = false;
  }
}

async function saveNote() {
  if (!selectedMember.value?.id || savingNote.value) return;
  if (!notesDate.value) {
    notesDate.value = todayDateInputValue();
  }

  savingNote.value = true;
  try {
    const response = await memberService.updateMember({
      ...selectedMember.value,
      adminNotes: notesText.value.trim() || undefined,
      adminNotesVisibleToMember: notesVisibleToMember.value,
      adminNotesEditedAt: `${notesDate.value}T00:00:00`
    });

    if (!response.succeeded) {
      notify({ type: "error", text: "Impossible d'enregistrer la note." });
      return;
    }

    const updatedMember = await memberService.getMember(selectedMember.value.id);
    selectedMember.value = updatedMember;
    notesText.value = updatedMember.adminNotes ?? "";
    notesVisibleToMember.value = updatedMember.adminNotesVisibleToMember ?? false;
    notesDate.value = toDateInputValue(updatedMember.adminNotesEditedAt) || todayDateInputValue();
    allMembers.value = allMembers.value.map(member => member.id === updatedMember.id ? { ...member, ...updatedMember } : member);
    notify({ type: "success", text: "Note enregistrée." });
  } finally {
    savingNote.value = false;
  }
}

function fullNameFor(member: Member) {
  return `${member.firstName || ""} ${member.lastName || ""}`.trim() || "Membre";
}

function initialsFor(member: Member) {
  const first = member.firstName?.charAt(0) || "";
  const last = member.lastName?.charAt(0) || "";
  return `${first}${last}`.toUpperCase() || "M";
}

onMounted(fetchMembers);
</script>
