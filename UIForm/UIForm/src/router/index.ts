import { createRouter, createWebHistory } from 'vue-router';
import AddForm from '../views/AddForm.vue';
import SearchForms from '../views/SearchForms.vue';

const routes = [
  {
    path: '/',
    name: 'AddForm',
    component: AddForm,
  },
  {
    path: '/search',
    name: 'SearchForms',
    component: SearchForms,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
