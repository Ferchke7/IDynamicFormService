<script setup lang="ts">
import { ref } from 'vue';
import NewInput from '../components/NewInput.vue';
import DynamicField from '../components/DynamicField.vue';

interface Field {
  id: string;
  label: string;
  type: 'text' | 'email' | 'number' | 'date' | 'tel' | 'url' | 'textarea' | 'select' | 'radio' | 'checkbox';
  required: boolean;
  placeholder?: string;
  options?: string[];
}

interface FormErrors {
  [key: string]: string;
}

const fields = ref<Field[]>([]);
const formData = ref<Record<string, any>>({});
const errors = ref<FormErrors>({});
const isSubmitting = ref(false);
const submitMessage = ref('');
const showModal = ref(false);

function openModal() {
  showModal.value = true;
}

function closeModal() {
  showModal.value = false;
}

function saveField(field: Field) {
  field.id = Date.now().toString();
  fields.value.push(field);
  formData.value[field.id] = '';
  closeModal();
}

function removeField(fieldId: string) {
  fields.value = fields.value.filter(f => f.id !== fieldId);
  delete formData.value[fieldId];
  delete errors.value[fieldId];
}

function validateForm(): boolean {
  errors.value = {};
  
  fields.value.forEach(field => {
    const value = formData.value[field.id];
    
    if (field.required) {
      if (field.type === 'checkbox' && !value) {
        errors.value[field.id] = 'This field is required';
      } else if (field.type !== 'checkbox' && (!value || value.toString().trim() === '')) {
        errors.value[field.id] = 'This field is required';
      }
    }
    
    if (value && field.type === 'email') {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(value)) {
        errors.value[field.id] = 'Invalid email format';
      }
    }
    
    if (value && field.type === 'url') {
      try {
        new URL(value);
      } catch {
        errors.value[field.id] = 'Invalid URL format';
      }
    }
    
    if (value && field.type === 'tel') {
      const phoneRegex = /^[\d\s\-\+\(\)]+$/;
      if (!phoneRegex.test(value)) {
        errors.value[field.id] = 'Invalid phone number format';
      }
    }
  });
  
  return Object.keys(errors.value).length === 0;
}

async function handleSubmit() {
  const fileInput = document.getElementById('file-input') as HTMLInputElement;
  const files = fileInput.files ? Array.from(fileInput.files) : [];
  
  if (fields.value.length < 1) {
    submitMessage.value = 'Please add at least 1 field to the form';
    return;
  }
  
  if (!validateForm()) {
    submitMessage.value = 'Please fix the errors in the form';
    return;
  }

  isSubmitting.value = true;
  submitMessage.value = '';
  
  try {
    const uploadPromises = files.map(file => {
      const fileData = new FormData();
      fileData.append('file', file);
      
      return fetch('/api/attachments', {
        method: 'POST',
        body: fileData,
      }).then(response => {
        if (!response.ok) {
          throw new Error(`File upload failed for ${file.name}: ${response.statusText}`);
        }
        return response.json();
      });
    });

    const uploadedFiles = await Promise.all(uploadPromises);

    const formJson: Record<string, any> = {};
    fields.value.forEach(field => {
      formJson[field.label] = formData.value[field.id] ?? null;
    });

    const newForm = {
      Form: formJson,
      AttachmentIds: uploadedFiles.map(file => file.guid),
    };

    const submitResponse = await fetch('/api/submissions', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(newForm),
    });

    if (!submitResponse.ok) {
      throw new Error(`Form submission failed: ${submitResponse.statusText}`);
    }

    submitMessage.value = 'Form submitted successfully!';
    fields.value = [];
    formData.value = {};
    if (fileInput) {
      fileInput.value = '';
    }

    setTimeout(() => {
      submitMessage.value = '';
    }, 3000);
  } catch (error: any) {
    submitMessage.value = `Error: ${error.message}`;
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<template>
  <div class="row justify-content-center">
    <div class="col-12 col-xl-10">
      <div class="premium-card p-5">
        <div class="d-flex justify-content-between align-items-center mb-5">
          <div>
            <h2 class="display-6 fw-bold text-primary mb-1">Build Your Form</h2>
            <p class="text-muted mb-0">Create dynamic forms with ease.</p>
          </div>
          <button type="button" @click="openModal" class="btn btn-primary d-flex align-items-center gap-2 shadow-sm">
            <i class="bi bi-plus-lg"></i>
            Add Field
          </button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <transition-group name="slide-up" tag="div" class="row g-4">
            <div v-if="fields.length === 0" key="empty-state" class="col-12">
              <div class="text-center p-5 border-2 border-dashed rounded-3 bg-light">
                <div class="mb-3 text-muted">
                  <i class="bi bi-clipboard-plus display-4"></i>
                </div>
                <h4 class="text-muted">No fields added yet</h4>
                <p class="text-muted mb-0">Click "Add Field" to start building your form</p>
              </div>
            </div>

            <div v-for="field in fields" :key="field.id" class="col-12">
              <DynamicField
                :field="field"
                v-model="formData[field.id]"
                :error="errors[field.id]"
                @remove="removeField"
              />
            </div>
          </transition-group>

          <div v-if="fields.length > 0" class="mt-5 pt-4 border-top">
            <div class="mb-4">
              <label for="file-input" class="form-label fw-semibold text-secondary small text-uppercase tracking-wide">
                Attachments (optional)
              </label>
              <div class="input-group">
                <input
                  id="file-input"
                  type="file"
                  multiple
                  class="form-control form-control-lg"
                />
                <span class="input-group-text bg-light text-muted">
                  <i class="bi bi-paperclip"></i>
                </span>
              </div>
            </div>

            <div class="d-grid gap-2 d-md-flex justify-content-md-end">
              <button
                type="submit"
                :disabled="isSubmitting"
                class="btn btn-primary btn-lg px-5 shadow-lg"
              >
                <span v-if="!isSubmitting" class="d-flex align-items-center gap-2">
                  Submit Form <i class="bi bi-send"></i>
                </span>
                <span v-else class="d-flex align-items-center gap-2">
                  <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                  Submitting...
                </span>
              </button>
            </div>
          </div>

          <transition name="fade">
            <div v-if="submitMessage" class="alert mt-4 shadow-sm border-0 d-flex align-items-center" :class="submitMessage.includes('success') ? 'alert-success bg-success text-white' : 'alert-danger bg-danger text-white'">
              <i class="bi me-2 fs-4" :class="submitMessage.includes('success') ? 'bi-check-circle-fill' : 'bi-exclamation-triangle-fill'"></i>
              <div>{{ submitMessage }}</div>
            </div>
          </transition>
        </form>
      </div>
    </div>
    <NewInput v-if="showModal" @close="closeModal" @save="saveField" />
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
.border-dashed {
  border-style: dashed !important;
}
</style>
