@echo off
title Start game - Genshin Impact Mod Pack 2023
chcp 65001 > NUL
echo.⠀  ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀ ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀  ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.  ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                Made by Sefinek - Start game
echo.⠀  ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.   ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀  ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀  ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v1.2.0 [SV_15112022_120-003]
echo.   ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.4.2
echo.  ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.1
echo. ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.  ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/8 - Checking game localization...
if exist "C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe" (
    echo [✓] Found: C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe
) else (
    echo [x] Not found. Where is the game installed?
)
echo.

echo 2/8 - Checking if git is installed...
if exist "C:\Program Files\Git\cmd\git.exe" (
    git -v
    echo.

    echo 3/8 - Checking for new updates...
    git fetch
    git pull










) else (
    echo [x] NOT INSTALLED! I CAN'T CHECK FOR NEW UPDATES.
    echo [i] PLEASE DOWNLOAD: https://git-scm.com/downloads & echo.

    echo 3/8 - Checking for new updates...
    echo [x] Canceled.
)
echo.





echo 4/8 - Checking required processes...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [x] The application "unlockfps_clr.exe" [Genshin FPS Unlocker] is already running.
    echo [i] Close it and try again.

    goto close-window
) else (
    echo [✓] unlockfps_clr.exe - OK
)
tasklist /fi "ImageName eq Genshin Impact Mod Pack.exe" /fo csv 2>NUL | find /I "Genshin Impact Mod Pack.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [i] Closing Genshin Impact Mod Pack.exe...
    taskkill /IM "Genshin Impact Mod Pack.exe"
)
echo [✓] Genshin Impact Mod Pack.exe - OK
echo.


echo 5/8 - Setting execution policy...
powershell.exe Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
echo [✓] Done. Everything is ready (=^･ω･^=)! & echo.


echo 6/8 - Starting...
set /p RunID=< "%AppData%\Genshin Impact MP by Sefinek\run"
if %RunID% == 1 (
    echo [i] Click 'Yes' in the two UAC (User Account Control) notifications. File inject.exe and unlockfps_clr.exe. & echo.
    powershell.exe Data\PowerShell\mod.ps1
    call Data\Cmd\start\1.cmd
) else if %RunID% == 2 (
    echo [i] Click 'Yes' in the UAC (User Account Control) notification. File inject.exe. & echo.
    powershell.exe Data\PowerShell\only-reshade.ps1
    call Data\Cmd\start\2.cmd
) else if %RunID% == 3 (
    echo [i] Click 'Yes' in the UAC (User Account Control) notification. File unlockfps_clr.exe. & echo.
    powershell.exe Data\PowerShell\only-fps_unlocker.ps1
    call Data\Cmd\start\3.cmd
) else (
    goto error
)

:close-window
    echo.
    timeout /t 16 /nobreak
    exit