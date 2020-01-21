If you don't have Microsoft.Net, which is part of the standard operating system update by Microsoft. Download it from here. 
https://support.microsoft.com/en-us/help/4486153/microsoft-net-framework-4-8-on-windows-10-version-1709-windows-10-vers

/*------------TO COMPILE THIS PROGRAM------------*/

Click the MakeFile.cmd

/*------------DETAILS OF THE MakeFile.cmd------------*/

Content of the MakeFile.cmd file:

// Display the executed commands

@echo on

// Set the path to Framework to be able to use csc

path=C:\Windows\Microsoft.NET\Framework\v4.0.30319

// Utilize csc to compile c# .cs file with reference to the libraries used in the code into ws.exe

csc /r:System.Net.Http.dll,System.dll,System.Collections.dll /out:ws.exe *.cs

// Start a new command console for executing the created .exe file

start cmd /k echo Enter command: ws.exe "url" "number of hops". (Example: ws.exe http://courses.washington.edu/css502/dimpsey 5)

/*------------LINKS THAT WAS USED TO TEST THE WEBSCRAPER------------*/

http://courses.washington.edu/css502/dimpsey

http://courses.washington.edu/css342/dimpsey