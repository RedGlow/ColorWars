# Inclusion

!include "MUI2.nsh"
!include "DotNetChecker.nsh"

# Extra functions

!define LVM_GETITEMCOUNT_ALIAS 0x1004
!define LVM_GETITEMTEXT 0x102D
 
Function DumpLog
  Exch $5
  Push $0
  Push $1
  Push $2
  Push $3
  Push $4
  Push $6
 
  FindWindow $0 "#32770" "" $HWNDPARENT
  GetDlgItem $0 $0 1016
  StrCmp $0 0 exit
  FileOpen $5 $5 "w"
  StrCmp $5 "" exit
    SendMessage $0 ${LVM_GETITEMCOUNT_ALIAS} 0 0 $6
    System::Alloc ${NSIS_MAX_STRLEN}
    Pop $3
    StrCpy $2 0
    System::Call "*(i, i, i, i, i, i, i, i, i) i \
      (0, 0, 0, 0, 0, r3, ${NSIS_MAX_STRLEN}) .r1"
    loop: StrCmp $2 $6 done
      System::Call "User32::SendMessageA(i, i, i, i) i \
        ($0, ${LVM_GETITEMTEXT}, $2, r1)"
      System::Call "*$3(&t${NSIS_MAX_STRLEN} .r4)"
      FileWrite $5 "$4$\r$\n"
      IntOp $2 $2 + 1
      Goto loop
    done:
      FileClose $5
      System::Free $1
      System::Free $3
  exit:
    Pop $6
    Pop $4
    Pop $3
    Pop $2
    Pop $1
    Pop $0
    Exch $5
FunctionEnd

# General settings

Name "Color Wars"
OutFile "Installer.exe"
InstallDir "$PROGRAMFILES\Color Wars"
InstallDirRegKey HKCU "Software\Color Wars" ""

# Interface settings

!define MUI_ABORTWARNING

# Configuration

Var StartMenuFolder
  
# Installation pages

Function installdesktop
	CreateShortCut "$DESKTOP\Color Wars.lnk" "$INSTDIR\ColorWars.exe"
FunctionEnd

!insertmacro MUI_PAGE_WELCOME
;!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\Color Wars" 
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
!insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_SHOWREADME ""
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
!define MUI_FINISHPAGE_SHOWREADME_TEXT "Create Desktop Shortcut"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION installdesktop
!insertmacro MUI_PAGE_FINISH

# Uninstallation pages

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

# Languages

!insertmacro MUI_LANGUAGE "English"

# Installation sections

Section "Color Wars" SecInst
	SetShellVarContext all
	SetOutPath $INSTDIR

	; .NET
	!insertmacro CheckNetFramework 45

	; program files
	File "..\ColorWars\bin\Release\ColorWars.exe"
	File "..\ColorWars\bin\Release\ColorManagment.dll"
	File "..\ColorWars\bin\Release\Effects.dll"
	File "..\ColorWars\bin\Release\ICCReader.dll"
	File "..\ColorWars\bin\Release\Kent.Boogaart.Converters.dll"
	File "..\ColorWars\bin\Release\Kent.Boogaart.HelperTrinity.dll"
	File "..\ColorWars\bin\Release\Newtonsoft.Json.dll"
	File "..\ColorWars\bin\Release\Xceed.Wpf.Toolkit.dll"
	WriteRegStr HKCU "Software\Color Wars" "" $INSTDIR
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
	; start menu
	!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
    CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Color Wars.lnk" "$INSTDIR\ColorWars.exe"
    CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
	!insertmacro MUI_STARTMENU_WRITE_END

	; add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Color Wars" "DisplayName" "Color Wars"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Color Wars" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""

	; log
	StrCpy $0 "$INSTDIR\install.log"
	Push $0
	Call DumpLog

SectionEnd

# Uninstallation sections

Section "un.Uninstallation"
	SetShellVarContext all
	Delete "$INSTDIR\Uninstall.exe"
	Delete "$INSTDIR\ColorWars.exe"
	Delete "$INSTDIR\ColorManagment.dll"
	Delete "$INSTDIR\Effects.dll"
	Delete "$INSTDIR\ICCReader.dll"
	Delete "$INSTDIR\Kent.Boogaart.Converters.dll"
	Delete "$INSTDIR\Kent.Boogaart.HelperTrinity.dll"
	Delete "$INSTDIR\Newtonsoft.Json.dll"
	Delete "$INSTDIR\Xceed.Wpf.Toolkit.dll"
	Delete "$INSTDIR\install.log"
  	RMDir "$INSTDIR"

	Delete "$DESKTOP\Color Wars.lnk"

	!insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
	Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk"
    Delete "$SMPROGRAMS\$StartMenuFolder\Color Wars.lnk"
	RMDir "$SMPROGRAMS\$StartMenuFolder"

	DeleteRegKey /ifempty HKCU "Software\Color Wars"
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Color Wars"
SectionEnd