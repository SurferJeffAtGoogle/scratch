if "%1"=="release" (
  javac -g:none Hello.java
) else (
  javac Hello.java
)

exit /b %ERRORLEVEL%