<script setup lang="ts">
import { defineProps } from 'vue';

defineProps({
  form: {
    type: Object,
    required: true,
  },
  index: {
    type: Number,
    required: true,
  },
});

// Get filename
function getFileName(path: string): string {
  if (!path) return 'Unknown File';
  const normalized = path.replace(/\\/g, '/');
  const parts = normalized.split(/wwwroot\//i);
  if (parts.length > 1) {
    return parts[1];
  }
  return normalized.split('/').pop() || 'Unknown File';
}

// Download file
async function downloadFile(att: any) {
  const path = att.path || att.Path;
  if (!path) {
    alert('File path is missing');
    return;
  }

  try {
    const response = await fetch(`/api/downloadfile?fileName=${encodeURIComponent(path)}`);
    if (!response.ok) {
      throw new Error(`Download failed: ${response.statusText}`);
    }

    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    const fileName = path.split(/[\\/]/).pop() || 'download';
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  } catch (error: any) {
    alert(`Failed to download file: ${error.message}`);
  }
}
</script>

<template>
  <div class="p-4 border rounded-3 bg-white shadow-sm hover-shadow transition-all">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h3 class="h5 fw-bold text-primary mb-0">
        <i class="bi bi-file-text me-2"></i>Submission #{{ index + 1 }}
      </h3>
      <span class="badge bg-light text-secondary border">
        {{ new Date().toLocaleDateString() }}
      </span>
    </div>
    
    <div class="bg-slate-50 rounded-3 p-4 mb-3">
      <div class="row g-4" v-if="form.jsonDocument || form.JsonDocument">
        <template v-for="[key, value] in Object.entries(form.jsonDocument || form.JsonDocument)" :key="key">
          <div class="col-12 col-md-6">
            <h6 class="fw-bold text-secondary small text-uppercase tracking-wide mb-1">{{ key }}</h6>
            <div class="text-dark bg-white p-2 rounded border-start border-4 border-primary shadow-sm">
              {{ typeof value === 'boolean' ? (value ? 'Yes' : 'No') : value }}
            </div>
          </div>
        </template>
      </div>
      <div v-else class="text-muted fst-italic">No form data available</div>
    </div>

    <div v-if="(form.attachmentList || form.AttachmentList) && (form.attachmentList || form.AttachmentList).length">
      <h6 class="fw-bold text-secondary small text-uppercase tracking-wide mb-2">
        <i class="bi bi-paperclip me-1"></i>Attachments
      </h6>
      <div class="d-flex flex-wrap gap-2">
        <a 
          v-for="att in (form.attachmentList || form.AttachmentList)" 
          :key="att.guid || att.Guid" 
          href="#"
          @click.prevent="downloadFile(att)"
          class="btn btn-sm btn-outline-primary d-flex align-items-center gap-2"
        >
          <i class="bi bi-file-earmark-arrow-down"></i>
          {{ getFileName(att.path || att.Path) }}
        </a>
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
.bg-slate-50 {
  background-color: #f8fafc;
}
</style>
