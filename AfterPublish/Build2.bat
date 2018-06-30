
SET NUGET="C:\Projects\buni-website\Tools\cake-tools\nuget.exe"
SET MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"

%NUGET% restore

REM <!-- <Target Name="GatherAllFilesToPublish" DependsOnTargets="PipelineTransformPhase;CopyAllFilesToSingleFolderForPackage;" /> -->

REM %MSBUILD%	AfterPublishTarget.sln /t:Build,Package


REM _PackageTempDir
REM t Package vs WebPublish

REM "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" AfterPublishTarget.sln ^
REM /p:DeployOnBuild=true ^
REM /p:DeployTarget=Package ^
REM /p:_PackageTempDir=pppp ^
REM /p:PackageAsSingleFile=False ^
REM /p:AutoParameterizationWebConfigConnectionStrings=false


REM IT APPEARS THAT THAT SPECIFYING _PackageTempDir to be the same location as PublishUrl actually
REM prevents one more set of copies :)

%MSBUILD%	AfterPublishTarget.sln /T:BUILD /M /DETAILEDSUMMARY /VERBOSITY:NORMAL ^
			/p:Configuration=Release ^
            /p:DeployOnBuild=True ^
            /p:DeployDefaultTarget=WebPublish ^
            /p:WebPublishMethod=FileSystem ^
            /p:DeleteExistingFiles=False ^
			/p:publishUrl="%cd%\_\Publish" ^
			/p:_PackageTempDir="%cd%\_\Publish" ^
			/p:_FindDependencies=False ^
			/p:MSDeployUseChecksum=True
			
REM http://www.zvolkov.com/clog/2010/05/18/how-to-packagepublish-web-site-project-using-vs2010-and-msbuild/
			