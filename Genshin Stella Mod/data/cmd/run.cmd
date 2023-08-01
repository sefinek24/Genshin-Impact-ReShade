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
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v%1
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v%2
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v%3
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                          ~ Made by Sefinek
echo ========================================================================================= & echo.

echo 1/4 - Preparing...

REM Variables
set "GameVersion=%6"


REM Check if the script is running with administrative permissions
net session >nul 2>&1
if not "%ERRORLEVEL%"=="0" (
    echo [x] This file needs to be executed with administrative privileges.
    echo [%DATE% %TIME%]: The file must be run as administrator. >> %7
    goto pause
)

REM Check if the files exist
if not exist "data\unlocker\unlockfps_clr.exe" (
    echo [x] Failed to start. File unlockfps_clr.exe was not found.
    goto pause
)
if not exist "data\reshade\ReShade64.dll" (
    echo [x] Failed to start. File ReShade64.dll was not found.
    goto pause
)
if not exist "data\reshade\inject64.exe" (
    echo [x] Failed to start. File inject64.exe was not found.
    goto pause
)

REM Get the launch mode from a file
if not "%4" equ "3" if not "%4" equ "4" if not "%4" equ "5" if not "%4" equ "6" (
    echo [x] Failed to start. Unknown launch mode: %4
    goto :pause
)

REM Get the game version from a file
if not "%GameVersion%" equ "1" if not "%GameVersion%" equ "2" (
    echo [x] Failed to start. Invalid game version: %GameVersion%
    goto pause
)

echo [✓] Everything looks okay ฅ^•ﻌ•^ฅ & echo.





REM Check for required processes
echo 3/4 - Checking required processes...

tasklist /fi "ImageName eq Genshin Stella Mod.exe" /fo csv | find /I "Genshin Stella Mod.exe" >NUL && (
    echo [%DATE% %TIME%]: Killing "Genshin Stella Mod.exe" process... >> %7
    taskkill /F /IM "Genshin Stella Mod.exe" >> %7

    echo [i] Genshin Stella Mod.exe - Closed
) || (
    echo [✓] Genshin Stella Mod.exe - OK
)
if "%GameVersion%" equ "1" (
    tasklist /fi "ImageName eq GenshinImpact.exe" /fo csv | find /I "GenshinImpact.exe" >NUL && (
        echo [%DATE% %TIME%]: Killing "GenshinImpact.exe" process... >> %7
        taskkill /F /IM "GenshinImpact.exe" >> %7

        echo [i] GenshinImpact.exe      - Closed
    ) || (
        echo [✓] GenshinImpact.exe      - OK
    )
) else if "%GameVersion%" equ "2" (
    tasklist /fi "ImageName eq YuanShen.exe" /fo csv | find /I "YuanShen.exe" >NUL && (
        echo [%DATE% %TIME%]: Killing "YuanShen.exe" process... >> %7
        taskkill /F /IM "YuanShen.exe" >> %7

        echo [i] YuanShen.exe           - Closed
    ) || (
        echo [✓] YuanShen.exe           - OK
    )
) else (
    echo [x] Something went wrong. Unknown game version ID.
    goto pause
)
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv | find /I "unlockfps_clr.exe" >NUL && (
    echo [%DATE% %TIME%]: Killing "unlockfps_clr.exe" process... >> %7
    taskkill /F /IM "unlockfps_clr.exe" >> %7

    echo [i] unlockfps_clr.exe      - Closed
) || (
    echo [✓] unlockfps_clr.exe      - OK
)
tasklist /fi "ImageName eq inject64.exe" /fo csv | find /I "inject64.exe" >NUL && (
    echo [%DATE% %TIME%]: Killing "inject64.exe" process... >> %7
    taskkill /F /IM "inject64.exe" >> %7

    echo [i] inject64.exe           - Closed
) || (
    echo [✓] inject64.exe           - OK
)
echo.





REM Start the game
echo 4/4 - Starting...
echo [%DATE% %TIME%]: Launch mode %1. Starting... >> %7


REM Choose the correct mode
if "%4" equ "6" (
    REM Unlock FPS
    pushd "%9"
    start "" "unlockfps_clr.exe"

    echo [✓] Everything is ready! Now run the game using an FPS unlocking program ᕱ⑅︎ᕱ & echo.

    REM Inject ReShade
    pushd "..\reshade"
    call "..\cmd\start\run-reshade.cmd"

    REM Wait for the unlocker to finish
    pushd "%8"
    pushd "data\cmd\start"
    call "wait_for_unlockfps.cmd"
) else if "%4" equ "3" (
    echo [✓] Everything is ready! Thank you for using Stella Mod. Have fun. ᕱ⑅︎ᕱ & echo.
    REM Ask the user to start the game
    echo [i] ~~ Please start the game now. ~~

    REM Inject ReShade
    pushd "data\reshade"
    call "..\cmd\start\run-reshade.cmd"

    REM Notify the user that the injection is done
    echo.
    pushd "..\cmd\start"

    call done.cmd
)  else if "%4" equ "4" (
    REM Unlock FPS
    pushd "%9"
    start "" "unlockfps_clr.exe"

    echo [✓] Good job! Now run the game using an FPS unlocking program ᕱ⑅︎ᕱ & echo.

    REM Wait for the unlocker to finish
    pushd "..\cmd\start"
    call wait_for_unlockfps.cmd
) else (
    REM Wrong launch mode
    echo [x] Failed to start. Invalid launch mode: %4
    echo [%DATE% %TIME%]: Failed to start. Launch mode %4 is invalid. >> %7
    goto pause
)





REM Restore the original directory
popd

REM Pause the batch file
:pause
    echo.
    set /p 0=
    goto pause
