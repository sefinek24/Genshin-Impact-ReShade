@echo off
setlocal

set "file_path=C:\GitHub\VS\Stella\Genshin-Stella-Mod\Genshin Stella Mod made by Sefinek.sln"
set "vs_path=C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe"

echo Running Genshin Stella Mod made by Sefinek.sln file as administrator... && echo.

powershell -Command "Start-Process -FilePath '%vs_path%' -ArgumentList '\"%file_path%\"' -Verb RunAs"

endlocal