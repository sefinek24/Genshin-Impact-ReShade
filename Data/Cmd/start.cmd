@echo off
title Start game - Genshin Impact Mod Pack 2023
chcp 65001 > NUL
echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                Made by Sefinek - Start game
echo.⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v4.0.0.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.6.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.6
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.


echo 1/3 - Checking required processes...
tasklist /fi "ImageName eq Genshin Impact Mod Pack.exe" /fo csv 2>NUL | find /I "Genshin Impact Mod Pack.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [i] Closing Genshin Impact Mod Pack.exe...
    taskkill /IM "Genshin Impact Mod Pack.exe"
)
echo [✓] Genshin Impact Mod Pack.exe - OK
tasklist /fi "ImageName eq inject.exe" /fo csv 2>NUL | find /I "inject.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [x] The application "inject.exe" [injector for ReShade] is already running. Close it and try again.

    goto pause
) else (
    echo [✓] inject.exe - OK
)
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [x] The application "unlockfps_clr.exe" [Genshin FPS Unlocker] is already running. Close it and try again.

    goto pause
) else (
    echo [✓] unlockfps_clr.exe - OK
)
echo.



echo 2/3 - Starting...
echo [i] Everything is ready! Please wait a moment (=^･ω･^=) & echo.

set /p RunID=<"%AppData%\Genshin Impact MP by Sefinek\launch-mode.sfn"
if %RunID% == 1 (
    powershell.exe Cd "Data\ReShade"; Start-Process -FilePath "inject.exe" "GenshinImpact.exe"; Cd "\"..\Unlocker\""; Start-Process -FilePath ".\unlockfps_clr.exe"

    call Data\Cmd\start\1.cmd
) else if %RunID% == 2 (
    powershell.exe Cd "Data\ReShade"; Start-Process -FilePath "inject.exe" "GenshinImpact.exe"

    call Data\Cmd\start\2.cmd
) else if %RunID% == 3 (
    powershell.exe Cd "\"Data\Unlocker\""; Start-Process -FilePath ".\unlockfps_clr.exe"

    call Data\Cmd\start\3.cmd
) else (
    goto error
)


:pause
    echo.
    set /p null=
    exit
