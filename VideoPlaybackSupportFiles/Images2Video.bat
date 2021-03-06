rem Commandline test string = Images2Video Nathan "C:\Temp\Scope Project_Img_test\\" "C:\Temp\Scope Project_Img_test\<Export #>\ipImageResultDisp\"
rem - example export1 == Images2Video Nathan "C:\Temp\Scope Project_Img_test\\" "C:\Temp\Scope Project_Img_test\Export 1\ipImageResultDisp\"

set videofilename=%1
set videodestinationpath=%2
set scopeimageexportpath=%3


echo %videofilename%
echo %videodestinationpath%
echo %scopeimageexportpath%

echo off
color 3

rem //added the arguments to the two vars below
set a=%scopeimageexportpath:~1,-1%
set a="%a%*.jpg"
set b=%videodestinationpath:~1,-1%%videofilename:~1,-1%
echo %b%
set b="%b%.mp4"
echo %b%
set bootfolder=C:\TwinCAT\3.1\boot\

set c="%bootfolder%ffmpeg"
set f=-c:v libx264 -pix_fmt yuvj420p -r 10

set tmp="%TEMP%\list.tmp"

for %%f in (%a%) do (@echo file 'file:%%f' >> %tmp%)
%c% -f concat -safe 0 -r 10  -i %tmp% %f% %b%
del /f /q %tmp%

RMDIR /S /Q %scopeimageexportpath%

exit

