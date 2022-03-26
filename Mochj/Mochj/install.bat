@echo off
rem install.bat
mkdir "%ProgramFiles%\Mochj"
mkdir "%ProgramFiles%\Mochj\pkg"
mkdir "%ProgramFiles%\Mochj\stage"
echo %ERRORLEVEL% created directory %ProgramFiles%\Mochj
copy * C:\"Program Files"\Mochj
echo %ERRORLEVEL% copied files
FTYPE "Mochj Script File"="%ProgramFiles%\Mochj\MyProg.exe" $%1
ASSOC .mochj="Mochj Script File"
echo %ERRORLEVEL% associated file types
del "C:\Program Files\Mochj\install.bat"
echo %ERRORLEVEL% success
pause