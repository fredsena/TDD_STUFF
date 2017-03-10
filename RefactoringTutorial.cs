
//Credits:
//http://softwareengineering.stackexchange.com/questions/186218/helper-static-methods-in-tdd

//To say that they're not testable is inaccurate. They're very testable, but they're not mockable and thus they are not test-friendly. That is, every time you test a unit of code that calls this method, you have to test the method. If the method is ever broken, many tests will fail and it won't be obvious why.

//Make it a non-static class, extract an interface with all the methods in and pass your helper into every class that needs it, preferably through the constructor, and preferably using an IOC Container.

public class FileHelper : IFileHelper
{
    public void ExtractZipFile(Stream zipStream, string location)
    {
        ..................................
    }

    public void CreatePageFolderIfNotExist(string directory)
    {
        .................................................
    }

    .......................................................
    .......................................................
}

public interface IFileHelper
{
    void ExtractZipFile(Stream zipStream, string location);
    void CreatePageFolderIfNotExist(string directory);

    .......................................................
    .......................................................
}

public class MyClass
{
     private readonly IFileHelper _fileHelper;

     public MyClass(IFileHelper fileHelper)
     {
         _fileHelper = fileHelper;
     }
}
