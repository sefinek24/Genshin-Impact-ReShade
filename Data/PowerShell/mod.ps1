Set-Location -Path "Data\Reshade"
Start-Process -FilePath "inject.exe" "GenshinImpact.exe" -verb RunAs

Set-Location -Path "..\FPS Unlocker"
Start-Process -FilePath "..\unlockfps_clr.exe" -verb RunAs