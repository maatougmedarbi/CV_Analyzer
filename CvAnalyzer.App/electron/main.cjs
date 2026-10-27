const { app, BrowserWindow, ipcMain } = require('electron')
const path = require('path')
const { spawn } = require('child_process')

let apiProcess = null
let mainWindow = null

const isDev = process.env.NODE_ENV !== 'production'
const API_PORT = 5298

function startApiProcess() {
  if (isDev) {
    // In dev, assume the API is already running
    return
  }

  const apiExePath = path.join(process.resourcesPath, 'api', 'CvAnalyzer.Api.exe')

  apiProcess = spawn(apiExePath, [], {
    detached: false,
    stdio: 'ignore',
    env: {
      ...process.env,
      ASPNETCORE_URLS: `http://localhost:${API_PORT}`,
      ASPNETCORE_ENVIRONMENT: 'Production',
    },
  })

  apiProcess.on('error', (err) => {
    if (mainWindow) {
      mainWindow.webContents.send('api-error', `Failed to start API: ${err.message}`)
    }
  })
}

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1400,
    height: 900,
    minWidth: 1100,
    minHeight: 700,
    frame: false,
    titleBarStyle: 'hidden',
    titleBarOverlay: {
      color: 'rgba(0,0,0,0)',
      symbolColor: '#64748b',
      height: 40,
    },
    backgroundColor: '#f8fafc',
    webPreferences: {
      preload: path.join(__dirname, 'preload.cjs'),
      contextIsolation: true,
      nodeIntegration: false,
    },
    show: false,
  })

  if (isDev) {
    mainWindow.loadURL('http://localhost:5173')
    mainWindow.webContents.openDevTools({ mode: 'detach' })
  } else {
    mainWindow.loadFile(path.join(__dirname, '../dist/index.html'))
  }

  mainWindow.once('ready-to-show', () => {
    mainWindow.show()
    if (!isDev) {
      setTimeout(() => {
        mainWindow.webContents.send('api-ready')
      }, 2000)
    }
  })

  mainWindow.on('closed', () => {
    mainWindow = null
  })
}

app.whenReady().then(() => {
  startApiProcess()
  createWindow()

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow()
  })
})

app.on('window-all-closed', () => {
  if (apiProcess) {
    apiProcess.kill()
  }
  if (process.platform !== 'darwin') {
    app.quit()
  }
})
