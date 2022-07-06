@echo off
rem install-global.bat
mkdir "%ProgramFiles%\Mochj"
mkdir "%ProgramFiles%\Mochj\pkg"
mkdir "%ProgramFiles%\Mochj\stage"
mkdir "%ProgramFiles%\Mochj\vsix"
echo %ERRORLEVEL% created directory %ProgramFiles%\Mochj
copy "Mochj_VSIntegration.vsix" C:\"Program Files"\Mochj\vsix
copy * C:\"Program Files"\Mochj
echo %ERRORLEVEL% copied files
FTYPE "Mochj Script File"="%ProgramFiles%\Mochj\Mochj.exe" "'%%1'"
ASSOC .mochj="Mochj Script File"
echo %ERRORLEVEL% associated file types
FTYPE "Debug Mochj Script File"="%ProgramFiles%\Mochj\dmochj.bat" "'%%1'"
ASSOC .dmochj="Debug Mochj Script File"
echo %ERRORLEVEL% associated file types
del "C:\Program Files\Mochj\install-global.bat"
del "C:\Program Files\Mochj\Mochj_VSIntegration.vsix"
echo %ERRORLEVEL% success
pause