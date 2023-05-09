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
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v7.2.1
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v5.8.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v2.0.8
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                          ~ Made by Sefinek
echo ========================================================================================= & echo.

set "OutputLog=%AppData%\Genshin Stella Mod by Sefinek\logs\cmd.output.log"


REM Check if the script is running with administrative permissions
echo 1/4 - Preparing...
net session >nul 2>&1
if not "%ERRORLEVEL%"=="0" (
    echo [x] This file needs to be executed with administrative privileges.
    echo [%DATE% %TIME%]: The file must be run as administrator. >> "%OutputLog%"
    goto pause
)

REM Check if the files exist
set "LaunchModePath=%AppData%\Genshin Stella Mod by Sefinek\launch-mode.sfn"
if not exist "%LaunchModePath%" (
    echo [x] Failed to start. The file launch-mode.sfn was not found in launcher appdata.
    echo [%DATE% %TIME%]: File was not found in: %AppData% >> "%OutputLog%"
    goto pause
)

set "GameVersionPath=%AppData%\Genshin Stella Mod by Sefinek\game-version.sfn"
if not exist "%GameVersionPath%" (
    call "data\vbs\chooseGameVersion.vbs"
)
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
set /p LaunchModeContent=<"%AppData%\Genshin Stella Mod by Sefinek\launch-mode.sfn"
if not "%LaunchModeContent%" equ "1" if not "%LaunchModeContent%" equ "2" if not "%LaunchModeContent%" equ "3" (
    echo [x] Failed to start. Unknown launch mode: %LaunchModeContent%
    goto pause
)

REM Get the game version from a file
set /p GameVersionContent=<"%AppData%\Genshin Stella Mod by Sefinek\game-version.sfn"
if not "%GameVersionContent%" equ "1" if not "%GameVersionContent%" equ "2" (
    echo [x] Failed to start. Invalid game version: %GameVersionContent%
    goto pause
)

echo [✓] Everything looks okay ฅ^•ﻌ•^ฅ & echo.





REM Check for required processes
echo 3/4 - Checking required processes...

tasklist /fi "ImageName eq launcher.exe" /fo csv | find /I "launcher.exe" >NUL && (
    echo [%DATE% %TIME%]: Killing "launcher.exe" process... >> "%OutputLog%"
    taskkill /F /IM "Ylauncher.exe" >> "%OutputLog%"

    echo [i] launcher.exe           - Closed
) || (
    echo [✓] launcher.exe           - OK
)
tasklist /fi "ImageName eq Genshin Stella Mod.exe" /fo csv | find /I "Genshin Stella Mod.exe" >NUL && (
    echo [%DATE% %TIME%]: Killing "Genshin Stella Mod.exe" process... >> "%OutputLog%"
    taskkill /F /IM "Genshin Stella Mod.exe" >> "%OutputLog%"

    echo [i] Genshin Stella Mod.exe - Closed
) || (
    echo [✓] Genshin Stella Mod.exe - OK
)
if "%GameVersionContent%" equ "1" (
    tasklist /fi "ImageName eq GenshinImpact.exe" /fo csv | find /I "GenshinImpact.exe" >NUL && (
        echo [%DATE% %TIME%]: Killing "GenshinImpact.exe" process... >> "%OutputLog%"
        taskkill /F /IM "GenshinImpact.exe" >> "%OutputLog%"

        echo [i] GenshinImpact.exe      - Closed
    ) || (
        echo [✓] GenshinImpact.exe      - OK
    )
) else if "%GameVersionContent%" equ "2" (
    tasklist /fi "ImageName eq YuanShen.exe" /fo csv | find /I "YuanShen.exe" >NUL && (
        echo [%DATE% %TIME%]: Killing "YuanShen.exe" process... >> "%OutputLog%"
        taskkill /F /IM "YuanShen.exe" >> "%OutputLog%"

        echo [i] YuanShen.exe           - Closed
    ) || (
        echo [✓] YuanShen.exe           - OK
    )
) else (
    echo [x] Something went wrong. Unknown game version ID.
    goto pause
)
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv | find /I "unlockfps_clr.exe" >NUL && (
    echo [%DATE% %TIME%]: Killing "unlockfps_clr.exe" process... >> "%OutputLog%"
    taskkill /F /IM "unlockfps_clr.exe" >> "%OutputLog%"

    echo [i] unlockfps_clr.exe      - Closed
) || (
    echo [✓] unlockfps_clr.exe      - OK
)
tasklist /fi "ImageName eq inject64.exe" /fo csv | find /I "inject64.exe" >NUL && (
    echo [%DATE% %TIME%]: Killing "inject64.exe" process... >> "%OutputLog%"
    taskkill /F /IM "inject64.exe" >> "%OutputLog%"

    echo [i] inject64.exe           - Closed
) || (
    echo [✓] inject64.exe           - OK
)
echo.





REM Start the game
echo 4/4 - Starting...
echo [%DATE% %TIME%]: Launch mode %LaunchModeContent%. Starting... >> "%OutputLog%"

REM Choose the correct mode based on the LaunchModeContent variable
if "%LaunchModeContent%" equ "1" (
    REM Unlock FPS
    pushd "data\unlocker"
    start "" "unlockfps_clr.exe"

    echo [✓] Everything is ready! Now run the game using an FPS unlocking program ᕱ⑅︎ᕱ & echo.

    REM Inject ReShade
    pushd "..\reshade"
    call "..\cmd\start\run-reshade.cmd"

    REM Wait for the unlocker to finish
    echo.
    pushd "..\cmd\start"
    call wait_for_unlockfps.cmd
) else if "%LaunchModeContent%" equ "2" (
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
) else if "%LaunchModeContent%" equ "3" (
    REM Unlock FPS
    pushd "data\unlocker"
    start "" "unlockfps_clr.exe"

    echo [✓] Good job! Now run the game using an FPS unlocking program ᕱ⑅︎ᕱ & echo.

    REM Wait for the unlocker to finish
    pushd "..\cmd\start"
    call wait_for_unlockfps.cmd
) else (
    REM Wrong launch mode
    echo [x] Failed to start. Invalid launch mode: %LaunchModeContent%
    echo [%DATE% %TIME%]: Failed to start. Launch mode is invalid. >> "%OutputLog%"
    goto pause
)





REM Restore the original directory
popd

REM Pause the batch file
:pause
    echo.
    set /p 0=
    goto pause