@echo off
title Start game
setlocal EnableExtensions DisableDelayedExpansion
chcp 65001 > NUL

echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                              Genshin Impact Stella Mod 2023
echo.   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                       Start the game
echo.⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v6.1.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v5.8.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v2.0.8
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                          ~ Made by Sefinek
echo ========================================================================================= & echo.


REM Check if the script is running with administrative permissions
echo 1/4 - Checking administrative permissions...
net session >nul 2>&1
if %ErrorLevel% == 0 (
    echo [✓] No problems found. & echo.
) else (
    echo [x] Error. This command must be run as administrator.
    goto pause
)

set "outputFile=%AppData%\Genshin Stella Mod by Sefinek\logs\cmd_output.log"

REM Check for required processes
echo 2/4 - Checking required processes...
tasklist /fi "ImageName eq Genshin Stella Mod Launcher.exe" /fo csv | find /I "Genshin Stella Mod Launcher.exe" >NUL && (
    echo [i] Genshin Stella Mod Launcher.exe - Closing...

    echo [%DATE% %TIME%]: Killing "Genshin Stella Mod Launcher.exe" process... >> "%outputFile%"
    taskkill /F /IM "Genshin Stella Mod Launcher.exe" >> "%outputFile%"
) || (
    echo [✓] Genshin Stella Mod Launcher.exe - OK
)

tasklist /fi "ImageName eq GenshinImpact.exe" /fo csv | find /I "GenshinImpact.exe" >NUL && (
    echo [i] GenshinImpact.exe - Closing...

    echo [%DATE% %TIME%]: Killing "GenshinImpact.exe" process... >> "%outputFile%"
    taskkill /F /IM "GenshinImpact.exe" >> "%outputFile%"
) || (
    echo [✓] GenshinImpact.exe - OK
)

tasklist /fi "ImageName eq inject64.exe" /fo csv | find /I "inject64.exe" >NUL && (
    echo [i] inject64.exe - Closing...

    echo [%DATE% %TIME%]: Killing "inject64.exe" process... >> "%outputFile%"
    taskkill /F /IM "inject64.exe" >> "%outputFile%"
) || (
    echo [✓] inject64.exe - OK
)

tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv | find /I "unlockfps_clr.exe" >NUL && (
    echo [i] unlockfps_clr.exe - Closing...

    echo [%DATE% %TIME%]: Killing "unlockfps_clr.exe" process... >> "%outputFile%"
    taskkill /F /IM "unlockfps_clr.exe" >> "%outputFile%"
) || (
    echo [✓] unlockfps_clr.exe - OK
)
echo.


REM Start the game
echo 3/4 - Starting...
echo [i] Everything is ready! Please wait a moment ᕱ⑅︎ᕱ & echo.

REM Check the launch mode and start the appropriate files
if not exist "%AppData%\Genshin Stella Mod by Sefinek\launch-mode.sfn" (
    echo [x] Failed to start. The file launch-mode.sfn was not found in launcher appdata.
    goto pause
)

setlocal EnableDelayedExpansion

REM Get the launch mode from a file
set /p LaunchMode=<"%AppData%\Genshin Stella Mod by Sefinek\launch-mode.sfn"

REM Choose the correct mode based on the LaunchMode variable
if "%LaunchMode%" equ "1" (
    REM Unlock FPS
    pushd "data\unlocker"
    start "" "unlockfps_clr.exe"

    REM Inject ReShade
    pushd "..\reshade"
    call "inject64.exe" "GenshinImpact.exe"

    REM Wait for the unlocker to finish
    echo.
    pushd "..\cmd\start"
    call wait_for_unlockfps.cmd
) else if "%LaunchMode%" equ "2" (
    REM Ask the user to start the game
    echo ~~ Please start the game now. ~~

    REM Inject ReShade
    pushd "data\reshade"
    call "inject64.exe" "GenshinImpact.exe"

    REM Notify the user that the injection is done
    echo.
    pushd "..\cmd\start"
    call done.cmd
) else if "%LaunchMode%" equ "3" (
    REM Unlock FPS
    pushd "data\unlocker"
    start "" "unlockfps_clr.exe"

    REM Wait for the unlocker to finish
    pushd "..\cmd\start"
    call wait_for_unlockfps.cmd
) else (
    REM Error: Invalid launch mode
    echo [x] Failed to start. Invalid launch mode: %LaunchMode%
    goto pause
)

REM Restore the original directory
popd

REM Exit the batch file
:pause
    echo.
    set /p 0=
    exit