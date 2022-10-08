Set-Location -Path "Data\Reshade"
Start-Process -FilePath "inject.exe" "GenshinImpact.exe" -verb RunAs
Start-Process -FilePath "..\unlockfps_clr.exe" -verb RunAs