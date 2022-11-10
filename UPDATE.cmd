@echo off
title Genshin Impact ReShade 2023 Mod Pack - Update
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                   Made by Sefinek - Update & echo.
call Data\header.cmd

echo 1/2 - Checking if git is installed...
if exist "C:\Program Files\Git\cmd\git.exe" (
    git -v
    echo.

    echo 2/2 - Checking for new updates...
    git fetch
    git pull
) else (
    echo [x] Not installed! I can't check for new updates.
    echo [i] Please download: https://git-scm.com/downloads
)
echo.

echo [i] You can close this window.
set /p null=
exit