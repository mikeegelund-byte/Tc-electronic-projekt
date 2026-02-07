@echo off
REM Nova System Manager - Quick Launch Script
echo Starting Nova System Manager...
echo.
cd /d "%~dp0"
dotnet run --project src\Nova.Presentation
pause
