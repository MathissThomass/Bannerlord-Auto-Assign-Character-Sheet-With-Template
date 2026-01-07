using System;
using System.IO;
using TaleWorlds.Library;
using Debug = System.Diagnostics.Debug;

namespace AutoAssignCharacterSheetWithTemplate.Utils;

public class TM_Log
{
    public static void Info(string msg)
    {
        InformationManager.DisplayMessage(new InformationMessage($"[TemplateManager] {msg}"));
        Debug.Print($"[TemplateManager] {msg}");
        File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TemplateManagerLog.txt"), msg + Environment.NewLine);
    }

    public static void Error(string msg)
    {
        InformationManager.DisplayMessage(new InformationMessage($"[TemplateManager] {msg}", Colors.Red));
        Debug.Print($"[TemplateManager][ERROR] {msg}");
        File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TemplateManagerLog.txt"), msg + Environment.NewLine);
    }
}