@echo off
title Cache removal tool
setlocal EnableExtensions DisableDelayedExpansion
chcp 65001 > NUL

echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                              Genshin Impact Stella Mod 2024
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

set /p "response=» Are you sure? [Yes/no]: "
if /i "%response%"=="yes" (
    echo You chose "Yes". Deleting log files and caches in progress...
) else (
    echo You chose "No" or provided an invalid response. Paused.
    goto done
)
echo.



echo 2/6 - Checking required paths...
set "GamePath=%~6"
if exist "%GamePath%" (
    echo [✓] %GamePath%
) else (
    echo [x] Not found main game folder: %GamePath%
    goto done
)
echo.



echo 3/6 - Deleting temp files...
set "ReShadeCache=%~5\ReShade\Cache"
echo [i] Path: %ReShadeCache%
if exist "%ReShadeCache%" (
    pushd "%ReShadeCache%"
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
set "WebViewCache=%~7\EBWebView"
echo [i] Path: %WebViewCache%
if exist "%WebViewCache%" (
    rd /s /q "%WebViewCache%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.



echo 5/6 - Deleting ReShade log file...
set "ReShadeLog=%~6\ReShade.log"
if exist "%ReShadeLog%" (
    del "%ReShadeLog%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)
echo.



echo 6/6 - Deleting Stella Mod logs...
set "StellaModLogs=%~7\logs"
echo [i] Path: %StellaModLogs%
if exist "%StellaModLogs%" (
    rd /s /q "%StellaModLogs%"
    echo [✓] Success.
) else (
    echo [x] Not found.
)

echo.
echo.



echo [i] Done! Operation has been completed.
goto done

:done
    echo.
    set /p null=
    exit
