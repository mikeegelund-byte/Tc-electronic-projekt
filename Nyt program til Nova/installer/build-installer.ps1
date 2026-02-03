#!/usr/bin/env pwsh
# Build script for Nova System Manager WiX Installer

param(
    [string]$Configuration = 'Release',
    [string]$OutputPath = '.\installer\output',
    [string]$RuntimeIdentifier = 'win-x64',
    [switch]$SkipPublish = $false,
    [switch]$Help = $false
)

function Show-Help {
    $lines = @(
        'Build Nova System Manager WiX Installer (.msi)',
        '',
        'Usage:',
        '  .\build-installer.ps1 [-Configuration <config>] [-OutputPath <path>] [-RuntimeIdentifier <rid>] [-SkipPublish]',
        '',
        'Parameters:',
        '  -Configuration       Build configuration (default: Release)',
        '  -OutputPath          Output directory for MSI file (default: .\installer\output)',
        '  -RuntimeIdentifier   Publish RID (default: win-x64)',
        '  -SkipPublish         Skip the dotnet publish step (use existing publish folder)',
        '  -Help                Show this help message',
        '',
        'Examples:',
        '  .\build-installer.ps1',
        '  .\build-installer.ps1 -Configuration Debug',
        '  .\build-installer.ps1 -RuntimeIdentifier win-x64',
        '  .\build-installer.ps1 -SkipPublish -OutputPath .\output',
        '',
        'Requirements:',
        '  - .NET 8.0 SDK',
        '  - WiX Toolset v4+ (install with: dotnet tool install --global wix)',
        ''
    )
    $lines | ForEach-Object { Write-Host $_ }
}

if ($Help) {
    Show-Help
    exit 0
}

# Script configuration
$ErrorActionPreference = 'Stop'
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$PublishDir = Join-Path $ProjectRoot 'publish'
$InstallerDir = $PSScriptRoot
$OutputDir = if ([IO.Path]::IsPathRooted($OutputPath)) { $OutputPath } else { Join-Path $ProjectRoot $OutputPath }
$ProjectFile = Join-Path $ProjectRoot 'src\Nova.Presentation\Nova.Presentation.csproj'

# Ensure output directory exists
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

Write-Host '========================================' -ForegroundColor Cyan
Write-Host 'Nova System Manager - Installer Build' -ForegroundColor Cyan
Write-Host '========================================' -ForegroundColor Cyan
Write-Host ''

# Step 1: Publish the application (unless skipped)
if (-not $SkipPublish) {
    Write-Host 'Step 1: Publishing application...' -ForegroundColor Yellow
    Write-Host ('  Configuration: ' + $Configuration) -ForegroundColor Gray
    Write-Host ('  Runtime: ' + $RuntimeIdentifier) -ForegroundColor Gray
    Write-Host ('  Output: ' + $PublishDir) -ForegroundColor Gray
    Write-Host ''

    $publishArgs = @(
        'publish',
        $ProjectFile,
        '-c', $Configuration,
        '-o', $PublishDir,
        '-r', $RuntimeIdentifier,
        '--self-contained', 'false',
        '-p:PublishSingleFile=false',
        '-p:Platform=x64'
    )

    & dotnet @publishArgs

    if ($LASTEXITCODE -ne 0) {
        Write-Host ('ERROR: dotnet publish failed with exit code ' + $LASTEXITCODE) -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Host '  ✓ Application published successfully' -ForegroundColor Green
    Write-Host ''
} else {
    Write-Host 'Step 1: Skipping publish (using existing publish folder)' -ForegroundColor Yellow
    Write-Host ''
}

# Step 2: Verify publish folder exists
if (-not (Test-Path $PublishDir)) {
    Write-Host ('ERROR: Publish folder not found at: ' + $PublishDir) -ForegroundColor Red
    Write-Host 'Run without -SkipPublish to create it.' -ForegroundColor Red
    exit 1
}

# Count files in publish folder
$publishedFiles = Get-ChildItem -Path $PublishDir -Recurse -File
Write-Host ('  Found ' + $publishedFiles.Count + ' files in publish folder') -ForegroundColor Gray
Write-Host ''

# Step 3: Check WiX toolset installation
Write-Host 'Step 2: Checking WiX toolset...' -ForegroundColor Yellow

$wixInstalled = $false
try {
    $wixVersion = & wix --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        $wixInstalled = $true
        Write-Host ('  ✓ WiX toolset found: ' + $wixVersion) -ForegroundColor Green
    }
} catch {
    # WiX not found
}

if (-not $wixInstalled) {
    Write-Host '  ✗ WiX toolset not found' -ForegroundColor Red
    Write-Host ''
    Write-Host 'Installing WiX toolset...' -ForegroundColor Yellow
    & dotnet tool install --global wix

    if ($LASTEXITCODE -ne 0) {
        Write-Host 'ERROR: Failed to install WiX toolset' -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Host '  ✓ WiX toolset installed successfully' -ForegroundColor Green
}
Write-Host ''

# Step 4: Harvest published files (PowerShell)
Write-Host 'Step 3: Harvesting published files...' -ForegroundColor Yellow

$componentsWxs = Join-Path $InstallerDir 'Components.wxs'

function New-SafeId {
    param(
        [string]$Text,
        [string]$Prefix
    )

    $hashBytes = [System.Security.Cryptography.SHA256]::Create().ComputeHash(
        [System.Text.Encoding]::UTF8.GetBytes($Text)
    )
    $hash = ([System.BitConverter]::ToString($hashBytes)) -replace "-", ""
    return "$Prefix$($hash.Substring(0, 12))"
}

function Write-ComponentsWxs {
    param(
        [string]$PublishRoot,
        [string]$OutputPath
    )

    $publishRootFull = (Resolve-Path $PublishRoot).Path
    $files = Get-ChildItem -Path $PublishRoot -Recurse -File
    $componentsByDir = @{}
    $components = @()
    $directoryTree = @{
        Id = "INSTALLFOLDER"
        Name = ""
        Children = @{}
    }

    function Get-OrCreate-DirNode {
        param(
            [string[]]$Segments
        )

        $node = $directoryTree
        foreach ($segment in $Segments) {
            if (-not $node.Children.ContainsKey($segment)) {
                $node.Children[$segment] = @{
                    Id = (New-SafeId -Text ($node.Id + "_" + $segment) -Prefix "Dir")
                    Name = $segment
                    Children = @{}
                }
            }
            $node = $node.Children[$segment]
        }
        return $node
    }

    $index = 1
    foreach ($file in $files) {
        $relPath = $file.FullName.Substring($publishRootFull.Length).TrimStart('\', '/')
        $relDir = [IO.Path]::GetDirectoryName($relPath)
        if ($null -eq $relDir) {
            $relDir = ""
        }
        $relDirNormalized = $relDir -replace '\\', '/'
        $segments = @()
        if ($relDirNormalized) {
            $segments = $relDirNormalized -split '/'
        }

        $dirNode = Get-OrCreate-DirNode -Segments $segments
        $componentId = "Comp$index"
        $fileId = "File$index"
        $guid = [guid]::NewGuid().ToString().ToUpper()
        $components += [pscustomobject]@{
            Id = $componentId
            FileId = $fileId
            Guid = $guid
            RelativePath = $relPath
            DirectoryNode = $dirNode
            DirectoryKey = $relDirNormalized
        }

        if (-not $componentsByDir.ContainsKey($relDirNormalized)) {
            $componentsByDir[$relDirNormalized] = @()
        }
        $componentsByDir[$relDirNormalized] += $components[-1]
        $index++
    }

    $sb = New-Object System.Text.StringBuilder
    [void]$sb.AppendLine('<?xml version="1.0" encoding="UTF-8"?>')
    [void]$sb.AppendLine('<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">')
    [void]$sb.AppendLine('  <Fragment>')
    [void]$sb.AppendLine('    <DirectoryRef Id="INSTALLFOLDER">')

    function Write-Directory {
        param(
            [hashtable]$Node,
            [string]$PathKey,
            [int]$IndentLevel
        )

        $indent = '  ' * $IndentLevel
        foreach ($child in $Node.Children.Values | Sort-Object Name) {
            [void]$sb.AppendLine("$indent<Directory Id=`"$($child.Id)`" Name=`"$($child.Name)`">")
            $childPathKey = if ($PathKey) { "$PathKey/$($child.Name)" } else { $child.Name }
            Write-Directory -Node $child -PathKey $childPathKey -IndentLevel ($IndentLevel + 1)
            [void]$sb.AppendLine("$indent</Directory>")
        }

        if ($componentsByDir.ContainsKey($PathKey)) {
            foreach ($component in $componentsByDir[$PathKey]) {
                $fileSource = $component.RelativePath -replace '/', '\\'
                [void]$sb.AppendLine("$indent<Component Id=`"$($component.Id)`" Guid=`"$($component.Guid)`">")
                [void]$sb.AppendLine("$indent  <File Id=`"$($component.FileId)`" Source=`"`$(var.PublishDir)\\$fileSource`" KeyPath=`"yes`" />")
                [void]$sb.AppendLine("$indent</Component>")
            }
        }
    [void]$sb.AppendLine('    <ComponentGroup Id="PublishedFiles" Directory="INSTALLFOLDER">')

    $i = 1
    foreach ($file in $files) {
        $relPath = $file.FullName.Substring($publishRootFull.Length).TrimStart('\') -replace '\\', '/'
        $guid = [guid]::NewGuid().ToString().ToUpper()
        [void]$sb.AppendLine('      <Component Id="Comp' + $i + '" Guid="' + $guid + '">')
        [void]$sb.AppendLine('        <File Id="File' + $i + '" Source="$(var.PublishDir)/' + $relPath + '" KeyPath="yes" />')
        [void]$sb.AppendLine('      </Component>')
        $i++
    }

    Write-Directory -Node $directoryTree -PathKey "" -IndentLevel 3
    [void]$sb.AppendLine('    </DirectoryRef>')
    [void]$sb.AppendLine('  </Fragment>')
    [void]$sb.AppendLine('  <Fragment>')
    [void]$sb.AppendLine('    <ComponentGroup Id="PublishedFiles">')
    foreach ($component in $components) {
        [void]$sb.AppendLine("      <ComponentRef Id=`"$($component.Id)`" />")
    }
    [void]$sb.AppendLine('    </ComponentGroup>')
    [void]$sb.AppendLine('  </Fragment>')
    [void]$sb.AppendLine('</Wix>')

    Set-Content -Path $OutputPath -Value $sb.ToString()
    return $files.Count
}

Write-Host "  Attempting WiX harvest..." -ForegroundColor Gray
$harvestArgs = @(
    "harvest",
    "dir", $PublishDir,
    "-cg", "PublishedFiles",
    "-gg",
    "-scom",
    "-sfrag",
    "-srd",
    "-sreg",
    "-dr", "INSTALLFOLDER",
    "-var", "var.PublishDir",
    "-out", $componentsWxs
)

& wix @harvestArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "  WiX harvest failed (exit code $LASTEXITCODE). Falling back to PowerShell generator." -ForegroundColor Yellow
    $count = Write-ComponentsWxs -PublishRoot $PublishDir -OutputPath $componentsWxs
    Write-Host "  ✓ Components generated with PowerShell ($count files)" -ForegroundColor Green
} else {
    Write-Host "  ✓ Components generated with WiX harvest" -ForegroundColor Green
}
Write-Host ""
$count = Write-ComponentsWxs -PublishRoot $PublishDir -OutputPath $componentsWxs
Write-Host ('  ✓ Components generated with PowerShell (' + $count + ' files)') -ForegroundColor Green
Write-Host ''

# Step 5: Build the MSI installer
Write-Host 'Step 4: Building MSI installer...' -ForegroundColor Yellow

$productWxs = Join-Path $InstallerDir 'Product.wxs'
$msiOutput = Join-Path $OutputDir 'NovaSystemManager.msi'

Write-Host ('  Source: ' + $productWxs) -ForegroundColor Gray
Write-Host ('  Components: ' + $componentsWxs) -ForegroundColor Gray
Write-Host ('  Output: ' + $msiOutput) -ForegroundColor Gray
Write-Host ''

$wixArgs = @(
    'build',
    '-arch', 'x64',
    $productWxs,
    $componentsWxs,
    '-o', $msiOutput,
    '-d', ('PublishDir=' + $PublishDir)
)

& wix @wixArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host ('ERROR: WiX build failed with exit code ' + $LASTEXITCODE) -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host '  ✓ MSI installer built successfully' -ForegroundColor Green
Write-Host ''

# Step 6: Display results
Write-Host '========================================' -ForegroundColor Cyan
Write-Host 'Build Complete!' -ForegroundColor Green
Write-Host '========================================' -ForegroundColor Cyan
Write-Host ''
Write-Host 'Installer location:' -ForegroundColor Yellow
Write-Host ('  ' + $msiOutput) -ForegroundColor White
Write-Host ''

# Display file size
$msiFile = Get-Item $msiOutput
$sizeMB = [math]::Round($msiFile.Length / 1MB, 2)
Write-Host ('File size: ' + $sizeMB + ' MB') -ForegroundColor Gray
Write-Host ''

Write-Host 'Next steps:' -ForegroundColor Yellow
Write-Host '  1. Test the installer on a clean Windows machine' -ForegroundColor Gray
Write-Host '  2. Verify the application launches correctly' -ForegroundColor Gray
Write-Host '  3. Test the uninstall process' -ForegroundColor Gray
Write-Host ''
