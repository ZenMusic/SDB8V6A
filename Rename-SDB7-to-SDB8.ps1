# Requires: PowerShell 5+
# Usage:   ./tools/Rename-SDB7-to-SDB8.ps1
# Optional: ./tools/Rename-SDB7-to-SDB8.ps1 -OldName SDB7 -NewName SDB8 -Root <repo-root>

[CmdletBinding()]
param(
  [string]$OldName = 'SDB7',
  [string]$NewName = 'SDB8',
  [string]$Root = (Get-Location).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Step([string]$msg) { Write-Host "==> $msg" -ForegroundColor Cyan }
function Backup-File([string]$path) {
  if (Test-Path $path) {
    $bak = "$path.bak"
    if (-not (Test-Path $bak)) { Copy-Item $path $bak -Force }
  }
}
function Update-TextFile([string]$path, [string]$old, [string]$new) {
  if (-not (Test-Path $path)) { return }
  $text = Get-Content -LiteralPath $path -Raw
  $repl = $text -replace [Regex]::Escape($old), $new
  if ($repl -ne $text) {
    Backup-File $path
    Set-Content -LiteralPath $path -Value $repl -NoNewline -Encoding UTF8
    Write-Host "   updated: $path" -ForegroundColor DarkGreen
  }
}

function Update-CsprojXml([string]$projPath, [string]$old, [string]$new) {
  if (-not (Test-Path $projPath)) { return }
  Backup-File $projPath

  try {
    [xml]$xml = Get-Content -LiteralPath $projPath -Raw
  } catch {
    # If parsing fails (SDK-style csproj should parse), fallback to text replacement:
    Update-TextFile -path $projPath -old $old -new $new
    return
  }

  $changed = $false
  $pgs = $xml.Project.PropertyGroup
  if ($pgs) {
    foreach ($pg in $pgs) {
      if ($pg.AssemblyName -and $pg.AssemblyName.InnerText -eq $old) {
        $pg.AssemblyName = $new; $changed = $true
      }
      if ($pg.RootNamespace -and $pg.RootNamespace.InnerText -eq $old) {
        # Only change RootNamespace if it exactly equals the old name
        $pg.RootNamespace = $new; $changed = $true
      }
      if ($pg.AssemblyTitle -and $pg.AssemblyTitle.InnerText -eq $old) {
        $pg.AssemblyTitle = $new; $changed = $true
      }
      if ($pg.Product -and $pg.Product.InnerText -eq $old) {
        $pg.Product = $new; $changed = $true
      }
      if ($pg.Company -and $pg.Company.InnerText -eq $old) {
        $pg.Company = $new; $changed = $true
      }
      if ($pg.Description -and $pg.Description.InnerText) {
        $newDesc = $pg.Description.InnerText -replace [Regex]::Escape($old), $new
        if ($newDesc -ne $pg.Description.InnerText) { $pg.Description = $newDesc; $changed = $true }
      }
    }
  }

  # Save XML if we changed any elements; also do a light path/text replace for includes like <ProjectReference Include="SDB7\...">
  if ($changed) { $xml.Save($projPath) }

  # Ensure path/name occurrences are updated too
  Update-TextFile -path $projPath -old $old -new $new
}

Write-Step "Root: $Root"
Push-Location $Root

try {
  # 1) Rename solution file SDB7.sln -> SDB8.sln (and update content)
  $sln = Join-Path $Root "$OldName.sln"
  if (-not (Test-Path $sln)) {
    # Try find any *.sln that contains old name
    $cand = Get-ChildItem -LiteralPath $Root -Filter '*.sln' -Recurse | Where-Object { $_.Name -match [Regex]::Escape($OldName) } | Select-Object -First 1
    if ($cand) { $sln = $cand.FullName }
  }
  if (Test-Path $sln) {
    Write-Step "Renaming solution file"
    $newSln = (Join-Path (Split-Path $sln -Parent) ($sln | Split-Path -Leaf)) -replace [Regex]::Escape("$OldName.sln"), "$NewName.sln"
    if ($newSln -ne $sln) { Rename-Item -LiteralPath $sln -NewName (Split-Path $newSln -Leaf) -Force; $sln = $newSln }
    # Update paths and project names inside the .sln
    Update-TextFile -path $sln -old $OldName -new $NewName
  } else {
    Write-Host "   No solution file containing '$OldName' found. Skipping rename." -ForegroundColor Yellow
  }

  # 2) Rename project folder SDB7 -> SDB8
  $oldProjectDir = Join-Path $Root $OldName
  if (-not (Test-Path $oldProjectDir)) {
    # Try find folder (depth 1) that equals old name
    $candDir = Get-ChildItem -LiteralPath $Root -Directory -Recurse | Where-Object { $_.Name -eq $OldName } | Select-Object -First 1
    if ($candDir) { $oldProjectDir = $candDir.FullName }
  }
  $newProjectDir = $null
  if (Test-Path $oldProjectDir) {
    Write-Step "Renaming project directory"
    $parent = Split-Path $oldProjectDir -Parent
    $newProjectDir = Join-Path $parent $NewName
    if (-not (Test-Path $newProjectDir)) {
      Rename-Item -LiteralPath $oldProjectDir -NewName $NewName -Force
    }
  } else {
    Write-Host "   No project directory named '$OldName' found. Skipping folder rename." -ForegroundColor Yellow
  }

  if (-not $newProjectDir) {
    # If we didn't rename, assume the new dir is under root
    $newProjectDir = Join-Path $Root $NewName
  }

  # 3) Rename .csproj SDB7.csproj -> SDB8.csproj and update XML
  if (Test-Path $newProjectDir) {
    $oldProjFile = Get-ChildItem -LiteralPath $newProjectDir -Filter "$OldName*.csproj" -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $oldProjFile) {
      # maybe already renamed; find any csproj inside
      $oldProjFile = Get-ChildItem -LiteralPath $newProjectDir -Filter '*.csproj' -File -ErrorAction SilentlyContinue | Select-Object -First 1
    }
    if ($oldProjFile) {
      $projPath = $oldProjFile.FullName
      $newProjPath = if ($oldProjFile.BaseName -eq $OldName) {
        Join-Path $newProjectDir "$NewName.csproj"
      } else {
        # Replace only prefix occurrence
        Join-Path $newProjectDir ($oldProjFile.Name -replace [Regex]::Escape($OldName), $NewName)
      }
      if ($newProjPath -and ((Split-Path $projPath -Leaf) -ne (Split-Path $newProjPath -Leaf))) {
        Write-Step "Renaming csproj: $(Split-Path $projPath -Leaf) -> $(Split-Path $newProjPath -Leaf)"
        Rename-Item -LiteralPath $projPath -NewName (Split-Path $newProjPath -Leaf) -Force
        $projPath = $newProjPath
      }
      # Update csproj xml + path/name occurrences
      Update-CsprojXml -projPath $projPath -old $OldName -new $NewName
    } else {
      Write-Host "   No .csproj found in '$newProjectDir'." -ForegroundColor Yellow
    }
  }

  # 4) Update references across solution: *.sln, *.csproj, *.props, *.targets
  Write-Step "Updating references across solution files"
  $filesToPatch = @()
  $filesToPatch += Get-ChildItem -LiteralPath $Root -Include *.sln,*.csproj,*.props,*.targets -Recurse -File -ErrorAction SilentlyContinue
  foreach ($f in $filesToPatch) {
    Update-TextFile -path $f.FullName -old "$OldName.csproj" -new "$NewName.csproj"
    Update-TextFile -path $f.FullName -old "$OldName\" -new "$NewName\"
    Update-TextFile -path $f.FullName -old $OldName -new $NewName
  }

  # 5) Optional: update launchSettings.json or solution filters if present
  Write-Step "Updating JSON settings referencing names (if any)"
  $jsons = Get-ChildItem -LiteralPath $Root -Include *.json,*.slnf -Recurse -File -ErrorAction SilentlyContinue
  foreach ($j in $jsons) { Update-TextFile -path $j.FullName -old $OldName -new $NewName }

  Write-Step "Done. Consider cleaning '.vs', 'bin', 'obj' and reopening '$NewName.sln' in Visual Studio."
}
finally {
  Pop-Location
}