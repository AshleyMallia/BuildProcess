
SET NUGET="C:\Projects\buni-website\Tools\cake-tools\nuget.exe"
SET MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"

%NUGET% restore

REM <!-- <Target Name="GatherAllFilesToPublish" DependsOnTargets="PipelineTransformPhase;CopyAllFilesToSingleFolderForPackage;" /> -->

REM THE HOLY GRAIL OF PUBLISHING A WEB APPLICATION TO A FOLDER
REM THIS COMBINATION OF ARGUMENTS:
REM 1. PERFORMS AN INCREMENTAL BUILD
REM 2. PUBLISES ASP WEB APPLICATION FILES TO %CD%\BUILT
REM 3. ONLY WRITES TO BUILT FOLDER WHEN CHANGES OCCUR

%MSBUILD%	References.sln /T:BUILD,PipelinePreDeployCopyAllFilesToOneFolder /M /DETAILEDSUMMARY /VERBOSITY:DIAGNOSTIC ^
			/p:Configuration=Release ^
			/p:_FindDependencies=False > build3.txt
			
REM  <!-- In Project disable the Transform and Parameterization features of WPP -->
REM  <Target Name="TransformWebConfig" />
REM  <Target Name="PipelineMsdeploySpecificTransformPhase" />

