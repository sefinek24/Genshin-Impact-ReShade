@echo off
title Cache removal tool
setlocal EnableExtensions DisableDelayedExpansion
chcp 65001 > NUL

echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                              Genshin Impact Stella Mod 2023
echo.   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                     Delete cache files
echo.⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v7.0.1
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v5.8.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v2.0.8
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                          ~ Made by Sefinek
echo ========================================================================================= & echo.

echo 1/6 - Checking administrative permissions...
net session >nul 2>&1
if "%ERRORLEVEL%"=="0" (
    echo [✓] No problems found. & echo.
) else (
    echo [x] Error. This command must be run as administrator.
    goto pause
)
echo.


echo 2/6 - Checking required files and folders...
set "GamePathSFN=%AppData%\Genshin Stella Mod by Sefinek\game-path.sfn"
if exist "%GamePathSFN%" (
    echo [✓] %GamePathSFN%
) else (
    echo [x] Not found.
    goto nothing_to_do
)

set /p GamePath=<"%GamePathSFN%"
if exist "%GamePath%" (
    echo [✓] %GamePath%
) else (
    echo [x] Not found.
    goto nothing_to_do
)
echo.


echo 3/6 - Deleting temp files...
set "temp_dir=C:\Genshin-Impact-ReShade\temp"
echo [i] Path: %temp_dir%
if exist "%temp_dir%" (
    del /q "%temp_dir%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 4/6 - Deleting WebView2 cache...
set "webview_dir=%AppData%\Genshin Stella Mod by Sefinek\EBWebView"
echo [i] Path: %webview_dir%
if exist "%webview_dir%" (
    rd /s /q "%webview_dir%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 5/6 - Deleting ReShade log file...
set "reshadeLog_file=%GamePath%\Genshin Impact game\ReShade.log"
echo [i] Path: %reshadeLog_file%
if exist "%reshadeLog_file%" (
    del "%reshadeLog_file%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 6/6 - Deleting Stella Mod logs...
set "stellaModLogs_dir=%AppData%\Genshin Stella Mod by Sefinek\logs"
echo [i] Path: %stellaModLogs_dir%
if exist "%stellaModLogs_dir%" (
    rd /s /q "%stellaModLogs_dir%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo. && echo.


echo [i] Done! You can close this window.
goto nothing_to_do

:nothing_to_do
    set /p 0=
    goto nothing_to_do