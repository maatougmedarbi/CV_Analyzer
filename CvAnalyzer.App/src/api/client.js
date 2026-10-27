const BASE = 'http://localhost:5298/api'

async function request(path, options = {}) {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options.headers },
    ...options,
  })

  if (!res.ok) {
    const text = await res.text()
    throw new Error(text || `HTTP ${res.status}`)
  }

  const text = await res.text()
  return text ? JSON.parse(text) : null
}

// Candidates
export const getCandidates = () => request('/candidates')
export const getCandidate = (id) => request(`/candidates/${id}`)
export const deleteCandidate = (id) => request(`/candidates/${id}`, { method: 'DELETE' })
export const generateSummary = (id) => request(`/candidates/${id}/summary`, { method: 'POST' })

// CV Upload
export const uploadCv = (file) => {
  const form = new FormData()
  form.append('file', file)
  return fetch(`${BASE}/cvupload`, { method: 'POST', body: form }).then(async (res) => {
    if (!res.ok) throw new Error(await res.text())
    return res.json()
  })
}

// Job Profiles
export const getJobProfiles = () => request('/jobprofiles')
export const getJobProfile = (id) => request(`/jobprofiles/${id}`)
export const createJobProfile = (data) => request('/jobprofiles', { method: 'POST', body: JSON.stringify(data) })
export const updateJobProfile = (id, data) => request(`/jobprofiles/${id}`, { method: 'PUT', body: JSON.stringify(data) })
export const deleteJobProfile = (id) => request(`/jobprofiles/${id}`, { method: 'DELETE' })

// Match
export const calculateMatch = (candidateId, jobProfileId) =>
  request(`/match/${candidateId}/${jobProfileId}`, { method: 'POST' })

export const matchByProfile = (jobProfileId) =>
  request(`/match/by-profile/${jobProfileId}`)
