@echo off
REM Nova System Manager - Publish & Launch
echo Nova System Manager - building...
cd /d "%~dp0"

dotnet publish src\Nova.Presentation -c Release -r win-x64 --self-contained -p:PublishTrimmed=false -o publish\Nova -v q 2>nul
if errorlevel 1 (
    echo BUILD FAILED - see errors above
    pause
    exit /b 1
)

echo Starting Nova System Manager...
start "" "publish\Nova\Nova.Presentation.exe"
