set DeployDir=2017.2
rmdir /s /q Deploy
rmdir /s /q bin
"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" Build.targets
mkdir Deploy\%DeployDir%
"%USERPROFILE%\.nuget\packages\NuGet.CommandLine\4.3.0\tools\nuget.exe" pack GammaJul.ReSharper.ForTea\GammaJul.ReSharper.ForTea.nuspec -NoPackageAnalysis -OutputDirectory Deploy\%DeployDir%
