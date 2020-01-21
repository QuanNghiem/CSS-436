@echo on

path=C:\Windows\Microsoft.NET\Framework\v4.0.30319

csc /r:System.Net.Http.dll,System.dll,System.Collections.dll /out:WebScraper.exe *.cs

start cmd /k echo Enter command: WebScraper.exe "url" "number of hops". (Example: WebScraper.exe http://courses.washington.edu/css502/dimpsey 5)

Pause



