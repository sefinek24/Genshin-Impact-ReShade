@echo off
cd ..\

if exist "genshin-impact-reshade-2023" (
    ren genshin-impact-reshade-2023 Genshin-Impact-ReShade
)

echo.
cd Genshin-Impact-ReShade
echo [✓] Done! & echo.

echo [i] New paths!
echo [i] Presets: C:\Genshin-Impact-ReShade\Data\- Presets
echo [i] Effects: C:\Genshin-Impact-ReShade\Data\- Shaders\Effects
echo [i] Textures: C:\Genshin-Impact-ReShade\Data\- Shaders\Textures

"Genshin Impact Mod Pack.exe"
del %temp%\rename.cmd
pause