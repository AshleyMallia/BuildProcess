
SET NUGET="C:\Projects\buni-website\Tools\cake-tools\nuget.exe"
SET MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"

%NUGET% restore

REM <!-- <Target Name="GatherAllFilesToPublish" DependsOnTargets="PipelineTransformPhase;CopyAllFilesToSingleFolderForPackage;" /> -->

REM THE HOLY GRAIL OF PUBLISHING A WEB APPLICATION TO A FOLDER
REM THIS COMBINATION OF ARGUMENTS:
REM 1. PERFORMS AN INCREMENTAL BUILD
REM 2. PUBLISES ASP WEB APPLICATION FILES TO %CD%\BUILT
REM 3. ONLY WRITES TO BUILT FOLDER WHEN CHANGES OCCUR

%MSBUILD%	References.sln /T:BUILD /M /DETAILEDSUMMARY /VERBOSITY:NORMAL ^
			/p:Configuration=Release ^
            /p:DeployOnBuild=True ^
            /p:DeployDefaultTarget=WebPublish ^
            /p:WebPublishMethod=FileSystem ^
            /p:DeleteExistingFiles=False ^
			/p:publishUrl="%cd%\built" ^
			/p:_PackageTempDir="tempdir" ^
			/p:MSDeployUseChecksum=True > build2.txt
			
REM  <!-- In Project disable the Transform and Parameterization features of WPP -->
REM  <Target Name="TransformWebConfig" />
REM  <Target Name="PipelineMsdeploySpecificTransformPhase" />
