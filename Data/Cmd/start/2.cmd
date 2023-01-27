echo 3/3 - Waiting for processes...
echo [i] unlockfps_clr.exe - SKIPPED

tasklist /fi "ImageName eq inject64.exe" /fo csv 2>NUL | find /I "inject64.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] inject64.exe - OK & echo.
    echo [i] You can open game.
) else (
    echo [x] inject64.exe - ERROR
    call Data\Cmd\start\error.cmd
    pause
)

call Data\Cmd\start\done.cmd