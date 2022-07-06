@echo OFF
rem RunTests.bat
IF %1 EQU Package Echo Tests not run for output type 'Package'
IF %1 EQU Package EXIT 0
"%CD%/mochj.exe" $%CD%/Test.mochj
EXIT /B %ERRORLEVEL%