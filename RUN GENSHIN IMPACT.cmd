@echo off
echo ==========================================================================
echo                  GENSHIN IMPACT MOD PACK MADE BY SEFINEK
echo ==========================================================================
echo.

echo * Downloaded mod version: v1.0.2 from 05.11.2022
echo * ReShade version: v5.4.2
echo * FPS Unlocker version: v2.0.0
echo.


echo 1/5 - Checking if git is installed...
if exist "C:\Program Files\Git\cmd\git.exe" (
    git -v
    echo.

    echo 2/5 - Checking for new updates...
    git fetch
    git pull
) else (
    echo Not installed! I can't check for new updates.
    echo Please download: https://git-scm.com/downloads
)
echo.

echo 3/5 - Checking game localization...
if exist "C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe" (
    echo Found.
    echo Game patch: C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe
) else (
    echo Not found. Where is the game installed?
)
echo.

echo 4/5 - Checking the required processes...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo The application "unlockfps_clr.exe" [FPS Unlocker] is already running.

    pause && exit
) else (
    echo Ok. Everything is ready to run FPS Unlocker!
)
echo.

echo 5/5 - Starting...
echo Click "Yes" in the two "User Account Control" (UAC) notifications. File inject.exe and unlockfps_clr.exe. & echo.
powershell.exe -file "ReShade.ps1"

tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo Done! If you need help, please create a new Issue on GitHub.
) else (
    echo Ohh. For some reason, the application "unlockfps_clr.exe" failed to start.
    echo If you need help, please create a new Issue on GitHub.
)

echo https://github.com/sefinek24/genshin-impact-reshade-2023/issues

echo. && pause