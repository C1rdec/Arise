namespace Arise.Console
{
    using Core.Services;

    class Program
    {
        static void Main(string[] args)
        {
            var googleDriveService = new GoogleDriveService();
            var file = googleDriveService.CreateFolder("Arise").Result;
            var file2 = googleDriveService.CreateFolder("Folder1", file.Id).Result;

            var file3 = googleDriveService.CreateFolder("Folder3", file2.Id);
        }
    }
}
