@echo off
title Genshin Impact ReShade 2023 Mod Pack - Update
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                    Download Windows Terminal & echo.
call Data\header.cmd

echo 1/2 - Downloading Windows Terminal...
winget install Microsoft.WindowsTerminal --accept-source-agreements --accept-package-agreements
if %ERRORLEVEL% EQU 0 (
    echo.
    echo 2/2 - Configuring...
    echo 1 > Data\configure
    echo Done.
) else (
    echo Error occurred.
    echo 0 > Data\configure
)

timeout /t 6 /nobreak
exit