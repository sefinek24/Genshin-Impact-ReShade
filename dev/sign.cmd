@echo off
set /p passwd=Password:
SignTool sign /fd SHA256 /a /f C:\cert.pfx /p %passwd% "C:\Genshin Stella Mod.msix"
pause