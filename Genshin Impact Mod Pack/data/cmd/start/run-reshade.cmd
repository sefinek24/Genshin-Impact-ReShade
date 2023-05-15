if "%GameVersion%" equ "1" (
    call "inject64.exe" "GenshinImpact.exe"
) else if "%GameVersion%" equ "2" (
    call "inject64.exe" "YuanShen.exe"
) else (
    echo [x] Something went wrong. Unknown game version ID: %GameVersion%
    goto pause
)
