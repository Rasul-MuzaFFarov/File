namespace WebApi.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }
    public string CreateFile(string folder, IFormFile file)
    {
        var path = Path.Combine(_environment.WebRootPath, folder,file.FileName);
        using (var stream = new FileStream(path, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return file.FileName;
    }

    public bool DeleteFile(string folder, string filename)
    {
        var path = Path.Combine(_environment.WebRootPath, folder, filename);
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        else
        {
            return false;
        }

    }
}