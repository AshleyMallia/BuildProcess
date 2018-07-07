for /d /r . %%d in (bin,obj,.vs,tempdir) do @if exist "%%d" rd /s/q "%%d"

rd /s/q obj
rd /s/q bin
rd /s/q output