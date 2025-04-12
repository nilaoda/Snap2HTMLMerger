@echo off
:: Path to the fixed template file (assumed to be in the same directory as the batch file)
set "template=template.html"

:: Set current directory
pushd "%~dp0"

:: Call the executable with the template and all dropped files
"%~dp0Snap2HTMLMerger.exe" "%~dp0%template%" %*

:: Pause to allow the user to see the result
pause