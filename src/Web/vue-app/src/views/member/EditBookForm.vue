<template>
  <div class="max-w-3xl">
    <h1 class="text-2xl font-bold text-gray-900 mb-6">{{ $t('routes.books.children.edit.name') }}</h1>

    <div v-if="loadingBook" class="flex justify-center py-12">
      <Loader2 class="w-6 h-6 animate-spin text-gray-400" />
    </div>

    <template v-else>
      <div v-if="apiErrors.length" class="mb-4 p-3 bg-brand-50 border border-brand-200 rounded-lg">
        <p v-for="error in apiErrors" :key="error" class="text-sm text-brand-600">{{ error }}</p>
      </div>

      <form @submit.prevent="handleSubmit" class="bg-white rounded-xl border border-gray-200 p-6 space-y-6">
        <fieldset>
          <legend class="text-sm font-semibold text-gray-900 mb-3">{{ $t('book.title') }}</legend>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.titleFr') }} *</label>
              <input v-model="form.nameFr" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              <p v-if="fieldErrors.nameFr" class="text-sm text-brand-500 mt-1">{{ fieldErrors.nameFr }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.titleEn') }} *</label>
              <input v-model="form.nameEn" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              <p v-if="fieldErrors.nameEn" class="text-sm text-brand-500 mt-1">{{ fieldErrors.nameEn }}</p>
            </div>
          </div>
        </fieldset>

        <fieldset>
          <legend class="text-sm font-semibold text-gray-900 mb-3">{{ $t('book.summary') }}</legend>
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.summaryFr') }}</label>
              <QuillEditor v-model:content="form.descriptionFr" contentType="html" theme="snow" class="bg-white" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.summaryEn') }}</label>
              <QuillEditor v-model:content="form.descriptionEn" contentType="html" theme="snow" class="bg-white" />
            </div>
          </div>
        </fieldset>

        <fieldset>
          <legend class="text-sm font-semibold text-gray-900 mb-3">{{ $t('book.info') }}</legend>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.isbn') }} *</label>
              <input v-model="form.isbn" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              <p v-if="fieldErrors.isbn" class="text-sm text-brand-500 mt-1">{{ fieldErrors.isbn }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.author') }} *</label>
              <input v-model="form.author" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              <p v-if="fieldErrors.author" class="text-sm text-brand-500 mt-1">{{ fieldErrors.author }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.editor') }} *</label>
              <input v-model="form.editor" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              <p v-if="fieldErrors.editor" class="text-sm text-brand-500 mt-1">{{ fieldErrors.editor }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.yearOfPublication') }} *</label>
              <input v-model.number="form.yearOfPublication" type="number" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              <p v-if="fieldErrors.yearOfPublication" class="text-sm text-brand-500 mt-1">{{ fieldErrors.yearOfPublication }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('book.numberOfPages') }} *</label>
              <input v-model.number="form.numberOfPages" type="number" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              <p v-if="fieldErrors.numberOfPages" class="text-sm text-brand-500 mt-1">{{ fieldErrors.numberOfPages }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('products.form.price') }} *</label>
              <input v-model.number="form.price" type="number" step="0.01" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              <p v-if="fieldErrors.price" class="text-sm text-brand-500 mt-1">{{ fieldErrors.price }}</p>
            </div>
          </div>
        </fieldset>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('products.form.image-card') }}</label>
          <div v-if="currentImage" class="mb-2">
            <img :src="currentImage" alt="" class="h-24 rounded-lg object-cover" />
          </div>
          <input
            type="file"
            accept="image/*"
            @change="handleFileChange"
            class="block w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:text-sm file:font-medium file:bg-brand-50 file:text-brand-700 hover:file:bg-brand-100 transition"
          />
        </div>

        <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
          <router-link
            :to="{ name: 'books.index' }"
            class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
          >
            {{ $t('global.cancel') }}
          </router-link>
          <button
            type="submit"
            :disabled="submitting"
            class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Loader2 v-if="submitting" class="w-4 h-4 animate-spin" />
            {{ $t('global.save') }}
          </button>
        </div>
      </form>
    </template>
  </div>
</template>

<script lang="ts" setup>
import {ref, reactive, onMounted} from "vue";
import {useRouter, useRoute} from "vue-router";
import {useI18n} from "vue3-i18n";
import {useNotification} from "@kyvg/vue3-notification";
import {Loader2} from "lucide-vue-next";
import {QuillEditor} from "@vueup/vue-quill";
import "@vueup/vue-quill/dist/vue-quill.snow.css";
import {useBookService} from "@/inversify.config";
import type {IEditBookRequest} from "@/types/requests/editBookRequest";
import {validate} from "@/validation";
import {required, min} from "@/validation/rules";

const router = useRouter();
const route = useRoute();
const {t} = useI18n();
const {notify} = useNotification();
const bookService = useBookService();

const form = reactive<IEditBookRequest>({});
const loadingBook = ref(true);
const submitting = ref(false);
const apiErrors = ref<string[]>([]);
const fieldErrors = reactive<Record<string, string | undefined>>({});
const currentImage = ref<string | undefined>();

onMounted(async () => {
  const book = await bookService.getBook(route.params.id as string);
  Object.assign(form, {
    id: book.id,
    nameFr: book.nameFr,
    nameEn: book.nameEn,
    descriptionFr: book.descriptionFr,
    descriptionEn: book.descriptionEn,
    isbn: book.isbn,
    author: book.author,
    editor: book.editor,
    yearOfPublication: book.yearOfPublication,
    numberOfPages: book.numberOfPages,
    price: book.price,
  });
  currentImage.value = book.savedCardImage;
  loadingBook.value = false;
});

function handleFileChange(e: Event) {
  const target = e.target as HTMLInputElement;
  if (target.files?.length) form.cardImage = target.files[0];
}

function validateForm(): boolean {
  let valid = true;
  Object.keys(fieldErrors).forEach(k => fieldErrors[k] = undefined);

  const checks: [string, ReturnType<typeof validate>][] = [
    ["nameFr", validate(form.nameFr || "", [required])],
    ["nameEn", validate(form.nameEn || "", [required])],
    ["isbn", validate(form.isbn || "", [required])],
    ["author", validate(form.author || "", [required])],
    ["editor", validate(form.editor || "", [required])],
    ["yearOfPublication", validate(String(form.yearOfPublication || ""), [required, min(1)])],
    ["numberOfPages", validate(String(form.numberOfPages || ""), [required, min(1)])],
    ["price", validate(String(form.price || ""), [required, min(0)])],
  ];

  for (const [field, result] of checks) {
    if (!result.valid) {
      fieldErrors[field] = result.message;
      valid = false;
    }
  }
  return valid;
}

async function handleSubmit() {
  if (!validateForm()) {
    notify({type: "error", text: t("global.formErrorNotification")});
    return;
  }

  submitting.value = true;
  apiErrors.value = [];

  const formData = new FormData();
  formData.append("id", form.id || "");
  if (form.nameFr) formData.append("nameFr", form.nameFr);
  if (form.nameEn) formData.append("nameEn", form.nameEn);
  if (form.descriptionFr) formData.append("descriptionFr", form.descriptionFr);
  if (form.descriptionEn) formData.append("descriptionEn", form.descriptionEn);
  if (form.isbn) formData.append("isbn", form.isbn);
  if (form.author) formData.append("author", form.author);
  if (form.editor) formData.append("editor", form.editor);
  if (form.yearOfPublication) formData.append("yearOfPublication", String(form.yearOfPublication));
  if (form.numberOfPages) formData.append("numberOfPages", String(form.numberOfPages));
  if (form.price !== undefined) formData.append("price", String(form.price));
  if (form.cardImage) formData.append("cardImage", form.cardImage);

  const response = await bookService.editBook(formData as any);
  if (response.succeeded) {
    notify({type: "success", text: t("validation.book.edit.success")});
    await router.push({name: "books.index"});
  } else {
    apiErrors.value = response.getErrorMessages("validation.book.edit");
  }
  submitting.value = false;
}
</script>
