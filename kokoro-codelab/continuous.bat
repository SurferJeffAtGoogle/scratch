set PATH=C:\Program Files\Java\jdk1.7.0_75\bin;%PATH%

AT > NUL
IF %ERRORLEVEL% EQU 0 (
    ECHO you are Administrator
) ELSE (
    ECHO you are NOT Administrator. Exiting...
) 
dir
cd github\scratch\kokoro-codelab
dir
call build.bat

exit %ERRORLEVEL%