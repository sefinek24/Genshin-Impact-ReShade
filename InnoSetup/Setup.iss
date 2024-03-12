#define AppName "Genshin Stella Mod"
#define AppVersion "8.0.0.2"
#define AppPublisher "Sefinek Inc."
#define AppURL "https://genshin.sefinek.net"
#define AppExeName "Stella Mod Launcher.exe"
#define AppCopyright "Copyright 2024 © by Sefinek. All Rights Reserved."
#define AppId "5D6E44F3-2141-4EA4-89E3-6C3018583FF7"
#define public Dependency_Path_NetCoreCheck "Data\Dependencies\"
#include "Data\Dependencies\CodeDependencies.iss"

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
VersionInfoOriginalFileName=Genshin Stella Mod
AppComments=Official installer for Stella Mod Launcher and Genshin Stella Mod.

ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\Sefinek\Genshin-Stella-Mod
DisableDirPage=no
DisableWelcomePage=no
ChangesAssociations=no
InfoBeforeFile=..\Build\Release\net8.0-windows\data\README.txt
LicenseFile=..\Build\Release\net8.0-windows\LICENSE
PrivilegesRequired=none
OutputBaseFilename=Stella-Mod-Setup_{#AppVersion}
WizardStyle=classic
DirExistsWarning=no
DisableProgramGroupPage=yes
AlwaysShowGroupOnReadyPage=True
AlwaysShowDirOnReadyPage=True

SetupIconFile="..\Assets\Images\InnoSetup\setup.ico"
UninstallDisplayIcon="{app}\uninstall.ico"
WizardImageFile="..\Assets\Images\InnoSetup\WizardImageFile.bmp"
WizardSmallImageFile="..\Assets\Images\InnoSetup\WizardSmallImageFile.bmp"

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
Name: "CreateDesktopIcon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Check: not IsUpdating

[Files]
Source: "..\Build\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Assets\Images\InnoSetup\uninstall.ico"; DestDir: "{app}";
Source: "Data\music.mp3"; Flags: dontcopy
Source: "Data\Dependencies\bass\bass.dll"; Flags: dontcopy

[Icons]
Name: "{autodesktop}\Stella Mod Launcher"; Filename: "{app}\net8.0-windows\{#AppExeName}"; Tasks: CreateDesktopIcon
Name: "{autoprograms}\Stella Mod Launcher\Uninstall {#AppName}"; Filename: "{uninstallexe}"

[Registry]
Root: HKCU; Subkey: "SOFTWARE\Stella Mod Launcher"; ValueType: string; ValueName: "StellaPath"; ValueData: "{app}\net8.0-windows\";
Root: HKCU; Subkey: "SOFTWARE\Stella Mod Launcher"; ValueType: string; ValueName: "InstallationDate"; ValueData: "{code:GetCurrentDateTime}"; Check: not IsUpdating
Root: HKCU; Subkey: "SOFTWARE\Stella Mod Launcher"; ValueType: string; ValueName: "LastUpdateDate"; ValueData: "{code:GetCurrentDateTime}"; Check: IsUpdating

[Run]
Filename: "{app}\net8.0-windows\{#AppExeName}"; WorkingDir: "{app}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}} Launcher"; Flags: nowait postinstall skipifsilent runascurrentuser; Check: not IsUpdating
Filename: "{app}\net8.0-windows\{#AppExeName}"; WorkingDir: "{app}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}} Launcher"; Flags: nowait postinstall runascurrentuser; Check: IsUpdating

[Code]
const  
  BASS_SAMPLE_LOOP = 0;
  BASS_UNICODE = $80000000;
  BASS_CONFIG_GVOL_STREAM = 5;
const
  #ifndef UNICODE
    EncodingFlag = 0;
  #else
    EncodingFlag = BASS_UNICODE;
  #endif
type
  HSTREAM = DWORD;

function BASS_Init(device: LongInt; freq, flags: DWORD; 
  win: HWND; clsid: Cardinal): BOOL;
  external 'BASS_Init@files:bass.dll stdcall';
function BASS_StreamCreateFile(mem: BOOL; f: string; offset1: DWORD; 
  offset2: DWORD; length1: DWORD; length2: DWORD; flags: DWORD): HSTREAM;
  external 'BASS_StreamCreateFile@files:bass.dll stdcall';
function BASS_ChannelPlay(handle: DWORD; restart: BOOL): BOOL; 
  external 'BASS_ChannelPlay@files:bass.dll stdcall';
function BASS_SetConfig(option: DWORD; value: DWORD ): BOOL;
  external 'BASS_SetConfig@files:bass.dll stdcall';
function BASS_Free: BOOL;
  external 'BASS_Free@files:bass.dll stdcall';

procedure InitializeWizard;
var
  StreamHandle: HSTREAM;
begin
  ExtractTemporaryFile('music.mp3');
  if BASS_Init(-1, 44100, 0, 0, 0) then
  begin
    StreamHandle := BASS_StreamCreateFile(False, ExpandConstant('{tmp}\music.mp3'), 0, 0, 0, 0, EncodingFlag or BASS_SAMPLE_LOOP);
    BASS_SetConfig(BASS_CONFIG_GVOL_STREAM, 2500);
    BASS_ChannelPlay(StreamHandle, False);
  end;
end;

procedure DeinitializeSetup;
begin
  BASS_Free;
end;

{ ///////////////////////////////////////////////////////////////////// } 
function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet48;
  Dependency_AddDotNet80;

  Dependency_ForceX86 := True;
  Dependency_AddVC2015To2022;
  Dependency_ForceX86 := False;
  Dependency_AddVC2015To2022;

  Dependency_AddWebView2;

  Result := True;
end;

function NeedRestart: Boolean;
begin
  Result := Dependency_NeedRestart;
end;


function InitializeUninstall(): Boolean;
var
  ErrorCode: Integer;
begin
  if CheckForMutexes('{#AppId}') then
  begin
    if MsgBox('Uninstall has detected that {#AppName} is currently running.' + #13#10#13#10 + 'Would you like to close it?', mbError, MB_YESNO) = IDYES then
    begin
      Exec('taskkill.exe', '/f /im {#AppExeName}', '', SW_HIDE, ewWaitUntilTerminated, ErrorCode);
    end
    else
    begin
      Result := False;
      Exit;
    end;
  end;

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

function IsUpdating(): Boolean;
begin
  Result := CmdLineParamExists('/UPDATE');
end;

function GetCurrentDateTime(Param: String): String;
begin
  Result := GetDateTimeString('dd.mm.yyyy hh:nn:ss', #0, #0);
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
