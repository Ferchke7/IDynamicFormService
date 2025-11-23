<script setup lang="ts">
import { defineProps, defineEmits } from 'vue';

const props = defineProps({
  field: {
    type: Object,
    required: true,
  },
  modelValue: {
    type: [String, Number, Boolean, Array],
    default: '',
  },
  error: {
    type: String,
    default: '',
  },
});

const emit = defineEmits(['update:modelValue', 'remove']);

function updateValue(event: Event) {
  const target = event.target as HTMLInputElement;
  emit('update:modelValue', target.value);
}
</script>

<template>
  <div class="p-4 border rounded-3 bg-white shadow-sm position-relative hover-shadow transition-all">
    <button 
      type="button" 
      @click="$emit('remove', field.id)" 
      class="btn-close position-absolute top-0 end-0 m-3" 
      aria-label="Remove"
    ></button>
    
    <div class="mb-3">
      <label class="form-label fw-semibold text-secondary small text-uppercase tracking-wide">
        {{ field.label }}
        <span v-if="field.required" class="text-danger">*</span>
      </label>
      
      <!-- Text-like inputs -->
      <input
        v-if="['text', 'email', 'number', 'date', 'tel', 'url'].includes(field.type)"
        :type="field.type"
        :value="modelValue"
        @input="updateValue"
        :placeholder="field.placeholder"
        :required="field.required"
        class="form-control form-control-lg"
        :class="{ 'is-invalid': error }"
      />

      <!-- Textarea -->
      <textarea
        v-else-if="field.type === 'textarea'"
        :value="modelValue"
        @input="updateValue"
        :placeholder="field.placeholder"
        :required="field.required"
        class="form-control form-control-lg"
        :class="{ 'is-invalid': error }"
        rows="3"
      ></textarea>

      <!-- Select -->
      <select
        v-else-if="field.type === 'select'"
        :value="modelValue"
        @change="updateValue"
        :required="field.required"
        class="form-select form-select-lg"
        :class="{ 'is-invalid': error }"
      >
        <option value="">-- Select an option --</option>
        <option v-for="opt in field.options" :key="opt" :value="opt">{{ opt }}</option>
      </select>

      <!-- Radio -->
      <div v-else-if="field.type === 'radio'" :class="{ 'is-invalid': error }">
        <div v-for="opt in field.options" :key="opt" class="form-check form-check-inline">
          <input
            class="form-check-input"
            type="radio"
            :name="field.id"
            :value="opt"
            :checked="modelValue === opt"
            @change="$emit('update:modelValue', opt)"
            :required="field.required && !modelValue"
          />
          <label class="form-check-label">{{ opt }}</label>
        </div>
      </div>

      <!-- Checkbox -->
      <div v-else-if="field.type === 'checkbox'" class="form-check">
        <input
          class="form-check-input"
          type="checkbox"
          :checked="modelValue === true"
          @change="$emit('update:modelValue', ($event.target as HTMLInputElement).checked)"
          :required="field.required && !modelValue"
        />
        <label class="form-check-label">{{ field.label }}</label>
      </div>

      <div v-if="error" class="invalid-feedback d-block mt-2">
        <i class="bi bi-exclamation-circle me-1"></i>
        {{ error }}
      </div>
    </div>
  </div>
</template>

<style scoped>
.hover-shadow:hover {
  box-shadow: var(--shadow-md) !important;
  transform: translateY(-2px);
}
.transition-all {
  transition: all 0.3s ease;
}
</style>
