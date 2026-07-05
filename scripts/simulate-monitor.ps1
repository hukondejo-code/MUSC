$iniPath = "bin\Debug\net10.0-windows\Settings.ini"
if (-not (Test-Path $iniPath)) {
	Write-Output "Settings.ini not found at $iniPath"
	exit 1
}
$lines = Get-Content $iniPath | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' -and $_ -notmatch '^\s*[#;]' }
$ini = @{}
foreach ($line in $lines) {
	if ($line -match '^(.*?)=(.*)$') {
		$k = $matches[1].Trim()
		$v = $matches[2].Trim()
		$ini[$k] = $v
	}
}
$prcEnRaw = $ini['PRCEN_GLOBAL']
$prcEn = $false
if ($null -ne $prcEnRaw) {
	if ($prcEnRaw -eq '1' -or $prcEnRaw -eq 'true') { $prcEn = $true } else { [bool]::TryParse($prcEnRaw,[ref]$prcEn) | Out-Null }
}
Write-Output "PRCEN_GLOBAL (auto-restart enabled): $prcEn"

# discover row indices by NEV_ keys
$indices = @()
foreach ($k in $ini.Keys) {
	if ($k -match '^NEV_(\d+)$') { $indices += [int]$matches[1] }
}
if ($indices.Count -eq 0) { Write-Output "No configured rows found."; exit 0 }
$max = ($indices | Measure-Object -Maximum).Maximum
Write-Output "Found rows: $($indices -join ', ') (max index = $max)"

for ($i=1; $i -le $max; $i++) {
	$nev = $ini["NEV_$i"]
	$mappa = $ini["MAPPA_$i"]
	$exe = $ini["EXE_$i"]
	$var = $ini["VAR_$i"]
	if (-not $nev) { Write-Output ("Row {0}: no NEV; skipping" -f $i); continue }
	Write-Output "---- Row $i ----"
	Write-Output "Name: $nev"
	Write-Output "Path: $mappa"
	Write-Output "Exe: $exe"
	Write-Output "VAR: $var"
	# check running processes
	try {
		$procs = Get-Process -Name $nev -ErrorAction SilentlyContinue
	} catch {
		$procs = @()
	}
	if ($procs -and $procs.Count -gt 0) {
		Write-Output "Status: Running ($($procs.Count) process(es)) - monitor will skip restart."
	} else {
		Write-Output "Status: Not running."
		if (-not $prcEn) {
			Write-Output "Auto-restart is disabled globally; monitor will not attempt restart."
		} else {
			# determine wait from VAR (assume VAR is seconds as used in monitor)
			$wait = 2
			if ($var) { [int]::TryParse($var,[ref]$wait) | Out-Null }
			if ($wait -lt 1) { $wait = 2 }
			Write-Output "Monitor would wait $wait second(s) (VAR_$i) then attempt to start: Start-Process -FilePath (Join-Path -Path '$mappa' -ChildPath '$exe')"
		}
	}
}
Write-Output "Simulation complete."