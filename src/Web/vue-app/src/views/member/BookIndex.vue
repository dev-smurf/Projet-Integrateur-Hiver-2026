<template>
  <div>
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900">{{ $t('routes.books.name') }}</h1>
      <router-link
        :to="{ name: 'books.children.add' }"
        class="flex items-center gap-2 bg-brand-600 hover:bg-brand-700 text-white font-medium py-2 px-4 rounded-lg transition text-sm"
      >
        <Plus class="w-4 h-4" />
        {{ $t('global.add') }}
      </router-link>
    </div>

    <!-- Skeleton loading -->
    <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      <div v-for="n in 6" :key="n" class="bg-white rounded-xl border border-gray-200 overflow-hidden animate-pulse">
        <div class="aspect-[2/1] bg-gray-200" />
        <div class="p-4 space-y-3">
          <div class="h-5 bg-gray-200 rounded w-3/4" />
          <div class="h-4 bg-gray-200 rounded w-1/2" />
          <div class="flex items-center justify-between mt-3">
            <div class="h-4 bg-gray-200 rounded w-16" />
            <div class="flex gap-1">
              <div class="h-6 w-6 bg-gray-200 rounded" />
              <div class="h-6 w-6 bg-gray-200 rounded" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else-if="!books.length" class="text-center py-12 text-gray-500">
      {{ $t('global.table.noData') }}
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      <div
        v-for="book in books"
        :key="book.id"
        class="bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-sm transition group"
      >
        <div v-if="book.savedCardImage" class="aspect-[2/1] overflow-hidden bg-gray-100">
          <img :src="book.savedCardImage" :alt="getBookName(book)" class="w-full h-full object-cover" />
        </div>
        <div v-else class="aspect-[2/1] bg-gray-100 flex items-center justify-center">
          <BookOpenIcon class="w-10 h-10 text-gray-300" />
        </div>
        <div class="p-4">
          <h3 class="font-semibold text-gray-900 mb-1">{{ getBookName(book) }}</h3>
          <p class="text-sm text-gray-500">{{ book.author }}</p>
          <div class="flex items-center justify-between mt-3">
            <span class="text-sm font-medium text-brand-600">
              {{ book.price ? `$${book.price.toFixed(2)}` : $t('global.free') }}
            </span>
            <div class="flex items-center gap-1">
              <router-link
                :to="{ name: 'books.children.edit', params: { id: book.id } }"
                class="p-1.5 text-gray-400 hover:text-brand-600 transition"
              >
                <Pencil class="w-4 h-4" />
              </router-link>
              <button
                @click="confirmDelete(book)"
                class="p-1.5 text-gray-400 hover:text-brand-600 transition"
              >
                <Trash2 class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete confirmation modal -->
    <div v-if="bookToDelete" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div class="bg-white rounded-xl p-6 w-full max-w-sm shadow-lg">
        <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ $t('global.delete') }}</h3>
        <p class="text-sm text-gray-600 mb-6">{{ $t('validation.book.delete.confirmation') }}</p>
        <div class="flex justify-end gap-3">
          <button
            @click="bookToDelete = null"
            class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
          >
            {{ $t('global.cancel') }}
          </button>
          <button
            @click="deleteBook"
            class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition"
          >
            {{ $t('global.delete') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, onMounted} from "vue";
import {useI18n} from "vue3-i18n";
import {useNotification} from "@kyvg/vue3-notification";
import {Plus, Pencil, Trash2, BookOpen as BookOpenIcon} from "lucide-vue-next";
import {useBookService} from "@/inversify.config";
import type {Book} from "@/types/entities";

const {locale, t} = useI18n();
const {notify} = useNotification();
const bookService = useBookService();

const books = ref<Book[]>([]);
const loading = ref(true);
const bookToDelete = ref<Book | null>(null);

function getBookName(book: Book): string {
  return locale === "fr" ? (book.nameFr || book.nameEn || "") : (book.nameEn || book.nameFr || "");
}

async function fetchBooks() {
  loading.value = true;
  books.value = await bookService.getAllBooks();
  loading.value = false;
}

function confirmDelete(book: Book) {
  bookToDelete.value = book;
}

async function deleteBook() {
  if (!bookToDelete.value?.id) return;
  const response = await bookService.deleteBook(bookToDelete.value.id);
  if (response.succeeded) {
    notify({type: "success", text: t("validation.book.delete.success")});
    bookToDelete.value = null;
    await fetchBooks();
  } else {
    notify({type: "error", text: t("validation.book.delete.errorOccured")});
  }
}

onMounted(fetchBooks);
</script>
