<script setup lang="ts">
import { ref, defineEmits } from 'vue';

const emits = defineEmits(['close', 'save']);

const fieldConfig = ref<any>({
  id: '',
  type: 'text',
  label: '',
  placeholder: '',
  required: false,
  options: []
});
const optionsText = ref('');

// Save field
function saveField() {
  if (!fieldConfig.value.label.trim()) {
    alert('Label is required');
    return;
  }

  const needsOptions = ['select', 'radio'].includes(fieldConfig.value.type);
  if (needsOptions) {
    fieldConfig.value.options = optionsText.value.split('\n').filter(Boolean);
    if (fieldConfig.value.options.length === 0) {
      alert('At least one option is required for this field type');
      return;
    }
  }
  emits('save', fieldConfig.value);
}
</script>

<template>
  <div class="modal-backdrop fade show"></div>
  <div class="modal fade show d-block" tabindex="-1" @click.self="$emit('close')">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content border-0 shadow-lg rounded-4 overflow-hidden">
        <div class="modal-header bg-primary text-white border-0 p-4">
          <h5 class="modal-title fw-bold d-flex align-items-center gap-2">
            <i class="bi bi-plus-circle-fill"></i> Add New Field
          </h5>
          <button type="button" class="btn-close btn-close-white" @click="$emit('close')"></button>
        </div>
        <div class="modal-body p-4 bg-slate-50">
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary small text-uppercase tracking-wide">Field Type</label>
            <select v-model="fieldConfig.type" class="form-select form-select-lg shadow-sm border-0">
              <option value="text">Text Input</option>
              <option value="email">Email</option>
              <option value="number">Number</option>
              <option value="date">Date</option>
              <option value="tel">Phone</option>
              <option value="url">URL</option>
              <option value="textarea">Text Area</option>
              <option value="select">Dropdown</option>
              <option value="radio">Radio Group</option>
              <option value="checkbox">Checkbox</option>
            </select>
          </div>

          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary small text-uppercase tracking-wide">
              Label <span v-if="fieldConfig.type === 'checkbox'" class="text-muted fw-normal text-transform-none">(checkbox text)</span>
            </label>
            <input
              type="text"
              v-model="fieldConfig.label"
              placeholder="e.g., Full Name, Email Address"
              class="form-control form-control-lg shadow-sm border-0"
            />
          </div>

          <div v-if="!['checkbox', 'radio'].includes(fieldConfig.type)" class="mb-4">
            <label class="form-label fw-semibold text-secondary small text-uppercase tracking-wide">Placeholder</label>
            <input
              type="text"
              v-model="fieldConfig.placeholder"
              placeholder="e.g., Enter your name"
              class="form-control form-control-lg shadow-sm border-0"
            />
          </div>

          <div class="form-check form-switch mb-4">
            <input class="form-check-input" type="checkbox" role="switch" id="requiredSwitch" v-model="fieldConfig.required" />
            <label class="form-check-label fw-medium" for="requiredSwitch">Required field</label>
          </div>

          <div v-if="['select', 'radio'].includes(fieldConfig.type)" class="mb-4">
            <label class="form-label fw-semibold text-secondary small text-uppercase tracking-wide">
              Options <span class="text-muted fw-normal text-transform-none">(one per line)</span>
            </label>
            <textarea
              v-model="optionsText"
              placeholder="Option 1\nOption 2\nOption 3"
              class="form-control form-control-lg shadow-sm border-0"
              rows="4"
            ></textarea>
          </div>
        </div>
        <div class="modal-footer border-0 p-4 bg-white">
          <button type="button" class="btn btn-light btn-lg px-4 fw-medium text-secondary" @click="$emit('close')">Cancel</button>
          <button type="button" class="btn btn-primary btn-lg px-4 shadow-md" @click="saveField">
            Add Field
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-backdrop {
  background-color: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(4px);
}
.bg-slate-50 {
  background-color: #f8fafc;
}
.text-transform-none {
  text-transform: none;
}
</style>