Option Explicit

Dim WshShell
Set WshShell = CreateObject("WScript.Shell")

Dim userInput
Do
    userInput = InputBox("Please select the game version for your region." & vbCrLf & vbCrLf & "Type the correct number:" & vbCrLf & "1 - GenshinImpact.exe [Default version OS]" & vbCrLf & "2 - YuanShen.exe [Version for China CN]", "Genshin Stella Mod by Sefinek")
    If userInput <> "1" And userInput <> "2" And userInput <> "" And userInput <> "9" Then
        MsgBox "Incorrect number entered. Please enter 1, 2 or 9 to cancel.", vbCritical, "Genshin Stella Mod"
    ElseIf userInput = "" Then
        MsgBox "No input provided. Please enter 1, 2, or 9 to cancel.", vbCritical, "Genshin Stella Mod"
        userInput = "0"
    End If
Loop Until userInput = "1" Or userInput = "2" Or userInput = "9"

If userInput <> "0" And userInput <> "9" Then
    Dim fso, outFile, appDataPath
    Set fso = CreateObject("Scripting.FileSystemObject")
    appDataPath = WshShell.ExpandEnvironmentStrings("%APPDATA%\Genshin Stella Mod by Sefinek")
    If Not fso.FolderExists(appDataPath) Then
        fso.CreateFolder appDataPath
    End If
    Set outFile = fso.CreateTextFile(appDataPath & "\game-version.sfn", True)
    outFile.Write(userInput)
    outFile.Close
    MsgBox "Your game version has been saved! You may now close this.", vbInformation, "Genshin Stella Mod"
End If


'' SIG '' Begin signature block
'' SIG '' MIIF1AYJKoZIhvcNAQcCoIIFxTCCBcECAQExDzANBglg
'' SIG '' hkgBZQMEAgEFADB3BgorBgEEAYI3AgEEoGkwZzAyBgor
'' SIG '' BgEEAYI3AgEeMCQCAQEEEE7wKRaZJ7VNj+Ws4Q8X66sC
'' SIG '' AQACAQACAQACAQACAQAwMTANBglghkgBZQMEAgEFAAQg
'' SIG '' Dd2gxPccBp4ZT4tz1PQX0Ythyn2Anv5WGJYBmamrFrug
'' SIG '' ggNAMIIDPDCCAiSgAwIBAgIQEYRz4lHhd6NMLJSUGLEx
'' SIG '' QzANBgkqhkiG9w0BAQsFADAvMS0wKwYDVQQDDCQyNDUw
'' SIG '' OEYzMS1FNjdBLTQyMTEtQkNBNi00MEIzODVCMDhBOEEw
'' SIG '' HhcNMjMwNTA5MTgwMTI1WhcNMjQwNTA5MTgyMTI1WjAv
'' SIG '' MS0wKwYDVQQDDCQyNDUwOEYzMS1FNjdBLTQyMTEtQkNB
'' SIG '' Ni00MEIzODVCMDhBOEEwggEiMA0GCSqGSIb3DQEBAQUA
'' SIG '' A4IBDwAwggEKAoIBAQDnr6ieTyQgtWOqSI6iSyiOhEbO
'' SIG '' 1zDX+YXVvP3SwazjZZVEDPEU5u4bO+8l1Ljx0R9HjIQx
'' SIG '' OFG4WDZXgHBe/32/O2felXM/6zqF+/9FX62ZxRNPA50E
'' SIG '' j2Fd+Q2GJGTfQhNpIAWqVHhJq06kvskf13aQIgSSkxCu
'' SIG '' G/J5TtFXwpm5JD+lvBV5ON1RGY8FhiewI2J4UeArsQib
'' SIG '' oklXC079qTESZx97yegMV5FVlSCKeuMZ5CjnLh8QmE9x
'' SIG '' 1xVSQOvdeQaTNHS8PsQnRHOhfW2JqiQrw8D4lTle0+Ga
'' SIG '' WT+87GhQN9SVHg266fh9DTcD665EKzxSAScYSyM/jnjk
'' SIG '' k7h71LQJAgMBAAGjVDBSMA4GA1UdDwEB/wQEAwIHgDAT
'' SIG '' BgNVHSUEDDAKBggrBgEFBQcDAzAMBgNVHRMBAf8EAjAA
'' SIG '' MB0GA1UdDgQWBBShHLP+lb/9t9UUg/Kg1zVFsew/ZDAN
'' SIG '' BgkqhkiG9w0BAQsFAAOCAQEAE5hRSr8px0brTK21DtOa
'' SIG '' QBjIQSLunmMR1XFOVmcJ4Q69FNxh0+3gjGMgGoe7e9Cd
'' SIG '' Wbu+Gn32MVQG2A7l/ujRdHNPIGR2OdtrvZ1nBbc/QgFL
'' SIG '' kYbDdMS3xtdrDN8jIC0OwBA/nz/ZpjP3sKfhcRO/pc8X
'' SIG '' XMX31hR64SRv/9LXLnRizSD0kHB7pUPSB3vfELNvzJSP
'' SIG '' OqdYC3HP98Gyq7/sFmzaPdxYycVmQdhic9huo+ik51Lj
'' SIG '' 3oFNRFIM68Tx3j8auSDohJVYwXlFmjx75insn1F5MgFr
'' SIG '' i3XIVVA4rI9Ebvb4ImkkIe9UDEUAAaGnNUf6b5fl+2K9
'' SIG '' 0NZXxs44Mrm55zGCAewwggHoAgEBMEMwLzEtMCsGA1UE
'' SIG '' AwwkMjQ1MDhGMzEtRTY3QS00MjExLUJDQTYtNDBCMzg1
'' SIG '' QjA4QThBAhARhHPiUeF3o0wslJQYsTFDMA0GCWCGSAFl
'' SIG '' AwQCAQUAoHwwEAYKKwYBBAGCNwIBDDECMAAwGQYJKoZI
'' SIG '' hvcNAQkDMQwGCisGAQQBgjcCAQQwHAYKKwYBBAGCNwIB
'' SIG '' CzEOMAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIE
'' SIG '' IJxnCt+HBZiBpnSHUNvXcCO4UoKV+NW+EJ0CWR9x5k/r
'' SIG '' MA0GCSqGSIb3DQEBAQUABIIBAM2VynawqXJzUSWhkGbJ
'' SIG '' LT81/iA7Xxqqn/m+ycCyLhMbXQnVFFm++SDb7GBo5vm5
'' SIG '' Cwxqqdbqwj0g0EpXRE4hnyp0B4rJ2s/yIr8mXkEtOMYX
'' SIG '' qIODCfply4lYMX1pow/sUCevxu9IjAzrLl11lriWwFVd
'' SIG '' LsPpDUdUPdM3fcO9k3dZ6FrhDIlbNtHsE2fUc6ZqVH+u
'' SIG '' nkLU1qTDNO7GUFMjC4jGsHW9ctMEpar2LVs0j2sN/y+e
'' SIG '' chpwtZB8HRF+0ZV2Dx3P1Ua/+jSBEO72dDSovq5I6y+M
'' SIG '' jofu6y7dyeFHBGznnuPzFkyErADQ/YDj0xeqv19YQd6c
'' SIG '' 9+XH6c0HjdZYnGE=
'' SIG '' End signature block
