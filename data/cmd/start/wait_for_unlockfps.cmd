echo 4/4 - Waiting for processes...
tasklist /fi "ImageName eq unlockfps_clr.exe" /fo csv 2>NUL | find /I "unlockfps_clr.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [✓] unlockfps_clr.exe - OK
    call done.cmd
) else (
    echo [x] unlockfps_clr.exe - ERROR
    call error.cmd
    pause
)