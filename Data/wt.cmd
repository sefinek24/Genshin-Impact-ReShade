@echo off
title Download Windows Terminal - Genshin Impact Mod Pack 2023
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                    Download Windows Terminal
echo ================================================================ & echo.

echo 1/2 - Downloading Windows Terminal...
winget install Microsoft.WindowsTerminal --accept-source-agreements --accept-package-agreements
if %ERRORLEVEL% EQU 0 (
    echo.
    echo 2/2 - Configuring...
    echo true> "%AppData%\Genshin Impact MP by Sefinek\configured"

    echo Done. You can now go to the application. Enjoy!
) else (
    echo.
    echo Error occurred. Please try again or contact with me on Discord.
    echo false> "%AppData%\Genshin Impact MP by Sefinek\configured"
)

timeout /t 99 /nobreak
exit