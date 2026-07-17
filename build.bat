@echo off
setlocal
echo ===================================================
echo Building ClipboardPaster.exe using pre-installed csc.exe...
echo ===================================================

set CSC_PATH="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if not exist %CSC_PATH% (
    echo [ERROR] csc.exe not found at %CSC_PATH%
    echo Please make sure Microsoft .NET Framework 4.0/4.8 is installed on Windows.
    exit /b 1
)

if not exist bin mkdir bin

set WINMETADATA=C:\Windows\System32\WinMetadata
set SYS_RUNTIME=C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Runtime\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Runtime.dll

set OCR_REFS=
if exist "%WINMETADATA%\Windows.Media.winmd" if exist "%SYS_RUNTIME%" (
    set OCR_REFS=/reference:"%SYS_RUNTIME%" /reference:"%WINMETADATA%\Windows.Media.winmd" /reference:"%WINMETADATA%\Windows.Graphics.winmd" /reference:"%WINMETADATA%\Windows.Foundation.winmd" /reference:"%WINMETADATA%\Windows.Storage.winmd" /reference:"%WINMETADATA%\Windows.Globalization.winmd"
)

%CSC_PATH% /nologo /target:winexe /optimize+ /out:bin\ClipboardPaster.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll %OCR_REFS% src\*.cs

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Compilation failed!
    exit /b %ERRORLEVEL%
)

echo.
echo [SUCCESS] Compiled successfully to: bin\ClipboardPaster.exe
echo.
echo To run the GUI installer:
echo   bin\ClipboardPaster.exe
echo.
echo To install directly from command line:
echo   bin\ClipboardPaster.exe --install
echo.
