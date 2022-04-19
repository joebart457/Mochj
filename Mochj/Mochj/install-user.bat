@echo off
rem install-user.bat
mkdir "%AppData%\Mochj"
mkdir "%AppData%\Mochj\pkg"
mkdir "%AppData%\Mochj\stage"
echo %ERRORLEVEL% created directory %AppData%\Mochj
copy * "%AppData%\Mochj"
echo %ERRORLEVEL% copied files
del "%AppData%\Mochj\install-user.bat"
echo %ERRORLEVEL% success
pause