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
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v%1
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v%2
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v%3
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                          ~ Made by Sefinek
echo ========================================================================================= & echo.

echo 1/6 - Checking administrative permissions...
net session >nul 2>&1
if "%ERRORLEVEL%"=="0" (
    echo [✓] No problems found. & echo.
) else (
    echo [x] This file needs to be executed with administrative privileges.
    goto done
)


echo 2/6 - Checking required files and folders...

if exist %4 (
    echo [✓] %4
) else (
    echo [x] Not found game-path.sfn.
    echo [x] %4
    goto done
)

set /p GamePath=<%4
if exist "%GamePath%" (
    echo [✓] "%GamePath%"
) else (
    echo [x] Not found main game folder.
    goto done
)
echo.



echo 3/6 - Deleting temp files...
echo [i] Path: %5
if exist %5 (
    pushd %5
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
echo [i] Path: %6
if exist %3 (
    rd /s /q %6
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 5/6 - Deleting ReShade log file...
echo [i] Path: %7
if exist %7 (
    del %7
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.


echo 6/6 - Deleting Stella Mod logs...
echo [i] Path: %8
if exist %8 (
    rd /s /q %8
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo. && echo.


echo [i] Done! Operation has been completed.
goto done

:done
    echo.
    pause
    exit
