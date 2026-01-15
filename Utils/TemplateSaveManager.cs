using System;
using System.Collections.Generic;
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

    public static List<TemplateManagerCharacter> LoadSavedTemplatesList()
    {
        var templateManagerCharacterList = new List<TemplateManagerCharacter>();
        var files = ListSavedTemplates();
        if (files == null) return templateManagerCharacterList;
        foreach (var file in files)
        {
            var dto = LoadTemplateFromFile(file);
            TemplateManagerCharacter template = new TemplateManagerCharacter();
            template.CreateNewTemplate();
            templateManagerCharacterList.Add(template);
            dto.ApplyToModel(template);
        }

        return templateManagerCharacterList;
    }

    public static void DeleteTemplate(string fileName)
    {
        try
        {
            var folder = GetSavesFolder();
            var path = Path.Combine(folder, fileName + ".json");

            if (!File.Exists(path))
            {
                TM_Log.Error($"File {path} doesn't exist");
            }

            File.Delete(path);
        }
        catch (IOException e)
        {
            TM_Log.Error($"IO error while deleting template: {e.Message}");
        }
    }

    public static Tuple<bool, string> CheckIfNameExists(string fileName)
    {
        var folder = GetSavesFolder();
        var path = Path.Combine(folder, fileName + ".json");
        if (!File.Exists(path))
        {
            return new Tuple<bool, string>(true, "");
        }
        return new Tuple<bool, string>(false, "Name already exists");
    }

    public static void RenameSaveFile(string newFileName, string oldFileName)
    {
        var folder = GetSavesFolder();

        string oldPath = Path.Combine(folder, SanitizeFileName(oldFileName) + ".json");
        string newPath = Path.Combine(folder, SanitizeFileName(newFileName) + ".json");

        if (!File.Exists(oldPath))
        {
            TM_Log.Error($"Le fichier à renommer n'existe pas : {oldPath}");
            return;
        }

        try
        {
            string json = File.ReadAllText(oldPath);

            var data = JsonConvert.DeserializeObject<TemplateCharacterDto>(json);

            data.Name = newFileName;

            string newJson = JsonConvert.SerializeObject(data, Formatting.Indented);

            File.WriteAllText(newPath, newJson);

            File.Delete(oldPath);

        }
        catch (Exception e)
        {
            TM_Log.Error($"Erreur lors du renommage : {e.Message}");
        }
    }

}