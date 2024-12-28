using System.Net.Http.Headers;
using System.Text.Json;
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
    public async Task<IActionResult> GetRecommendation(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            ViewBag.ErrorMessage = "Lütfen bir resim yükleyin.";
            return View("Upload");
        }

        // Resmi geçici olarak saklama
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "temp");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // OpenAI API'ye çağrı
            var apiKey = "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"; // Replace with your actual API key

            var recommendation = await CallOpenAiApi(apiKey, filePath);

            // Öneriyi kullanıcıya göster
            ViewBag.RecommendationResult = recommendation;
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Hata: {ex.Message}";
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

    private static async Task<string> CallOpenAiApi(string apiKey, string imagePath)
    {
        using var httpClient = new HttpClient();

        // OpenAI API başlığına yetkilendirme ekleyin
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        // Resmi Base64'e çevirin
        var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
        var base64Image = Convert.ToBase64String(imageBytes);

        // API için JSON verisi hazırlayın
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = "Bu resimdeki kişiye uygun olacak saç modellerini listeler misin. Her bir öneriyi bir line olarak paylaş benimle"
                },
                new
                {
                    role = "assistant",
                    content = $"data:image/jpeg;base64,{base64Image}"
                }
            }
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        // OpenAI API'ye POST isteği gönder
        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenAI API Hatası: {response.StatusCode} - {errorContent}");
        }

        // Yanıtı JSON formatında işle
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return ExtractRecommendation(jsonResponse);
    }

    private static string ExtractRecommendation(string jsonResponse)
    {
        try
        {
            using var document = JsonDocument.Parse(jsonResponse);
            var root = document.RootElement;

            // Yanıttan öneriyi al
            var choices = root.GetProperty("choices");
            if (choices.GetArrayLength() > 0)
            {
                var content = choices[0].GetProperty("message").GetProperty("content").GetString();
                return content ?? "Öneri bulunamadı.";
            }
            return "Yanıt işlenemedi.";
        }
        catch
        {
            return "Yanıt işlenemedi.";
        }
    }
}
