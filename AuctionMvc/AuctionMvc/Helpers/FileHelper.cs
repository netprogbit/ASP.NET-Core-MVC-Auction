using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AuctionMvc.Helpers
{
  public static class FileHelper
  {
    private static readonly string _imageFolderName = "Images";
    private static readonly string _defaultImageName = "default.png";

    public static IHostingEnvironment HostingEnvironment { get; set; }

    public static string GetUniqueFileName(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return null;

      return Guid.NewGuid().ToString() + Path.GetExtension(fileName);
    }

    public static string FilterImageName(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return _defaultImageName;

      return fileName;
    }

    public static string GetLinkNoImage()
    {
      return GetLinkImage(_defaultImageName);
    }

    public static string GetLinkImage(string fileName)
    {
      return "/" + _imageFolderName + "/" + fileName;
    }

    public static async Task AddImageAsync(IFormFile file, string newFileName)
    {
      if (file == null)
        return; // Default image will be used

      string imageFileName = FilterImageName(newFileName);
      string pathImageFile = GetPath(imageFileName);

      using (var fileStream = new FileStream(pathImageFile, FileMode.Create))
      {
        await file.CopyToAsync(fileStream);
      }
    }

    public static async Task UpdateImageAsync(IFormFile file, string oldFileName, string newFileName)
    {
      if (file == null)
        return; // Оld image will be used

      string pathImageFile = GetPath(newFileName);

      using (var fileStream = new FileStream(pathImageFile, FileMode.Create))
      {
        await file.CopyToAsync(fileStream);
      }

      if (oldFileName != newFileName)
        FileHelper.DeleteFile(oldFileName); // Detele old image file
    }

    public static void DeleteFile(string fileName)
    {
      if (string.IsNullOrEmpty(fileName) || fileName == _defaultImageName)
        return; // Default image not delete

      string fullFileName = Path.Combine(Path.Combine(HostingEnvironment.WebRootPath, _imageFolderName), fileName);
      FileInfo fileInf = new FileInfo(fullFileName);

      if (!fileInf.Exists)
        return;

      File.Delete(fullFileName);
    }

    private static string GetPath(string fileName)
    {
      return Path.Combine(Path.Combine(HostingEnvironment.WebRootPath, FileHelper._imageFolderName), fileName);
    }

  }
}
