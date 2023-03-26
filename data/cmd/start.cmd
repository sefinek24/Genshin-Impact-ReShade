@echo off
title Start game - Genshin Impact Mod Pack 2023
chcp 65001 > NUL
setlocal EnableDelayedExpansion
echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                Made by Sefinek - Start game
echo.⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v6.0.0.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.7.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.8
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/4 - Checking administrative permissions...
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [✓] No problems found. & echo.
) else (
    echo [x] Error. This command must be run as administrator.
    goto :missing_perms
)


echo 2/4 - Checking required processes...
tasklist /fi "ImageName eq Genshin Stella Mod Launcher.exe" /fo csv | find /I "Genshin Stella Mod Launcher.exe" >NUL && (
    echo [i] Genshin Stella Mod Launcher.exe - Closing...
    taskkill /F /IM "Genshin Stella Mod Launcher.exe"
) || (
    echo [✓] Genshin Stella Mod Launcher.exe - OK
)

tasklist /fi "ImageName eq GenshinImpact.exe" /fo csv | find /I "GenshinImpact.exe" >NUL && (
    echo [i] GenshinImpact.exe - Closing...
    taskkill /F /IM "GenshinImpact.exe"
) || (
    echo [✓] GenshinImpact.exe - OK
)

tasklist /fi "ImageName eq inject64.exe" /fo csv | find /I "inject64.exe" >NUL && (
    echo [i] inject64.exe - Closing...
    taskkill /F /IM "inject64.exe"
) || (
    echo [✓] inject64.exe - OK
)

tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv | find /I "unlockfps_clr.exe" >NUL && (
    echo [i] unlockfps_clr.exe - Closing...
    taskkill /F /IM "unlockfps_clr.exe"
) || (
    echo [✓] unlockfps_clr.exe - OK
)
echo.


echo 3/4 - Starting...
echo [i] Everything is ready! Please wait a moment ᕱ⑅︎ᕱ & echo.

set /p LMode=<"%AppData%\Genshin Stella Mod by Sefinek\launch-mode.sfn"
if %LMode% equ 1 (
    cd "data\unlocker"
    start "" "unlockfps_clr.exe"

    cd "..\reshade"
    start "" /wait "inject64.exe" "GenshinImpact.exe"

    echo.
    cd "..\cmd\start"
    call wait_for_unlockfps.cmd
) else if %LMode% equ 2 (
    echo Please start the game now.

    cd "data\reshade"
    start "" /wait "inject64.exe" "GenshinImpact.exe"

    echo.
    cd "..\cmd\start"
    call done.cmd
) else if %LMode% equ 3 (
    cd "data\unlocker"
    start "" "unlockfps_clr.exe"

    cd "..\cmd\start"
    call wait_for_unlockfps.cmd
) else (
    echo Error: Invalid launch mode: %LMode%
    pause
)


:pause
    echo.
    set /p 0=
    exit

:missing_perms
    echo.
    echo [x] Error: The script must be run as an administrator.
    pause
    exit