@echo off
title Genshin Impact ReShade 2023 Mod Pack - Start game
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                   Made by Sefinek - Start game & echo.
call Data\header.cmd
echo                        * Information *
echo         If you're using Windows 10, I recommend download
echo       Windows Terminal from the Microsoft Store. Good luck! & echo.


echo 1/8 - Checking if git is installed...
if exist "C:\Program Files\Git\cmd\git.exe" (
    git -v
    echo.

    echo 2/8 - Checking for new updates...
    git fetch
    git pull
) else (
    echo [x] NOT INSTALLED! I CAN'T CHECK FOR NEW UPDATES.
    echo [i] PLEASE DOWNLOAD: https://git-scm.com/downloads
)
echo.


echo 3/8 - Checking game localization...
if exist "C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe" (
    echo [✓] Found.
    echo [i] Patch: C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe
) else (
    echo [x] Not found. Where is the game installed?
)
echo.


echo 4/8 - Checking required processes...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [x] The application "unlockfps_clr.exe" [Genshin FPS Unlocker] is already running.
    echo [i] Close it and try again.

    goto close-window
) else (
    echo [✓] Ok. Everything is ready to run FPS Unlocker.
)
echo.


echo 5/8 - Setting execution policy...
powershell.exe Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
echo [✓] Done. & echo.


echo 6/8 - Starting...
echo [i] Click "Yes" in the two "User Account Control" (UAC) notifications. File inject.exe and unlockfps_clr.exe. & echo.
powershell.exe Data\ReShade.ps1


echo 7/8 - Waiting for unlockfps_clr.exe...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] Ok.
) else (
    goto error
)
echo.


echo 8/8 - Waiting for inject.exe...
tasklist /fi "ImageName eq inject.exe" /fo csv 2>NUL | find /I "inject.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] Done! If you need help, add me on Discord Sefinek#0001.
    echo [i] Visit my YouTube channel. Thank you.
    echo [i] https://www.youtube.com/channel/UClrAIcAzcqIMbvGXZqK7e0A
    goto close-window
) else (
    goto error
)




:error
    echo [x] Oh, sorry. For some reason, the application failed to start.
    echo [i] If you need help, please create a new Issue on GitHub or add me on Discord.
    echo [i] My username: Sefinek#0001
    echo [i] GitHub: https://github.com/sefinek24/genshin-impact-reshade-2023/issues
    echo.

    goto close-window

:close-window
    echo.
    set /p null="[i] Press ENTER to close this window."
    exit