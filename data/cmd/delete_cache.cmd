@echo off
title Cache removal tool - Genshin Impact Mod Pack 2023
chcp 65001 > nul
echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                Cache and logs removal tool
echo.⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.   ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v6.0.1.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.8.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.8
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/6 - Checking administrative permissions...
net session >nul 2>&1
if %ErrorLevel% == 0 (
    echo [✓] No problems found.
) else (
    goto missing_perms
)
echo.


echo 2/6 - Checking required files and folders...
set "GamePathSFN=%AppData%\Genshin Stella Mod by Sefinek\game-path.sfn"
if exist "%GamePathSFN%" (
    echo [✓] %GamePathSFN%
) else (
    echo [x] File "%GamePathSFN%" was not found.
    goto nothing_to_do
)

set /p GamePath=<"%GamePathSFN%"
if exist "%GamePath%" (
    echo [✓] %GamePath%
) else (
    echo [x] Folder "%GamePath%" was not found.
    goto nothing_to_do
)
echo.


echo 3/6 - Deleting %temp%\ReShade...
if exist "%temp%\ReShade" (
    rd /s /q "%temp%\ReShade"
    echo [✓] Deleted ReShade cache.
) else (
    echo [x] Folder was not found.
)
echo.


echo 4/6 - Deleting %AppData%\Genshin Stella Mod by Sefinek\EBWebView...
if exist "%AppData%\Genshin Stella Mod by Sefinek\EBWebView" (
    rd /s /q "%AppData%\Genshin Stella Mod by Sefinek\EBWebView"
    echo [✓] Deleted folder.
) else (
    echo [x] Folder was not found.
)
echo.


echo 5/6 - Deleting %GamePath%\ReShade.log...
if exist "%GamePath%\ReShade.log" (
    del "%GamePath%\ReShade.log"
    echo [✓] Deleted file.
) else (
    echo [x] File was not found.
)
echo.


echo 6/6 - Deleting %AppData%\Genshin Stella Mod by Sefinek\logs...
if exist "%AppData%\Genshin Stella Mod by Sefinek\logs" (
    rd /s /q "%AppData%\Genshin Stella Mod by Sefinek\logs"
    echo [✓] Deleted folder.
) else (
    echo [x] Folder was not found.
)
echo. && echo.

echo [i] Done! You can close this window.
goto nothing_to_do

:nothing_to_do
    set /p 0=
    exit

:missing_perms
    echo.
    echo [x] Error: The script must be run as an administrator.
    pause
    exit