<template>
  <div class="relative" ref="root">
    <button
      type="button"
      @click="toggleDropdown"
      class="flex min-h-11 w-full items-center justify-between gap-3 rounded-lg border bg-white px-3 py-2 text-left outline-none transition focus:ring-2 focus:ring-brand-500 focus:border-brand-500"
      :class="hasError ? 'border-red-400' : 'border-gray-300'"
    >
      <div class="flex flex-1 flex-wrap items-center gap-2">
        <template v-if="selectedOptions.length">
          <span
            v-for="option in selectedOptions"
            :key="option.value"
            class="inline-flex items-center gap-1 rounded-full bg-brand-50 px-2.5 py-1 text-xs font-medium text-brand-700"
          >
            {{ option.label }}
            <button
              type="button"
              class="text-brand-500 hover:text-brand-700"
              @click.stop="removeOption(option.value)"
            >
              <X class="h-3 w-3" />
            </button>
          </span>
        </template>
        <span v-else class="text-sm text-gray-400">{{ placeholder }}</span>
      </div>
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
          v-for="option in filteredOptions"
          :key="option.value"
          type="button"
          @click="toggleOption(option.value)"
          class="flex w-full items-center justify-between rounded-lg px-3 py-2 text-sm transition hover:bg-gray-50"
        >
          <span class="text-gray-700">{{ option.label }}</span>
          <Check v-if="isSelected(option.value)" class="h-4 w-4 text-brand-600" />
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
import { Check, ChevronDown, X } from "lucide-vue-next";

type SelectOption = {
  value: string;
  label: string;
};

const props = withDefaults(
  defineProps<{
    modelValue: string[];
    options: SelectOption[];
    placeholder?: string;
    searchPlaceholder?: string;
    emptyText?: string;
    hasError?: boolean;
  }>(),
  {
    placeholder: "Sélectionner une ou plusieurs équipes",
    searchPlaceholder: "Rechercher une équipe",
    emptyText: "Aucune équipe trouvée",
    hasError: false,
  },
);

const emit = defineEmits<{
  "update:modelValue": [value: string[]];
}>();

const isOpen = ref(false);
const search = ref("");
const root = ref<HTMLElement | null>(null);

const filteredOptions = computed(() => {
  const query = search.value.trim().toLowerCase();
  if (!query) {
    return props.options;
  }

  return props.options.filter((option) =>
    option.label.toLowerCase().includes(query),
  );
});

const selectedOptions = computed(() =>
  props.options.filter((option) => props.modelValue.includes(option.value)),
);

function emitValue(value: string[]) {
  emit("update:modelValue", value);
}

function toggleDropdown() {
  isOpen.value = !isOpen.value;
}

function isSelected(value: string) {
  return props.modelValue.includes(value);
}

function toggleOption(value: string) {
  if (isSelected(value)) {
    emitValue(props.modelValue.filter((item) => item !== value));
    return;
  }

  emitValue([...props.modelValue, value]);
}

function removeOption(value: string) {
  emitValue(props.modelValue.filter((item) => item !== value));
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
