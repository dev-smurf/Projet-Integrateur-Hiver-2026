<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900">{{ $t('pages.notifications.title') }}</h1>
      <p class="mt-1 text-sm text-gray-500">
        {{ $t('pages.notifications.subtitle') }}
      </p>
    </div>

    <div
      v-if="loading"
      class="space-y-3"
    >
      <div
        v-for="n in 2"
        :key="n"
        class="h-28 rounded-2xl border border-gray-200 bg-white animate-pulse"
      />
    </div>

    <div
      v-else-if="notifications.length === 0"
      class="rounded-2xl border border-dashed border-gray-300 bg-white p-8 text-center"
    >
      <h2 class="text-lg font-semibold text-gray-900">{{ $t('pages.notifications.empty') }}</h2>
      <p class="mt-2 text-sm text-gray-500">
        {{ $t('pages.notifications.emptyHint') }}
      </p>
    </div>

    <div
      v-else
      class="space-y-4"
    >
      <article
        v-for="notification in notifications"
        :key="notification.id"
        class="rounded-2xl border border-brand-200 bg-white p-5 shadow-sm"
      >
        <div class="flex items-start justify-between gap-4">
          <div>
            <p class="text-xs font-semibold uppercase tracking-[0.18em] text-brand-700">
              {{ $t('pages.notifications.sender') }}
            </p>
            <h2 class="mt-2 text-lg font-semibold text-gray-900">
              {{ notification.title }}
            </h2>
          </div>
          <div class="rounded-full bg-brand-50 px-3 py-1 text-xs font-medium text-brand-700">
            {{ $t('pages.notifications.badge') }}
          </div>
        </div>
        <p class="mt-4 whitespace-pre-line text-sm leading-6 text-gray-700">
          {{ notification.message }}
        </p>
      </article>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue3-i18n";
import { useMemberService } from "@/inversify.config";
import { usePersonStore } from "@/stores/personStore";
import { useUserStore } from "@/stores/userStore";
import { markMemberAdminNoteAsRead, MEMBER_ADMIN_NOTE_READ_EVENT } from "@/utils/memberAdminNotes";

const { t } = useI18n();
const memberService = useMemberService();
const personStore = usePersonStore();
const userStore = useUserStore();
const loading = ref(true);

const notifications = computed(() => {
  const message = personStore.person.visibleAdminNotes?.trim();
  if (!message) return [];

  return [
    {
      id: "admin-note",
      title: t("pages.notifications.defaultTitle"),
      message
    }
  ];
});

function markCurrentNoteAsRead() {
  const memberIdentifier = userStore.user.email || userStore.username || "";
  markMemberAdminNoteAsRead(memberIdentifier, personStore.person.visibleAdminNotes);
  window.dispatchEvent(new Event(MEMBER_ADMIN_NOTE_READ_EVENT));
}

onMounted(async () => {
  try {
    const member = await memberService.getAuthenticated();
    if (member) {
      personStore.setPerson(member);
    }
  } finally {
    markCurrentNoteAsRead();
    loading.value = false;
  }
});
</script>
