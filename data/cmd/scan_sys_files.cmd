@echo off
title Scan and repair system files
chcp 65001 > nul
echo.⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                Scan and repair system files
echo.⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v6.0.1.0
echo.    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.8.0
echo.   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.8
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/7 - Checking for administrative permissions...
net session >nul 2>&1
if %ErrorLevel% == 0 (
    echo [✓] Administrative permissions verified.
    echo.
) else (
    echo [x] Error: This command must be run as administrator.
    pause
    exit
)

REM Run System File Checker tool to repair missing or corrupted system files.
echo 2/7 - Running System File Checker tool...
sfc /SCANNOW && echo.
echo.

REM Check the file system and file system metadata of a volume for logical and physical errors.
echo 3/7 - Checking file system and file system metadata...
CHKDSK /F /R /X && echo.
echo.

REM Repair a Windows Image: Check if an image is repairable.
echo 4/7 - Scanning Windows Image for corruption...
echo This operation will take several minutes.
echo Internet connection is required.
echo See: https://learn.microsoft.com/en-us/windows-hardware/manufacture/desktop/repair-a-windows-image?view=windows-11
DISM /Online /Cleanup-Image /ScanHealth && echo.
echo.

REM Repair a Windows Image: Check the image to see whether any corruption has been detected.
echo 5/7 - Checking for Windows Image corruption...
DISM /Online /Cleanup-Image /CheckHealth && echo.
echo.

REM Repair a Windows Image: Scan the store for corruption and also repairs corrupted files.
echo 6/7 - Repairing Windows Image...
echo If the command appears stuck, this is normal behavior.
echo After a few minutes, the process will complete successfully.
DISM /Online /Cleanup-Image /RestoreHealth && echo.
echo.

echo 7/7 - Running System File Checker tool again...
sfc /SCANNOW && echo.
echo.

echo [i] Done. You can close this window.
pause
exit