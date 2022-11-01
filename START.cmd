@echo off
echo ==========================================================================
echo                  GENSHIN IMPACT MOD PACK MADE BY SEFINEK
echo ==========================================================================
echo.

echo Downloaded version: v1.0.1 from 11/01/2022
echo ReShade version: v5.4.2
echo.

echo Checking if git is installed...
if exist "C:\Program Files\Git\cmd\git.exe" (
    git -v
    echo.

    echo Checking for new updates...
    git pull
) else (
    echo Not installed! I can't check for new updates.
    echo Please download: https://git-scm.com/downloads
)
echo.

echo Checking game localization...
if exist "C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe" (
    echo OK
) else (
    echo Not found. Where is the game installed?
)
echo.

echo Starting...
powershell.exe -file "ReShade.ps1"
echo Done! If you need help, please create a new Issue on GitHub.
echo https://github.com/sefinek24/genshin-impact-reshade-2023/issues && echo.


pause