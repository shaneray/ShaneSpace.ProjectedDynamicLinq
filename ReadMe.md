ShaneSpace.ProjectedDynamicLinq
===============================
This is a modified copy of the Microsoft assembly for the .Net 4.0 Dynamic language functionality to work with Automapper and projections.

All functionality of the library should still work.  Only addition made is to allow this to work with Automapper and Projection.
When using a Projected model simply call IQueryable<TSource>.Where<TSource, TDest>("string expression") where *TDest* is the model you are projecting/mapping to. 

Documentation and Samples
-------------------------
You can find sample code and documentation on usage for the original library from this link, just Accept the terms and you will download a Visual Studio file with C# code and HTML documentation.

http://msdn.microsoft.com/en-US/vstudio/bb894665.aspx