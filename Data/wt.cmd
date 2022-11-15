@echo off
title Genshin Impact ReShade 2023 Mod Pack - Update
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                    Download Windows Terminal & echo.
call header.cmd

echo 1/2 - Downloading Windows Terminal...
winget install Microsoft.WindowsTerminal --accept-source-agreements --accept-package-agreements
if %ERRORLEVEL% EQU 0 (
    echo.
    echo 2/2 - Configuring...
) else (
    echo Error occurred.
)
echo.


set /p null=
exit