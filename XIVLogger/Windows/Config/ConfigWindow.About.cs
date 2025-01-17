using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using XIVLogger.Resources;

namespace XIVLogger.Windows.Config;

public partial class ConfigWindow
{
    private const float SeparatorPadding = 1.0f;
    private static float GetSeparatorPaddingHeight => SeparatorPadding * ImGuiHelpers.GlobalScale;

    private static bool About()
    {
        using var tabItem = ImRaii.TabItem(Language.TabAbout);
        if (!tabItem.Success)
            return false;

        ImGui.TextUnformatted(Language.AboutVersion);
        ImGui.SameLine();
        ImGui.TextColored(ImGuiColors.ParsedGold, Plugin.PluginInterface.Manifest.Author);

        ImGui.TextUnformatted(Language.AboutDiscord);
        ImGui.SameLine();
        ImGui.TextColored(ImGuiColors.ParsedGold, "@infi");

        ImGui.TextUnformatted(Language.AboutVersion);
        ImGui.SameLine();
        ImGui.TextColored(ImGuiColors.ParsedOrange, Plugin.PluginInterface.Manifest.AssemblyVersion.ToString());

        return true;
    }

    private void DrawAboutButtons()
    {
        using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.ParsedBlue))
        {
            if (ImGui.Button(Language.ButtonDiscord))
                Dalamud.Utility.Util.OpenLink("https://discord.com/channels/581875019861328007/1161515882690904074");
        }

        ImGui.SameLine();

        using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DPSRed))
        {
            if (ImGui.Button(Language.ButtonIssues))
                Dalamud.Utility.Util.OpenLink("https://github.com/Infiziert90/XIVLogger/issues");
        }

        ImGui.SameLine();

        using (ImRaii.PushColor(ImGuiCol.Button, new Vector4(0.12549f, 0.74902f, 0.33333f, 0.6f)))
        {
            if (ImGui.Button(Language.ButtonKoFi))
                Dalamud.Utility.Util.OpenLink("https://ko-fi.com/infiii");
        }
    }
}
