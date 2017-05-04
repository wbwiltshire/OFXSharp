del *.nupkg
del lib\net35\*.dll
copy ..\source\OFXSharp\bin\x64\Debug\OFXSharp.dll lib\net35\OFXSharp.dll
..\source\.nuget\nuget.exe pack OFXSharp.nuspec
pause