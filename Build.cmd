set DeployDir=2016.3
rmdir /s /q Deploy
rmdir /s /q bin
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" Build.targets
mkdir Deploy\%DeployDir%
packages\NuGet.CommandLine.3.4.3\tools\NuGet.exe pack GammaJul.ReSharper.ForTea\GammaJul.ReSharper.ForTea.nuspec -NoPackageAnalysis -OutputDirectory Deploy\%DeployDir%
