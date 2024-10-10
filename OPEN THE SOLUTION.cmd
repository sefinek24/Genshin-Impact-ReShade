@echo off
setlocal

set "file_path=D:\Projects\stella\Genshin-Impact-ReShade\Genshin Stella Mod made by Sefinek.sln"
set "vs_path=C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe"

for %%f in ("%file_path%") do (
    echo Running the '%%~nxf' file as administrator... && echo.
    powershell.exe -Command "Start-Process '%vs_path%' -ArgumentList '\"%file_path%\"' -Verb RunAs"
)

endlocal