set PATH=C:\Program Files\Java\jdk1.7.0_75\bin;%PATH%

dir
cd github\scratch\kokoro-codelab
dir
call build.bat

exit %ERRORLEVEL%