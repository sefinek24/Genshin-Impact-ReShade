echo 5/5 - Waiting for processes...
echo [i] unlockfps_clr.exe - SKIPPED

tasklist /fi "ImageName eq inject.exe" /fo csv 2>NUL | find /I "inject.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] inject.exe - OK & echo.
    echo [i] You can open game.
) else (
    echo [x] inject.exe - ERROR
    call Data\Cmd\start\error.cmd
    pause
)

call Data\Cmd\start\done.cmd