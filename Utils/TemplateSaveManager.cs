using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoAssignCharacterSheetWithTemplate.Models;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace AutoAssignCharacterSheetWithTemplate.Utils;

public class TemplateSaveManager
{
    private static readonly string SavesFolderName = "SavedTemplates";

    private static string SanitizeFileName(string name)
    {
        var invalid = new string(Path.GetInvalidFileNameChars());
        var regex = new Regex($"[{Regex.Escape(invalid)}]");
        var cleaned = regex.Replace(name, "_").Trim();
        if (string.IsNullOrEmpty(cleaned)) cleaned = "template";
        return cleaned;
    }

    private static string GetModFolder()
    {
        var asmPath = Assembly.GetExecutingAssembly().Location;
        return Path.GetDirectoryName(asmPath);
    }

    private static string GetSavesFolder()
    {
        var modFolder = GetModFolder();
        var savesFolder = Path.Combine(modFolder, SavesFolderName);
        if (!Directory.Exists(savesFolder)) Directory.CreateDirectory(savesFolder);
        return savesFolder;
    }

    public static void SaveTemplate(TemplateCharacterDto data)
    {
        try
        {
            var saveFolder = GetSavesFolder();
            string fileName = SanitizeFileName(data.Name) + ".json";
            string path = Path.Combine(saveFolder, fileName);
            TM_Log.Info($"Saving {path}");
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        catch (JsonException e)
        {
            TM_Log.Error(e.Message);
            throw;
        }
    }

    public static TemplateCharacterDto LoadTemplateFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<TemplateCharacterDto>(json);
    }

    public static string[]? ListSavedTemplates()
    {
        var folder = GetSavesFolder();
        if (!Directory.Exists(folder)) return null;
        return Directory.GetFiles(folder, "*.json");
    }
}