@echo off
rem install.bat
mkdir "%ProgramFiles%\Mochj"
echo %ERRORLEVEL% created directory %ProgramFiles%\Mochj
copy * C:\"Program Files"\Mochj
echo %ERRORLEVEL% copied files
del "C:\Program Files\Mochj\install.bat"
echo %ERRORLEVEL% success?
pause