# Install Git for Windows (best-effort script)
# Usage: Open PowerShell as Administrator and run: .\install-git-windows.ps1
# This script will:
# 1) Check if 'git' is already available in PATH
# 2) Try to install via winget if available
# 3) Otherwise download Git for Windows installer and run it silently (Inno Setup)

param(
	[switch]$ForceDownload
)

function Has-Git {
	try {
		$null = & git --version 2>$null
		return $true
	} catch {
		return $false
	}
}

Write-Host "Checking for git..."
if (-not $ForceDownload -and (Has-Git)) {
	Write-Host "Git already found."
	exit 0
}

# Try winget
$winget = Get-Command winget -ErrorAction SilentlyContinue
if ($winget) {
	Write-Host "Attempting to install Git via winget..."
	try {
		winget install --id Git.Git -e --accept-package-agreements --accept-source-agreements -h
		if (Has-Git) { Write-Host "Git installed successfully via winget."; exit 0 }
	} catch {
		Write-Warning "winget install failed or not supported on this system. Will try direct installer next."
	}
}

# Fallback: download Git for Windows installer
$downloadUrl = 'https://github.com/git-for-windows/git/releases/latest/download/Git-64-bit.exe'
$tempPath = Join-Path $env:TEMP ("GitInstaller_{0}.exe" -f ([guid]::NewGuid().ToString()))
Write-Host "Downloading Git installer from $downloadUrl to $tempPath"
try {
	Invoke-WebRequest -Uri $downloadUrl -OutFile $tempPath -UseBasicParsing -ErrorAction Stop
} catch {
	Write-Error "Failed to download Git installer: $_"
	exit 2
}

Write-Host "Running installer (silent)... You may be prompted for elevation."
try {
	$proc = Start-Process -FilePath $tempPath -ArgumentList '/VERYSILENT','/NORESTART' -Wait -PassThru
	if ($proc.ExitCode -eq 0) {
		Write-Host "Installer exited with code 0."
	} else {
		Write-Warning "Installer exited with code $($proc.ExitCode)."
	}
} catch {
	Write-Error "Failed to run installer: $_"
	exit 3
} finally {
	try { Remove-Item -Path $tempPath -Force -ErrorAction SilentlyContinue } catch {}
}

if (Has-Git) {
	Write-Host "Git installed successfully. You may need to restart your terminal or Visual Studio to pick up PATH changes."
	exit 0
} else {
	Write-Warning "Git installation completed but 'git' not found in PATH. You may need to restart or add Git to PATH manually."
	exit 4
}
