@echo off
title Update - Genshin Impact Mod Pack 2023
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                   Made by Sefinek - Update & echo.
call Data\header.cmd

echo 1/4 - Checking if git is installed...
if exist "C:\Program Files\Git\cmd\git.exe" (
    git -v
    echo.

    echo 2/4 - End process "Genshin Impact Mod Pack.exe"...
    taskkill /IM "Genshin Impact Mod Pack.exe"
    echo.

    echo 3/4 - Checking for new updates...
    git fetch
    git pull
) else (
    echo [x] Not installed! I can't check for new updates.
    echo [i] Please download: https://git-scm.com/downloads
)
echo.

echo 4/4 - Relaunching...
echo [i] You can close this window.
"Genshin Impact Mod Pack.exe"

echo [i] The process was ended.
timeout /t 6 /nobreak
exit