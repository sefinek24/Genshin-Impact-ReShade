@echo off
REM This batch file waits for the Genshin Impact Mod Launcher to start up.
REM It then exits.

title Waiting for launcher
chcp 65001 > nul

REM Wait for the launcher to start up.
echo Waiting for launcher...
cd "...."
start "" /b "Genshin Stella Mod.exe"
timeout /t 10 > nul

REM Exit.
echo Exiting...
exit