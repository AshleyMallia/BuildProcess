# [Reflection.Assembly]::LoadFile("Name of your dll")
# [Your.NameSpace.And.TypeName]::YourMethod()

# var t = Type.GetType("NameSpace.Type,Name Of Dll");
# var m = t.GetMethod("NameOfMethod");
# m.Invoke(null, new object[] { params });



[Reflection.Assembly]::LoadFile(“C:\_\BuildProcess\WebApplication2\bin\AfterPublishTarget.dll”)
[AfterPublishTarget.Class1]::GetAllCompilerInfo()


[Reflection.Assembly]::LoadFile(“C:\_\BuildProcess\WebApplication2\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll”)
