@echo off
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                   Made by Sefinek - Start game & echo.
echo.⠀⠀  ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀⠀  ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿
echo.⠀  ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿
echo.⠀⠀  ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.⠀⠀  ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo.⠀⠀⠀  ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo.⠀⠀⠀  ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.⠀  ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆
echo.⠀⠀  ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆
echo.⠀  ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.⠀  ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ================================================================ & echo.
echo * Downloaded mod version:  v1.1.1 [BV_09112022_111-002]
echo * ReShade version:         v5.4.2
echo * FPS Unlocker version:    v2.0.0 & echo.
echo ================================================================ & echo.

echo 1/5 - Checking if git is installed...
if exist "C:\Program Files\Git\cmd\git.exe" (
    git -v
    echo.

    echo 2/5 - Checking for new updates...
    git fetch
    git pull
) else (
    echo [x] Not installed! I can't check for new updates.
    echo [i] Please download: https://git-scm.com/downloads
)
echo.

echo 3/5 - Checking game localization...
if exist "C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe" (
    echo [✓] Found.
    echo [i] Patch: C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe
) else (
    echo [x] Not found. Where is the game installed?
)
echo.

echo 4/5 - Checking required processes...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [x] The application "unlockfps_clr.exe" [Genshin FPS Unlocker] is already running. & echo.

    pause && exit
) else (
    echo [✓] Ok. Everything is ready to run FPS Unlocker!
)
echo.

echo 5/5 - Starting...
echo [i] Click "Yes" in the two "User Account Control" (UAC) notifications. File inject.exe and unlockfps_clr.exe. & echo.
powershell.exe -file "ReShade.ps1"

tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] Done! If you need help, please create a new Issue on GitHub. Thank you.
) else (
    echo [x] Oh, sorry. For some reason, the application "unlockfps_clr.exe" failed to start.
    echo [i] If you need help, please create a new Issue on GitHub.
)

echo [i] https://github.com/sefinek24/genshin-impact-reshade-2023/issues

echo.
set /p null="Press ENTER to close this window ..."
exit