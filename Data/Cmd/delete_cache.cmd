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
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.6.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.7
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/5 - Checking administrative permissions. Please wait...
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [✓] No problems found. & echo.
) else (
    echo [x] Error. This command must be run as administrator.
    goto nothing_to_do
)


echo 2/5 - Deleting %temp%\ReShade...
rmdir /s /q %temp%\ReShade
echo.

echo 3/5 - Deleting %AppData%\Genshin Impact MP by Sefinek\EBWebView...
rmdir /s /q "%AppData%\Genshin Impact MP by Sefinek\EBWebView"
echo.

set /p GamePath=<"%AppData%\Genshin Impact MP by Sefinek\game-path.sfn"
echo 4/5 - Deleting %GamePath%\ReShade.log...
del "%GamePath%\ReShade.log"
echo.

echo 5/5 - Deleting %AppData%\Genshin Impact MP by Sefinek\logs..
rmdir /s /q "%AppData%\Genshin Impact MP by Sefinek\logs"


echo.
echo [i] Success. You can close this window.

:nothing_to_do
set /p 0=
exit