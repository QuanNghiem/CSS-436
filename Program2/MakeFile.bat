@echo on

path=C:\Windows\Microsoft.NET\Framework\v4.0.30319

set p=%cd%

csc /r:"%p%\System.Net.Http.dll","%p%\Newtonsoft.Json.dll","%p%\System.Threading.Tasks.dll" /out:prog2.exe *.cs

start cmd /k echo Enter command: prog2.exe "city name". (Example: prog2.exe new York)

Pause



