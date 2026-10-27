const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('electronAPI', {
  onApiReady: (cb) => ipcRenderer.on('api-ready', cb),
  onApiError: (cb) => ipcRenderer.on('api-error', (_e, msg) => cb(msg)),
})
