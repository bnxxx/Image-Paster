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

%CSC_PATH% /nologo /target:winexe /optimize+ /out:bin\ClipboardPaster.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll src\*.cs

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
