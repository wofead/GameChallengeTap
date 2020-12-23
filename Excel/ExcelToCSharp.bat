tabtoy.exe -mode=v3 -index=Index.xlsx -package=main -csharp_out=table_gen.cs -binary_out=table_gen.bin
copy table_gen.cs %cd%\..\Rainbow\Assets\Scripts\Common\ExcelGen\table_gen.cs /y
copy table_gen.bin %cd%\..\Rainbow\Assets\Resources\Excel\table_gen.bin /y
pause