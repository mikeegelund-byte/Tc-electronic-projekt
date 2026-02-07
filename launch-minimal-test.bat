@echo off
REM Nova System Manager - Minimal MIDI Test
echo Starting Minimal MIDI Test...
echo This tests basic MIDI communication.
echo.
cd /d "%~dp0"
dotnet run --project src\Nova.MinimalMidiTest
pause
