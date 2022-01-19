
set videofilename=%1
set videodestinationpath=%2
set scopeimageexportpath=%3

rem for /r %%i in (*.jpg) do (
	rem ffmpeg -framerate 10 -i %%i.jpg -c:v libx264 -r 30 -pix_fmt yuv420p out.mp4
rem )

echo off
color 3
set a="*.jpg"
set b=%videofilename%".mp4"
set c=ffmpeg
set f=-c:v libx264 -r 60 -framerate 10 -pix_fmt yuvj420p -framerate 10

rem (was at end of previous line) -crf 


set tmp="list.tmp"

del %b%

for %%f in (%a%) do (@echo file 'file:%cd%\%%f' >> %tmp%)
%c% -y -f concat -safe 0 -i %tmp% -framerate 10 %f% %b%


rem cd /d Result
del /f /q list.tmp
rem exit