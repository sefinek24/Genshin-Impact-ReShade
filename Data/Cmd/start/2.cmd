echo 7/8 - Waiting for unlockfps_clr.exe...
echo [✓] Skipped.
echo.

echo 8/8 - Waiting for inject.exe...
tasklist /fi "ImageName eq inject.exe" /fo csv 2>NUL | find /I "inject.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] Ok. You can open game.
) else (
    echo [x] Error.
    call Data\Cmd\start\error.cmd
    pause
)

call Data\Cmd\start\done.cmd