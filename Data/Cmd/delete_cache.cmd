@echo off
title Cache removal tool - Genshin Impact Mod Pack 2023
chcp 65001 > nul
echo.⠀  ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀ ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀  ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.  ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                Cache and logs removal tool
echo.⠀  ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.   ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀  ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀  ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v3.0.0 [SV_21112022_300N-001]
echo.   ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.5.1
echo.  ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.4
echo. ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.  ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/5 - Administrative permissions are required. Please wait...

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

echo 3/5 - Deleting %ProgramFiles%\Genshin Impact\Genshin Impact game\ReShade.log...
rmdir /s /q "%ProgramFiles%\Genshin Impact\Genshin Impact game\ReShade.log"
echo.

echo 4/5 - Deleting %AppData%\Genshin Impact MP by Sefinek\EBWebView...
rmdir /s /q "%AppData%\Genshin Impact MP by Sefinek\EBWebView"
echo.

echo 5/5 - Deleting %AppData%\Genshin Impact MP by Sefinek\error.log...
rmdir /s /q "%AppData%\Genshin Impact MP by Sefinek\error.log"
echo.

echo [i] You can close this window.
set /p 0=
exit