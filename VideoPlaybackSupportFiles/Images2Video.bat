rem Commandline test string = Images2Video Nathan "C:\Temp\Scope Project_Img_test\\" "C:\Temp\Scope Project_Img_test\<Export #>\ipImageResultDisp\"
rem - example export1 == Images2Video Nathan "C:\Temp\Scope Project_Img_test\\" "C:\Temp\Scope Project_Img_test\Export 1\ipImageResultDisp\"

set videofilename=%1
set videodestinationpath=%2
set scopeimageexportpath=%3


echo %videofilename%
echo %videodestinationpath%
echo %scopeimageexportpath%


rem for /r %%i in (*.jpg) do (
    rem ffmpeg -framerate 10 -i %%i.jpg -c:v libx264 -r 30 -pix_fmt yuv420p out.mp4
rem )

echo off
color 3

rem //added the arguments to the two vars below
set a=%scopeimageexportpath:~1,-1%
set a="%a%*.jpg"
set b=%videodestinationpath:~1,-1%%videofilename:~1,-1%
set b="%b%.mp4"
set bootfolder=C:\TwinCAT\3.1\boot\

set c="%bootfolder%ffmpeg"
set f=-c:v libx264 -r 60 -framerate 10 -pix_fmt yuvj420p -framerate 10
rem (was at end of previous line) -crf 

set tmp="%TEMP%\list.tmp"



for %%f in (%a%) do (@echo file 'file:%%f' >> %tmp%)
%c% -y -f concat -safe 0 -i %tmp% -framerate 10 %f% %b%
del /f /q %tmp%

rmdir /Q /S %scopeimageexportpath%
rmdir /Q /S %scopeimageexportpath:~1,-1%


exit
