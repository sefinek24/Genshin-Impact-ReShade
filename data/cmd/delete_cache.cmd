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
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v7.1.0
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
    echo [x] Error: This file needs to be executed with administrative privileges.
    goto pause
)


echo 2/6 - Checking required files and folders...
set "GamePathSFN=%AppData%\Genshin Stella Mod by Sefinek\game-path.sfn"
if exist "%GamePathSFN%" (
    echo [✓] %GamePathSFN%
) else (
    echo [x] Not found.
    goto pause
)

set /p GamePath=<"%GamePathSFN%"
if exist "%GamePath%" (
    echo [✓] %GamePath%
) else (
    echo [x] Not found.
    goto pause
)
echo.


echo 3/6 - Deleting temp files...
set "TempDir=C:\Genshin-Impact-ReShade\data\reshade\cache"
echo [i] Path: %TempDir%
if exist "%TempDir%" (
    pushd "%TempDir%"
    for %%I in (*) do (
      if not "%%~nxI"=="null" (
        del /f /q "%%I"
      )
    )
    popd

    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 4/6 - Deleting WebView2 cache...
set "WebviewDir=%AppData%\Genshin Stella Mod by Sefinek\EBWebView"
echo [i] Path: %WebviewDir%
if exist "%WebviewDir%" (
    rd /s /q "%WebviewDir%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 5/6 - Deleting ReShade log file...
set "ReShadeLogFile=%GamePath%\Genshin Impact game\ReShade.log"
echo [i] Path: %ReShadeLogFile%
if exist "%ReShadeLogFile%" (
    del "%ReShadeLogFile%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 6/6 - Deleting Stella Mod logs...
set "StellaModLogsDir=%AppData%\Genshin Stella Mod by Sefinek\logs"
echo [i] Path: %StellaModLogsDir%
if exist "%StellaModLogsDir%" (
    rd /s /q "%StellaModLogsDir%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo. && echo.


echo [i] Done! You can close this window.
goto pause

:pause
    set /p 0=
    goto pause