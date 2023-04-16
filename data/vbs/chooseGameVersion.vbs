Set WshShell = CreateObject("WScript.Shell")

Dim userInput

Do
userInput = InputBox("Please select the game version for your region." & vbCrLf & vbCrLf & "Type the correct number:" & vbCrLf & "1 - OS (GenshinImpact.exe)" & vbCrLf & "2 - CN (YuanShen.exe)", "Genshin Stella Mod by Sefinek")
If userInput <> "1" And userInput <> "2" Then
    MsgBox "Incorrect number entered. Please enter 1 or 2.", vbCritical, "Genshin Stella Mod by Sefinek"
End If

Loop Until userInput = "1" Or userInput = "2"

Dim fso, outFile, appDataPath
Set fso = CreateObject("Scripting.FileSystemObject")
appDataPath = WshShell.ExpandEnvironmentStrings("%APPDATA%")
Set outFile = fso.CreateTextFile(appDataPath & "\Genshin Stella Mod by Sefinek\game-version.sfn", True)
outFile.Write(userInput)
outFile.Close

MsgBox "Your game version has been saved. You may now close this.", vbInformation, "Genshin Stella Mod by Sefinek"