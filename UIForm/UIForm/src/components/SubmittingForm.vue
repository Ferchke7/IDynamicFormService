

<template>
  <div>
    <transition-group name="slide-up" tag="div" class="row g-4">
      <div v-if="!forms || forms.length === 0" key="empty-state" class="col-12">
        <div class="text-center p-5 border-2 border-dashed rounded-3 bg-light">
          <div class="mb-3 text-muted">
            <i class="bi bi-inbox display-4"></i>
          </div>
          <h4 class="text-muted">No submissions found</h4>
          <p class="text-muted mb-0">Submitted forms will appear here.</p>
        </div>
      </div>

      <div v-else v-for="(form, index) in forms" :key="index" class="col-12">
        <SubmissionCard :form="form" :index="index" />
      </div>
    </transition-group>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import SubmissionCard from './SubmissionCard.vue';

defineProps({
  forms: Array,
});
</script>

<style scoped>
.hover-shadow:hover {
  box-shadow: var(--shadow-md) !important;
  transform: translateY(-2px);
}
.transition-all {
  transition: all 0.3s ease;
}
.border-dashed {
  border-style: dashed !important;
}
.bg-slate-50 {
  background-color: #f8fafc;
}
</style>