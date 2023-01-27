echo 3/3 - Waiting for processes...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] unlockfps_clr.exe - OK
) else (
    echo [x] unlockfps_clr.exe - ERROR
    call Data\Cmd\start\error.cmd
    pause
)

tasklist /fi "ImageName eq inject64.exe" /fo csv 2>NUL | find /I "inject64.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] inject64.exe - OK
    call Data\Cmd\start\done.cmd
) else (
    echo [x] inject64.exe - ERROR
    call Data\Cmd\start\error.cmd
    pause
)