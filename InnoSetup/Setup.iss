#define AppName "Genshin Stella Mod"
#define AppVersion "7.9.7.6"
#define AppPublisher "Sefinek Inc."
#define AppURL "https://genshin.sefinek.net"
#define AppExeName "Stella Mod Launcher.exe"
#define AppCopyright "Copyright 2024 © by Sefinek. All Rights Reserved."
#define AppId "5D6E44F3-2141-4EA4-89E3-6C3018583FF7"
#define public Dependency_Path_NetCoreCheck "Data\Dependencies\"
#include "Data\Dependencies\DependencyInstaller.iss"

[Setup]
AppCopyright={#AppCopyright}
AppId={#AppId}
AppMutex={#AppId}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppContact=contact@sefinek.net

ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
DefaultDirName={autopf}\Sefinek\Genshin-Stella-Mod
DisableDirPage=no
DisableWelcomePage=no
ChangesAssociations=no
InfoBeforeFile=..\Build\Release\net8.0-windows\data\README.txt
LicenseFile=..\Build\Release\net8.0-windows\LICENSE
PrivilegesRequired=none
OutputBaseFilename=Stella-Mod-Setup
WizardStyle=classic

DirExistsWarning=no
DisableProgramGroupPage=yes
UninstallDisplayIcon="..\Assets\Images\Icons\setup\52x52.ico"

AppSupportURL=https://genshin.sefinek.net/support
AppUpdatesURL=https://genshin.sefinek.net/docs?page=changelog_v7
VersionInfoCompany={#AppPublisher}
VersionInfoProductName=Stella Mod Setup
VersionInfoProductTextVersion={#AppVersion}
VersionInfoDescription={#AppName + " v" + AppVersion + " Setup"}
VersionInfoVersion={#AppVersion}
VersionInfoProductVersion={#AppVersion}
VersionInfoTextVersion={#AppVersion}
VersionInfoCopyright={#AppCopyright}

WizardImageFile="..\Assets\Images\InnoSetup\WizardImageFile.bmp"
WizardSmallImageFile="..\Assets\Images\InnoSetup\WizardSmallImageFile.bmp"
SetupIconFile="..\Assets\Images\Icons\setup\52x52.ico"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "armenian"; MessagesFile: "compiler:Languages\Armenian.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "bulgarian"; MessagesFile: "compiler:Languages\Bulgarian.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "icelandic"; MessagesFile: "compiler:Languages\Icelandic.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "slovak"; MessagesFile: "compiler:Languages\Slovak.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Check: not InstViaSetup and not InstViaLauncher

[Files]
Source: "..\Build\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autodesktop}\Stella Mod Launcher"; Filename: "{app}\net8.0-windows\{#AppExeName}"; Tasks: desktopicon
Name: "{autoprograms}\Stella Mod Launcher\Uninstall {#AppName}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\net8.0-windows\{#AppExeName}"; WorkingDir: "{app}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}} Launcher"; Flags: nowait postinstall skipifsilent runascurrentuser

[Code]
{ ///////////////////////////////////////////////////////////////////// }
function InitializeSetup: Boolean;
begin
  Dependency_AddWebView2;

  Dependency_AddDotNet48;
  Dependency_AddDotNet80;

  Dependency_ForceX86 := True;
  Dependency_AddVC2015To2022;
  Dependency_ForceX86 := False;
  Dependency_AddVC2015To2022;

  Result := True;
end;


{ ///////////////////////////////////////////////////////////////////// }
function CmdLineParamExists(const value: string): Boolean;
var
  i: Integer;
begin
  Result := False;
  for i := 1 to ParamCount do
    if CompareText(ParamStr(i), value) = 0 then
    begin
      Result := True;
      Exit;
    end;
end;

function InstViaSetup(): Boolean;
begin
  Result := CmdLineParamExists('/SETUP');
end;

function InstViaLauncher(): Boolean;
begin
  Result := CmdLineParamExists('/UPDATE');
end;


{ ///////////////////////////////////////////////////////////////////// }
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;


{ ///////////////////////////////////////////////////////////////////// }
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;


{ ///////////////////////////////////////////////////////////////////// }
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
{ Return Values: }
{ 1 - uninstall string is empty }
{ 2 - error executing the UnInstallString }
{ 3 - successfully executed the UnInstallString }

  { default return value }
  Result := 0;

  { get the uninstall string of the old app }
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

{ ///////////////////////////////////////////////////////////////////// }
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;
