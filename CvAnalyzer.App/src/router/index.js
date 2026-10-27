import { createRouter, createWebHashHistory } from 'vue-router'
import Dashboard from '../views/Dashboard.vue'
import Upload from '../views/Upload.vue'
import Candidates from '../views/Candidates.vue'
import CandidateDetail from '../views/CandidateDetail.vue'
import JobProfiles from '../views/JobProfiles.vue'
import Match from '../views/Match.vue'

const routes = [
  { path: '/', component: Dashboard },
  { path: '/upload', component: Upload },
  { path: '/candidates', component: Candidates },
  { path: '/candidates/:id', component: CandidateDetail },
  { path: '/job-profiles', component: JobProfiles },
  { path: '/match', component: Match },
]

export default createRouter({
  history: createWebHashHistory(),
  routes,
})
