using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

public class HairRecommendationController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public HairRecommendationController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View("Upload");
    }

    [HttpPost]
    public async Task<IActionResult> GetRecommendation(IFormFile ImageFile)
    {
        if (ImageFile == null || ImageFile.Length == 0)
        {
            ViewBag.ErrorMessage = "Lütfen bir resim yükleyin.";
            return View("Upload");
        }

        // Resmi geçici olarak saklama
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "temp");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{ImageFile.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await ImageFile.CopyToAsync(stream);
        }

        // Yapay zeka API'sine çağrı
        try
        {
            var apiResponse = await CallHairRecommendationApi(filePath);
            ViewBag.RecommendationResult = apiResponse;
        }
        catch
        {
            ViewBag.ErrorMessage = "Yapay zeka servisiyle bağlantı kurulamadı. Lütfen daha sonra tekrar deneyin.";
        }
        finally
        {
            // Geçici dosyayı sil
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        return View("Upload");
    }

    private async Task<string> CallHairRecommendationApi(string imagePath)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var apiEndpoint = "https://your-ai-api-endpoint.com/recommend";
        var fileContent = new ByteArrayContent(await System.IO.File.ReadAllBytesAsync(imagePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

        using var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "image", Path.GetFileName(imagePath));

        var response = await httpClient.PostAsync(apiEndpoint, formData);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}