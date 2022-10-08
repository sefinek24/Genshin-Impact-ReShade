@echo off
echo Cache folder: %temp%\ReShade & echo.

if exist "%temp%\ReShade" (
    echo Please wait...
    rmdir /s /q %temp%\ReShade
    echo Done!
) else (
    echo The cache file for ReShade does not exist.
)

echo.
pause