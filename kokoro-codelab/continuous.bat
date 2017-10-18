set PATH=C:\Program Files\Java\jdk1.7.0_75\bin;%PATH%

choco install -y microsoft-build-tools dotnetcore-sdk

cd github\scratch\kokoro-codelab
call build.bat

exit %ERRORLEVEL%