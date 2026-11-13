using Bannerlord.UIExtenderEx.Prefabs2;
using Bannerlord.UIExtenderEx.Attributes;

namespace AutoAssignCharacterSheetWithTemplate.PrefabExtensionInsertPatch;

[PrefabExtension("CharacterDeveloper", "/Prefab/Window/Widget/Children")]
public class CharacterDeveloperPatchInsertButton : Bannerlord.UIExtenderEx.Prefabs2.PrefabExtensionInsertPatch
{
    public override InsertType Type => InsertType.Child;

    public override int Index => int.MaxValue;

    [PrefabExtensionText]
    public string Content => """
                             <ButtonWidget
                                     Id="TemplatesCharacterButton"
                                     DoNotPassEventsToChildren="true"
                                     WidthSizePolicy="Fixed"
                                     HeightSizePolicy="Fixed"
                                     SuggestedWidth="180"
                                     SuggestedHeight="40"
                                     HorizontalAlignment="Right"
                                     VerticalAlignment="Top"
                                     MarginRight="30"
                                     MarginTop="30"
                                     Brush="ButtonBrush1"
                                     Command.Click="ExecuteOpenTemplateManager">
                               <Children>
                                 <RichTextWidget WidthSizePolicy="StretchToParent" HeightSizePolicy="StretchToParent" Brush="OverlayPopup.ButtonText" Brush.FontSize="20" Text="@TitleTemplateText"/>
                               </Children>
                             </ButtonWidget>
                             """;
}