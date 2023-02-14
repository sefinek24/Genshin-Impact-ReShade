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
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v6.0.0.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.6.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.8
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.


echo 1/3 - Checking required processes...
tasklist /fi "ImageName eq Genshin Impact Mod Pack.exe" /fo csv 2>NUL | find /I "Genshin Impact Mod Pack.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [i] Genshin Impact Mod Pack.exe - Closing...
    taskkill /F /IM "Genshin Impact Mod Pack.exe"
) else (
    echo [✓] Genshin Impact Mod Pack.exe - OK
)

tasklist /fi "ImageName eq GenshinImpact.exe" /fo csv 2>NUL | find /I "GenshinImpact.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [i] GenshinImpact.exe - Closing...
    taskkill /F /IM "GenshinImpact.exe"
) else (
    echo [✓] GenshinImpact.exe - OK
)

tasklist /fi "ImageName eq inject64.exe" /fo csv 2>NUL | find /I "inject64.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [i] inject64.exe - Closing...
    taskkill /F /IM "inject64.exe"
) else (
    echo [✓] inject64.exe - OK
)

tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [i] unlockfps_clr.exe - Closing...
    taskkill /F /IM "unlockfps_clr.exe"
) else (
    echo [✓] unlockfps_clr.exe - OK
)
echo.



echo 2/3 - Starting...
echo [i] Everything is ready! Please wait a moment ᕱ⑅︎ᕱ & echo.

set /p LMode=<"%AppData%\Genshin Impact MP by Sefinek\launch-mode.sfn"
if %LMode% == 1 (
	cd "Data\Unlocker"
	start .\unlockfps_clr.exe
	
    cd "..\ReShade"
	"inject64.exe" "GenshinImpact.exe"

	echo.
	cd ..\Cmd\start
    call wait_for_unlockfps.cmd
) else if %LMode% == 2 (
    echo Please start the game now.

    cd "Data\ReShade"
    "inject64.exe" "GenshinImpact.exe"

    echo.
    cd ..\Cmd\start
    call done.cmd
) else if %LMode% == 3 (
    cd "Data\Unlocker"
    start .\unlockfps_clr.exe

    cd ..\Cmd\start
    call wait_for_unlockfps.cmd
) else (
    goto error
)


:pause
    echo.
    set /p null=
    exit
