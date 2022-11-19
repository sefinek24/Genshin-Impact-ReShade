echo 7/8 - Waiting for unlockfps_clr.exe...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] Ok.
) else (
    echo [x] Error.
    call Data\Cmd\start\error.cmd
    pause
)
echo.

echo 8/8 - Waiting for inject.exe...
tasklist /fi "ImageName eq inject.exe" /fo csv 2>NUL | find /I "inject.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] Ok.
    call Data\Cmd\start\done.cmd
) else (
    echo [x] Error.
    call Data\Cmd\start\error.cmd
    pause
)