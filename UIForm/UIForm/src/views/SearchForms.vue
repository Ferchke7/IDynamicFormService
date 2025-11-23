<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import SubmittingForm from '../components/SubmittingForm.vue';

interface FormSubmission {
  jsonDocument: any;
  attachmentList: Array<{
    guid: string;
    path: string;
  }>;
}

const forms = ref<FormSubmission[]>([]);
const searchQuery = ref('');
let debounceTimer: ReturnType<typeof setTimeout> | null = null;

async function fetchForms() {
  try {
    const response = await fetch('/api/submissions');
    if (!response.ok) {
      throw new Error(`Failed to fetch forms: ${response.statusText}`);
    }
    forms.value = await response.json();
  } catch (error) {
    console.error('Error fetching forms:', error);
  }
}

async function searchForms() {
  if (!searchQuery.value.trim()) {
    await fetchForms();
    return;
  }

  try {
    const response = await fetch(`/api/submissions/search?keyword=${encodeURIComponent(searchQuery.value)}`);
    if (!response.ok) {
      throw new Error(`Search failed: ${response.statusText}`);
    }
    forms.value = await response.json();
  } catch (error) {
    console.error('Error searching forms:', error);
  }
}

function debouncedSearch() {
  if (debounceTimer) {
    clearTimeout(debounceTimer);
  }
  debounceTimer = setTimeout(() => {
    searchForms();
  }, 300);
}

watch(searchQuery, () => {
  debouncedSearch();
});

onMounted(() => {
  fetchForms();
});
</script>

<template>
  <div class="row justify-content-center">
    <div class="col-12 col-xl-10">
      <div class="premium-card p-5">
        <div class="d-flex justify-content-between align-items-center mb-5">
          <div>
            <h2 class="display-6 fw-bold text-primary mb-1">All Submissions</h2>
            <p class="text-muted mb-0">View and manage submitted forms.</p>
          </div>
          <div class="input-group w-auto shadow-sm">
            <span class="input-group-text bg-white border-end-0 ps-3">
              <i class="bi bi-search text-muted"></i>
            </span>
            <input
              type="text"
              v-model="searchQuery"
              class="form-control border-start-0 ps-0"
              placeholder="Search submissions..."
              style="min-width: 250px;"
            />
          </div>
        </div>

        <SubmittingForm :forms="forms" />
      </div>
    </div>
  </div>
</template>
