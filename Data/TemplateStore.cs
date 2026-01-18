using System;
using System.Collections.Generic;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;

namespace AutoAssignCharacterSheetWithTemplate.Data;

public sealed class TemplateStore
{
    private static readonly Lazy<TemplateStore> _lazy = new Lazy<TemplateStore>(() =>
    {
        var s = new TemplateStore();
        s.LoadFromDisk();
        return s;
    });

    public static TemplateStore Instance => _lazy.Value;
    public List<TemplateManagerCharacter> TemplateList { get; private set; }

    private TemplateStore()
    {
        TemplateList = new List<TemplateManagerCharacter>();
    }

    public void LoadFromDisk()
    {
        TemplateList = TemplateSaveManager.LoadSavedTemplatesList() ?? new List<TemplateManagerCharacter>();
    }
}