@echo off
title Delete WebView2 cache - Genshin Impact Mod Pack 2023
chcp 65001 > nul
echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                  Delete WebView2 cache
echo.⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.   ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v3.0.0 [SV_DEV_04122022_300X-001]
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.5.1
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.6
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/2 - Administrative permissions are required. Please wait...
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [✓] No problems found. & echo.
) else (
    echo [x] Error. This command must be run as administrator.
    goto nothing_to_do
)


echo 2/2 - Deleting %AppData%\Genshin Impact MP by Sefinek\EBWebView...
rmdir /s /q "%AppData%\Genshin Impact MP by Sefinek\EBWebView"


echo.
echo [i] Success. You can close this window.

:nothing_to_do
set /p 0=
exit