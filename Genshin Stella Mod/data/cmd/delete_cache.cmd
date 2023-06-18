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
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v7.6.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v5.8.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v2.0.11
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                          ~ Made by Sefinek
echo ========================================================================================= & echo.

echo 1/6 - Checking administrative permissions...
net session >nul 2>&1
if "%ERRORLEVEL%"=="0" (
    echo [✓] No problems found. & echo.
) else (
    echo [x] This file needs to be executed with administrative privileges.
    goto pause
)


echo 2/6 - Checking required files and folders...

if exist %1 (
    echo [✓] %1
) else (
    echo [x] Not found game-path.sfn.
    goto pause
)

set /p GamePath=<%1
if exist "%GamePath%" (
    echo [✓] "%GamePath%"
) else (
    echo [x] Not found main game folder.
    goto pause
)
echo.



echo 3/6 - Deleting temp files...
echo [i] Path: %2
if exist %2 (
    pushd %2
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
echo [i] Path: %3
if exist %3 (
    rd /s /q %3
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 5/6 - Deleting ReShade log file...
echo [i] Path: %4
if exist %4 (
    del %4
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 6/6 - Deleting Stella Mod logs...
echo [i] Path: %5
if exist %5 (
    rd /s /q %5
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
