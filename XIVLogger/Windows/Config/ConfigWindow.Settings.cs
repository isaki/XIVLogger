using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using XIVLogger.Resources;

namespace XIVLogger.Windows.Config;

public partial class ConfigWindow
{
    private void Settings()
    {
        using var tabItem = ImRaii.TabItem(Language.TabSettings);
        if (!tabItem.Success)
            return;

        var save = false;
        var longText = Language.TabSettingsFileName;
        var width = ImGui.CalcTextSize(longText).X + (25.0f * ImGuiHelpers.GlobalScale);

        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted(longText);
        ImGui.SameLine(width);
        ImGui.InputText("##filename", ref Plugin.Configuration.fileName, 256);

        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted(Language.TabSettingsFilePath);
        ImGui.SameLine(width);
        ImGui.InputText("##filepath", ref Plugin.Configuration.filePath, 256);
        ImGui.TextUnformatted(Language.TabSettingsPathTooltip);

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        save |= ImGui.Checkbox(Language.TabSettingsIgnoreChatTypes, ref Plugin.Configuration.StoreEveryMessage);
        ImGuiComponents.HelpMarker(Language.TabSettingsIgnoreChatTypesTooltip);

        save |= ImGui.Checkbox(Language.TabSettingsTimestamp, ref Plugin.Configuration.fTimestamp);
        if (Plugin.Configuration.fTimestamp)
        {
            using var indent = ImRaii.PushIndent(10.0f);
            save |= ImGui.Checkbox(Language.TabSettingsUse24h, ref Plugin.Configuration.f24hTimestamp);
            if (Plugin.Configuration.f24hTimestamp)
            {
                using var innerIndent = ImRaii.PushIndent(10.0f);
                save |= ImGui.Checkbox(Language.TabSettingsShowSeconds, ref Plugin.Configuration.fTimeSeconds);
            }
            save |= ImGui.Checkbox(Language.TabSettingsIncludeTimestamp, ref Plugin.Configuration.fDatestamp);
        }

        save |= ImGui.Checkbox(Language.TabSettingsSortableFilenames, ref Plugin.Configuration.fileSortableDatetime);

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        if (ImGui.Checkbox(Language.TabSettingsAutosave, ref Plugin.Configuration.fAutosave))
        {
            Plugin.ChatLog.SetupAutosave();
            Plugin.ChatLog.AutoSave();

            Plugin.Configuration.UpdateAutosaveTime();
            Plugin.Configuration.Save();
        }
        save |= ImGui.Checkbox(Language.TabSettingsAutosaveNotification, ref Plugin.Configuration.fAutosaveNotif);

        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted(Language.TabSettingsEvery);
        ImGui.SameLine();

        var currentAutoMin = Plugin.Configuration.fAutoMin;
        ImGui.InputInt("##autosavemin", ref currentAutoMin, 1);
        if (currentAutoMin != Plugin.Configuration.fAutoMin)
        {
            Plugin.Configuration.fAutoMin = Math.Clamp(currentAutoMin, 1, int.MaxValue);
            Plugin.Configuration.Save();
        }

        ImGui.SameLine();
        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted(Language.TabSettingsMinutes);

        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted(Language.TabSettingsAutosavePath);
        ImGui.SameLine();
        ImGui.InputText("##autofilepath", ref Plugin.Configuration.autoFilePath, 256);

        if (save)
            Plugin.Configuration.Save();
    }
}
