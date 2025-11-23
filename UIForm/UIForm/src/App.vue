<template>
  <div id="app" class="d-flex min-vh-100 bg-slate-50">
    <!-- Sidebar -->
    <aside class="sidebar d-flex flex-column flex-shrink-0 p-3 bg-white shadow-sm" style="width: 280px;">
      <a href="/" class="d-flex align-items-center mb-3 mb-md-0 me-md-auto link-dark text-decoration-none">
        <div class="icon-box bg-primary text-white rounded-3 p-2 me-2">
          <i class="bi bi-ui-checks fs-4"></i>
        </div>
        <span class="fs-4 fw-bold tracking-tight">UIForm</span>
      </a>
      <hr>
      <ul class="nav nav-pills flex-column mb-auto">
        <li class="nav-item">
          <router-link to="/" class="nav-link d-flex align-items-center gap-2" active-class="active" aria-current="page">
            <i class="bi bi-plus-circle"></i>
            Add Form
          </router-link>
        </li>
        <li>
          <router-link to="/search" class="nav-link d-flex align-items-center gap-2" active-class="active">
            <i class="bi bi-list-check"></i>
            All Submissions
          </router-link>
        </li>
      </ul>
    </aside>

    <!-- Main Content -->
    <main class="flex-grow-1 overflow-auto">
      <div class="container-fluid p-4 p-lg-5">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </div>
    </main>
  </div>
</template>

<style scoped>
.sidebar {
  z-index: 1000;
}
.nav-link {
  color: var(--text-secondary);
  font-weight: 500;
  padding: 0.75rem 1rem;
  border-radius: var(--radius-md);
  transition: all 0.2s;
}
.nav-link:hover {
  background-color: #f1f5f9;
  color: var(--primary-color);
}
.nav-link.active {
  background-color: var(--primary-color);
  color: white;
  box-shadow: var(--shadow-md);
}
.icon-box {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
}
.tracking-tight {
  letter-spacing: -0.025em;
}
</style>
<script setup lang="ts">
</script>