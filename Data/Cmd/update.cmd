@echo off
title Update tool - Genshin Impact Mod Pack 2023
chcp 65001 > NUL
echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                        Update tool
echo.⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo.  ⠀  ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.   ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v4.1.0.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.6.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.7
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.


echo 1/4 - Downloading new setup from cdn.sefinek.net...
cd "%temp%"
if exist "Genshin Impact Mod Pack Setup.exe" del "Genshin Impact Mod Pack Setup.exe"
curl -o "Genshin Impact Mod Pack Setup.exe" https://cdn.sefinek.net/resources/genshin-impact-reshade/launcher/download.exe
echo.


echo 2/4 - Killing required processes...
taskkill /F /IM "Genshin Impact Mod Pack.exe"
taskkill /F /IM "inject.exe"
taskkill /F /IM "unlockfps_clr.exe"
echo.


echo 3/4 - Installing...
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set date=%%c-%%a-%%b)
for /f "tokens=1-2 delims=/:" %%a in ("%TIME%") do (set time=%%a%%b)

if not exist "%AppData%\Genshin Impact MP by Sefinek\logs" mkdir "%AppData%\Genshin Impact MP by Sefinek\logs"
if not exist "%AppData%\Genshin Impact MP by Sefinek\logs\updates" mkdir "%AppData%\Genshin Impact MP by Sefinek\logs\updates"

"%temp%\Genshin Impact Mod Pack Setup.exe" /SILENT /NORESTART /LOG="%AppData%\Genshin Impact MP by Sefinek\logs\updates\%time%_%date%.log"
echo Done. & echo.


echo 4/4 - Finishing...
del "Genshin Impact Mod Pack Setup.exe"
echo Done. & echo.

echo Nice! This process has been completed. You can close this window.
cd C:\Genshin-Impact-ReShade
"Genshin Impact Mod Pack.exe"

timeout /t 9 /nobreak
exit