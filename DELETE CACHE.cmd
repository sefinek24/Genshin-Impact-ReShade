@echo off
title Genshin Impact ReShade 2023 Mod Pack - Cache removal tool
chcp 65001 > nul

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                  Cache removal tool for ReShade & echo.

if exist "C:\genshin-impact-reshade-2023\Data\header.cmd" (
    call C:\genshin-impact-reshade-2023\Data\header.cmd
) else (
    call Data\header.cmd
)

echo [i] Administrative permissions are required. Please wait...

net session >nul 2>&1
if %errorLevel% == 0 (
    echo [✓] Ok. No problems found. & echo.
) else (
    echo [x] Error. Run this file as administrator.
    goto nothing_to_do
)


echo [i] ReShade cache path: %temp%\ReShade

if exist "%temp%\ReShade" (
    goto question
) else (
    echo [x] Cache folder for ReShade does not exist.
    goto nothing_to_do
)


:question
    set /p deleteFolder="[i] Are you sure? (y/n): "
    if "%deleteFolder%" == "y" (
        goto delete
    ) else if "%deleteFolder%" == "n" (
        goto no
    ) else (
        goto bad_answer
    )

:delete
    echo [i] Please wait...
    rmdir /s /q %temp%\ReShade
    echo [✓] Done.

    if exist "C:\Program Files\Genshin Impact\Genshin Impact game\ReShade.log" (
        goto log_file
    )

    goto nothing_to_do

:log_file
    echo.
    echo [i] Found: C:\Program Files\Genshin Impact\Genshin Impact game\ReShade.log
    set /p deleteLogFile="[i] Delete log file? (y/n): "

    if "%deleteLogFile%" == "y" (
        echo [i] Please wait...
        del "C:\Program Files\Genshin Impact\Genshin Impact game\ReShade.log"
        echo [✓] Done.

        goto nothing_to_do
    ) else if "%deleteLogFile%" == "n" (
        goto no
    ) else (
        goto bad_answer
    )

:no
    echo.
    echo [i] Bruh. Okay, no problem.
    goto nothing_to_do

:bad_answer
    echo.
    echo [x] Wrong answer. Expected: 'y' or 'n'. Close this window and please try again.
    echo [i] y = yes
    echo [i] n = no

:nothing_to_do
    echo.
    echo [i] You can close this window.
    set /p null=
    exit