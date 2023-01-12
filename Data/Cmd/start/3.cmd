echo 5/5 - Waiting for processes...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] unlockfps_clr.exe - OK
) else (
    echo [x] unlockfps_clr.exe - ERROR
    call Data\Cmd\start\error.cmd
    pause
)

tasklist /fi "ImageName eq inject.exe" /fo csv 2>NUL | find /I "inject.exe">NUL
echo [✓] inject.exe - SKIPPED

call Data\Cmd\start\done.cmd