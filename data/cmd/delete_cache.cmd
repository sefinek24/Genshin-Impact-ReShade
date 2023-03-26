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
echo.   ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v6.0.0.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.7.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.8
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/6 - Checking administrative permissions...
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [✓] No problems found.
) else (
    goto :missing_perms
)
echo.


echo 2/6 - Checking required files and folders...
set GamePathSFN="%AppData%\Genshin Stella Mod by Sefinek\game-path.sfn"
if not exist %GamePathSFN% (
    echo [!] File %GamePathSFN% was not found.
    goto nothing_to_do
) else (
    echo [✓] %GamePathSFN%
)

set /p GamePath=<%GamePathSFN%
if not exist %GamePath% (
    echo [!] Folder %GamePath% was not found.
    goto nothing_to_do
) else (
    echo [✓] %GamePath%
)
echo.


echo 3/6 - Deleting %temp%\ReShade...
if exist %temp%\ReShade (
    rmdir /s /q %temp%\ReShade
    echo [i] Deleted %temp%\ReShade
) else (
    echo [!] Folder not found: %temp%\ReShade
)
echo.


echo 4/6 - Deleting %AppData%\Genshin Stella Mod by Sefinek\EBWebView...
if exist "%AppData%\Genshin Stella Mod by Sefinek\EBWebView" (
    rmdir /s /q "%AppData%\Genshin Stella Mod by Sefinek\EBWebView"
    echo [i] Deleted "%AppData%\Genshin Stella Mod by Sefinek\EBWebView"
) else (
    echo [!] Folder not found: "%AppData%\Genshin Stella Mod by Sefinek\EBWebView"
)
echo.


echo 5/6 - Deleting %GamePath%\ReShade.log...
if exist "%GamePath%\ReShade.log" (
    del "%GamePath%\ReShade.log"
    echo [i] Deleted "%GamePath%\ReShade.log"
) else (
    echo [!] File not found: "%GamePath%\ReShade.log"
)
echo.


echo 6/6 - Deleting %AppData%\Genshin Stella Mod by Sefinek\logs..
if exist "%AppData%\Genshin Stella Mod by Sefinek\logs" (
    rmdir /s /q "%AppData%\Genshin Stella Mod by Sefinek\logs"
    echo [i] Deleted "%AppData%\Genshin Stella Mod by Sefinek\logs"
) else (
    echo [!] Folder not found: "%AppData%\Genshin Stella Mod by Sefinek\logs"
)
echo.


echo. && echo [✓] Success. You can close this window.
pause
exit

:nothing_to_do
    set /p 0=
    exit

:missing_perms
    echo.
    echo [!] Error: The script must be run as an administrator.
    pause
    exit