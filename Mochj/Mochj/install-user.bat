@echo off
rem install-user.bat
mkdir "%AppData%\Mochj"
mkdir "%AppData%\Mochj\pkg"
mkdir "%AppData%\Mochj\stage"
mkdir "%AppData%\Mochj\vsix"
echo %ERRORLEVEL% created directory %AppData%\Mochj
copy "Mochj_VSIntegration.vsix" "%AppData%\Mochj\vsix"
copy * "%AppData%\Mochj"
echo %ERRORLEVEL% copied files
del "%AppData%\Mochj\install-user.bat"
del "%AppData%\Mochj\Mochj_VSIntegration.vsix"
echo %ERRORLEVEL% success
pause