@SETLOCAL

@call :install rake
@call :install rubyzip
@call :install albacore

@ENDLOCAL

exit /b

:install
@call gem list -l -i --no-verbose %1 2>&1 > NUL
IF ERRORLEVEL 1 (@call ECHO *** Installing %1 && gem install %1 --no-rdoc --no-ri) ELSE (@call ECHO *** %1 already installed.)

exit /b
