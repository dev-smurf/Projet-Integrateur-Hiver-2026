<template>
  <div ref="root" class="relative">
    <button
      type="button"
      class="flex min-h-11 w-full items-center justify-between gap-3 rounded-lg border bg-white px-3 py-2 text-left outline-none transition focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
      :class="hasError ? 'border-red-400' : 'border-gray-300'"
      @click="toggleDropdown"
    >
      <span
        class="min-w-0 flex-1 truncate text-sm"
        :class="selectedOption ? 'text-gray-900' : 'text-gray-400'"
      >
        {{ selectedOption?.label ?? placeholder }}
      </span>
      <ChevronDown class="h-4 w-4 shrink-0 text-gray-400" :class="{ 'rotate-180': isOpen }" />
    </button>

    <div
      v-if="isOpen"
      class="absolute z-20 mt-2 w-full rounded-xl border border-gray-200 bg-white p-3 shadow-lg"
    >
      <input
        v-model="search"
        type="text"
        :placeholder="searchPlaceholder"
        class="mb-3 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none transition focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
      />

      <div class="max-h-56 overflow-y-auto">
        <button
          v-if="clearable"
          type="button"
          class="flex w-full items-center justify-between rounded-lg px-3 py-2 text-sm transition hover:bg-gray-50"
          @click="selectOption(undefined)"
        >
          <span class="text-gray-700">{{ clearLabel }}</span>
          <Check v-if="modelValue == null || modelValue === ''" class="h-4 w-4 text-brand-600" />
        </button>

        <button
          v-for="option in filteredOptions"
          :key="option.value"
          type="button"
          class="flex w-full items-center justify-between rounded-lg px-3 py-2 text-sm transition hover:bg-gray-50"
          @click="selectOption(option.value)"
        >
          <span class="text-gray-700">{{ option.label }}</span>
          <Check v-if="option.value === modelValue" class="h-4 w-4 text-brand-600" />
        </button>

        <p v-if="!filteredOptions.length" class="px-3 py-2 text-sm text-gray-400">
          {{ emptyText }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, ref, watch } from "vue";
import { Check, ChevronDown } from "lucide-vue-next";

type SelectOption = {
  value: string;
  label: string;
};

const props = withDefaults(
  defineProps<{
    modelValue?: string;
    options: SelectOption[];
    placeholder?: string;
    searchPlaceholder?: string;
    emptyText?: string;
    clearLabel?: string;
    clearable?: boolean;
    hasError?: boolean;
  }>(),
  {
    placeholder: "Selectionner une equipe",
    searchPlaceholder: "Rechercher une equipe",
    emptyText: "Aucune equipe trouvee",
    clearLabel: "Aucune (equipe principale)",
    clearable: true,
    hasError: false,
  },
);

const emit = defineEmits<{
  "update:modelValue": [value: string | undefined];
}>();

const isOpen = ref(false);
const search = ref("");
const root = ref<HTMLElement | null>(null);

const filteredOptions = computed(() => {
  const query = search.value.trim().toLowerCase();
  if (!query) {
    return props.options;
  }

  return props.options.filter((option) => option.label.toLowerCase().includes(query));
});

const selectedOption = computed(() =>
  props.options.find((option) => option.value === props.modelValue),
);

function toggleDropdown() {
  isOpen.value = !isOpen.value;
}

function selectOption(value?: string) {
  emit("update:modelValue", value || undefined);
  isOpen.value = false;
}

function handleClickOutside(event: MouseEvent) {
  if (!root.value) {
    return;
  }

  if (!root.value.contains(event.target as Node)) {
    isOpen.value = false;
  }
}

watch(isOpen, (open) => {
  if (!open) {
    search.value = "";
  }
});

document.addEventListener("click", handleClickOutside);

onBeforeUnmount(() => {
  document.removeEventListener("click", handleClickOutside);
});
</script>
