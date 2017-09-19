SET MSBUILD=C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe

REM SET BUILD=Debug
SET BUILD=Release

COPY ..\src\ServiceStack.Text\bin\%BUILD%\portable45-net45+win8\ServiceStack.Text.*  ..\..\ServiceStack\lib\pcl
COPY ..\src\ServiceStack.Text\bin\%BUILD%\sl5\ServiceStack.Text.*	..\..\ServiceStack\lib\sl5
COPY ..\src\ServiceStack.Text\PclExport.Net40.cs		..\..\ServiceStack\src\ServiceStack.Pcl.Android\
COPY ..\src\ServiceStack.Text\PclExport.Net40.cs		..\..\ServiceStack\src\ServiceStack.Pcl.Ios10\
COPY ..\src\ServiceStack.Text\PclExport.Net40.cs		..\..\ServiceStack\src\ServiceStack.Pcl.Mac20\
COPY ..\src\ServiceStack.Text\PclExport.Net40.cs		..\..\ServiceStack\src\ServiceStack.Pcl.Net45\
COPY ..\src\ServiceStack.Text\PclExport.WinStore.cs		..\..\ServiceStack\src\ServiceStack.Pcl.WinStore\
COPY ..\src\ServiceStack.Text\PclExport.WinStore.cs		..\..\ServiceStack\src\ServiceStack.Pcl.WinStore81\

COPY ..\src\ServiceStack.Text\Pcl.*				..\..\ServiceStack\src\ServiceStack.Pcl.Android\
COPY ..\src\ServiceStack.Text\Pcl.*				..\..\ServiceStack\src\ServiceStack.Pcl.Ios10\
COPY ..\src\ServiceStack.Text\Pcl.*				..\..\ServiceStack\src\ServiceStack.Pcl.Mac20\
COPY ..\src\ServiceStack.Text\Pcl.*				..\..\ServiceStack\src\ServiceStack.Pcl.Net45\
COPY ..\src\ServiceStack.Text\Pcl.*				..\..\ServiceStack\src\ServiceStack.Pcl.WinStore\

