@echo off
MakeAppx pack /v /h SHA256 /d "D:\GitHub\Stella\Genshin-Stella-Mod" /p "C:\Stella Mod Launcher.msix"
echo.
set /p passwd=Password: 
SignTool sign /debug /fd SHA256 /a /f C:\Stella-Mod-Launcher.pfx /p %passwd% "C:\Stella Mod Launcher.msix"
pause