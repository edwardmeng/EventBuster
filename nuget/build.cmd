@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
) else (
   set version=-Version 1.0.0
)
REM Determine msbuild path
set msbuildtmp="%ProgramFiles%\MSBuild\14.0\bin\msbuild"
if exist %msbuildtmp% set msbuild=%msbuildtmp%
set msbuildtmp="%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild"
if exist %msbuildtmp% set msbuild=%msbuildtmp%
set VisualStudioVersion=14.0

REM Package restore
echo.
echo Running package restore...
call :ExecuteCmd nuget.exe restore ..\EventBuster.sln -OutputDirectory ..\packages -NonInteractive -ConfigFile nuget.config
IF %ERRORLEVEL% NEQ 0 goto error

if not exist %cd%\packages mkdir %cd%\packages

call :Package EventBuster netstandard1.1
call :Package EventBuster.Activation netstandard1.6
goto end


:Package
setlocal
set _PackageName=%1
set _CoreFramework=%2

if not exist %cd%\lib mkdir %cd%\lib
if not exist %cd%\lib\net35 mkdir %cd%\lib\net35
if not exist %cd%\lib\net451 mkdir %cd%\lib\net451
if not exist %cd%\lib\%_CoreFramework% mkdir %cd%\lib\%_CoreFramework%

rmdir ..\build\net35\%_PackageName%\bin /S /Q
rmdir ..\build\net35\%_PackageName%\obj /S /Q
call :ExecuteCmd %msbuild% ..\build\net35\%_PackageName%\%_PackageName%.net35.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
copy ..\build\net35\%_PackageName%\bin\%config%\%_PackageName%.dll %cd%\lib\net35\ /Y
copy ..\build\net35\%_PackageName%\bin\%config%\%_PackageName%.xml %cd%\lib\net35\ /Y

rmdir ..\build\net451\%_PackageName%\bin /S /Q
rmdir ..\build\net451\%_PackageName%\obj /S /Q
call :ExecuteCmd %msbuild% ..\build\net451\%_PackageName%\%_PackageName%.net451.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
copy ..\build\net451\%_PackageName%\bin\%config%\%_PackageName%.dll %cd%\lib\net451\ /Y
copy ..\build\net451\%_PackageName%\bin\%config%\%_PackageName%.xml %cd%\lib\net451\ /Y

rmdir ..\build\netcore\%_PackageName%\bin /S /Q
rmdir ..\build\netcore\%_PackageName%\obj /S /Q
call :ExecuteCmd %msbuild% ../build/netcore/%_PackageName%/%_PackageName%.netcore.xproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
copy ..\build\netcore\%_PackageName%\bin\%config%\%_CoreFramework%\%_PackageName%.dll %cd%\lib\%_CoreFramework%\ /Y
copy ..\build\netcore\%_PackageName%\bin\%config%\%_CoreFramework%\%_PackageName%.xml %cd%\lib\%_CoreFramework%\ /Y
copy ..\build\netcore\%_PackageName%\bin\%config%\%_CoreFramework%\%_PackageName%.deps.json %cd%\lib\%_CoreFramework%\ /Y

call :ExecuteCmd nuget.exe pack "%cd%\%_PackageName%.nuspec" -OutputDirectory %cd%\packages %version%
rmdir %cd%\lib /S /Q

exit /b %ERRORLEVEL%

:: Execute command routine that will echo out when error
:ExecuteCmd
setlocal
set _CMD_=%*
call %_CMD_%
if "%ERRORLEVEL%" NEQ "0" echo Failed exitCode=%ERRORLEVEL%, command=%_CMD_%
exit /b %ERRORLEVEL%

:error
endlocal
echo An error has occurred during build.
call :exitSetErrorLevel
call :exitFromFunction 2>nul

:exitSetErrorLevel
exit /b 1

:exitFromFunction
()

:end
endlocal
echo Build finished successfully.