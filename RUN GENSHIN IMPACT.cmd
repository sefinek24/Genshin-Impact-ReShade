@echo off
title Genshin Impact ReShade 2023 Mod Pack - Start game
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                   Made by Sefinek - Start game & echo.
:call Data\header.cmd
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
    echo [i] PLEASE DOWNLOAD: https://git-scm.com/downloads & echo.

    echo 2/8 - Checking for new updates...
    echo [x] Canceled.
)
echo.


echo 3/8 - Checking game localization...
if exist "C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe" (
    echo [✓] Found: C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe
) else (
    echo [x] Not found. Where is the game installed?
)
echo.


echo 4/8 - Checking required processes...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [x] The application "unlockfps_clr.exe" [Genshin FPS Unlocker] is already running.
    echo [i] Close it and try again. & echo.

    pause
) else (
    echo [✓] Done.
)
echo.







echo.⠀  ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀ ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀  ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.  ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿
echo.⠀  ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.   ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀  ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀  ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v2.0.0 [SV_19112022_200-002]
echo.   ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.5.0
echo.  ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.1
echo. ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.  ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo =========================================================================================
echo                                  NEW UPDATE IS AVAILABLE! & echo.
echo.

echo 5/5 - Finalizing the update...
copy rename.cmd %temp%\rename.cmd
call %temp%\rename.cmd
exit