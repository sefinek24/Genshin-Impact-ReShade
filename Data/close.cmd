@echo off
title Genshin Impact ReShade 2023 Mod Pack - Close processes
chcp 65001 > NUL

echo [i] Closing unlockfps_clr.exe...
taskkill /f /im unlockfps_clr.exe
echo [i] Closing inject.exe...
taskkill /f /im inject.exe
echo [✓] Done.
echo.

set /p null="Press ENTER to close this window..."
exit