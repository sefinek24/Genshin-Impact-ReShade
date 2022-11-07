@echo off
chcp 65001 > NUL

echo.               Genshin Impact ReShade 2023 Mod Pack
echo.                  Cache removal tool for Reshade & echo.
echo.⠀⠀  ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀⠀  ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿
echo.⠀  ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿
echo.⠀⠀  ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.⠀⠀  ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo.⠀⠀⠀  ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo.⠀⠀⠀  ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.⠀  ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆
echo.⠀⠀  ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆
echo.⠀  ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇
echo.  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.⠀  ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ================================================================ & echo.

echo [i] ReShade cache path: %temp%\ReShade

if exist "%temp%\ReShade" (
    goto question
) else (
    echo [x] Cache folder for ReShade does not exist.
    goto nothingToDo
)


:question
    set /p res="[i] Are you sure? (y/n): "
    if "%res%" == "y" (
        goto delete
    ) else if "%res%" == "n" (
        goto no
    ) else (
        goto badAnswer
    )

:delete
    echo [i] Please wait...
    rmdir /s /q %temp%\ReShade
    echo [✓] Done! You can close this window.

    goto nothingToDo

:no
    echo [i] Ok. Press [Enter] to close this window.
    goto nothingToDo

:badAnswer
    echo.
    echo [x] Wrong answer. Expected: 'y' or 'n'. Close this window and please try again.
    echo [i] y = yes
    echo [i] n = no

:nothingToDo
    set /p null=
    exit