using Dalamud.Interface.Colors;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

namespace XIVLogger.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Plugin Plugin;

    public ConfigWindow(Plugin plugin) : base("Configuration##XIVLogger")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 460),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var about = false;
        if (ImGui.BeginTabBar("##ConfigTabBar"))
        {
            GeneralTab();

            ChatTypes();

            Advanced();

            about |= About();

            ImGui.EndTabBar();
        }

        if (about)
            return;

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        if (ImGui.Button("Save Config"))
            Plugin.Configuration.Save();

        ImGui.SameLine();

        if (ImGui.Button("Save Log"))
        {
            var latestLogTime = Plugin.ChatLog.PrintLog("");
            Plugin.PluginInterface.UiBuilder.AddNotification($"Log saved at {latestLogTime}", "[XIVLogger]", NotificationType.Success);
            Plugin.Configuration.Save();
        }

        ImGui.SameLine();

        if (ImGui.Button("Copy To Clipboard"))
        {
            var clip = Plugin.ChatLog.PrintLog("", aClipboard: true);
            ImGui.SetClipboardText(clip);
            Plugin.PluginInterface.UiBuilder.AddNotification("Log copied to clipboard", "[XIVLogger]", NotificationType.Success);
            Plugin.Configuration.Save();
        }
    }

    private void GeneralTab()
    {
        if (!ImGui.BeginTabItem("Config"))
            return;

        var save = false;
        var longText = "File Name:";
        var width = ImGui.CalcTextSize(longText).X + (25.0f * ImGuiHelpers.GlobalScale);

        ImGuiHelpers.ScaledDummy(5.0f);

        ImGui.AlignTextToFramePadding();
        ImGui.Text(longText);
        ImGui.SameLine(width);
        ImGui.InputText("##filename", ref Plugin.Configuration.fileName, 256);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("File Path:");
        ImGui.SameLine(width);
        ImGui.InputText("##filepath", ref Plugin.Configuration.filePath, 256);
        ImGui.Text("Default: Documents folder");

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        save |= ImGui.Checkbox("Include Timestamp", ref Plugin.Configuration.fTimestamp);

        ImGuiHelpers.ScaledIndent(10.0f);
        if (Plugin.Configuration.fTimestamp)
        {
            save |= ImGui.Checkbox("Use 24h Time", ref Plugin.Configuration.f24hTimestamp);

            ImGuiHelpers.ScaledIndent(10.0f);
            if (Plugin.Configuration.f24hTimestamp)
                save |= ImGui.Checkbox("Show seconds", ref Plugin.Configuration.fTimeSeconds);
            ImGuiHelpers.ScaledIndent(-10.0f);
        }

        save |= ImGui.Checkbox("Include Datestamp", ref Plugin.Configuration.fDatestamp);
        ImGuiHelpers.ScaledIndent(-10.0f);

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        if (ImGui.Checkbox("Autosave", ref Plugin.Configuration.fAutosave))
        {
            Plugin.ChatLog.SetupAutosave();
            Plugin.ChatLog.AutoSave();

            Plugin.Configuration.UpdateAutosaveTime();
            Plugin.Configuration.Save();
        }
        save |= ImGui.Checkbox("Autosave notification echoed in chat?", ref Plugin.Configuration.fAutosaveNotif);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Every");
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
        ImGui.Text("minutes");

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Autosave File Path:");
        ImGui.SameLine();
        ImGui.InputText("##autofilepath", ref Plugin.Configuration.autoFilePath, 256);

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        save |= ImGui.Checkbox("Use sortable datetime in file names", ref Plugin.Configuration.fileSortableDatetime);

        if (save)
            Plugin.Configuration.Save();

        ImGui.EndTabItem();
    }

    private void ChatTypes()
    {
        if (ImGui.BeginTabItem("Chat Types"))
        {
            ImGui.Text("Default configuration settings");
            if (ImGui.BeginTable("configlist", 3, ImGuiTableFlags.BordersInnerH))
            {
                foreach (var entry in Plugin.Configuration.PossibleChatTypes)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    var enabled = Plugin.Configuration.defaultConfig.TypeConfig[entry.Key];
                    if (ImGui.Checkbox($"{entry.Value}", ref enabled))
                    {
                        Plugin.Configuration.defaultConfig.TypeConfig[entry.Key] = enabled;
                        Plugin.Configuration.Save();
                    }

                }

                ImGui.EndTable();
            }

            ImGui.EndTabItem();
        }
    }

    private void Advanced()
    {
        if (ImGui.BeginTabItem("Advanced Settings"))
        {

            ImGui.Text("Set up additional configurations with different combinations of chat types here.");

            ImGui.Spacing();

            if (ImGui.BeginTable("configlist", 4, ImGuiTableFlags.BordersInner))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();

                ImGui.Text("Active?");

                ImGui.TableNextColumn();

                ImGui.Text("Configuration Name");

                ImGui.TableNextColumn();

                ImGui.Text("Edit");

                ImGui.TableNextColumn();

                ImGui.Text("Remove");

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                if (ImGui.Button((Plugin.Configuration.defaultConfig.IsActive ? "@##active_default" : "##active_default"), new Vector2(20, 20)))
                    Plugin.Configuration.SetActiveConfig(Plugin.Configuration.defaultConfig);

                ImGui.TableNextColumn();
                ImGui.Text($"{Plugin.Configuration.defaultConfig.Name}");

                // list
                var index = 0;

                foreach (var config in Plugin.Configuration.configList.ToArray())
                {
                    index++;
                    var id = config.Name + index;

                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    if (ImGui.Button((config.IsActive ? "@##active_" + id : "##active_" + id), new Vector2(20, 20)))
                        Plugin.Configuration.SetActiveConfig(config);

                    ImGui.TableNextColumn();
                    ImGui.Text($"{config.Name}");


                    ImGui.TableNextColumn();
                    if (ImGui.Button("Edit##" + id))
                    {
                        Plugin.NewConfigWindow.SelectedConfig = config;
                        Plugin.NewConfigWindow.IsOpen = true;
                    }

                    ImGui.TableNextColumn();
                    if (ImGui.Button("Remove##" + id))
                    {
                        Plugin.Configuration.RemoveConfig(config);
                        Plugin.NewConfigWindow.IsOpen = false;
                    }
                }

                ImGui.TableNextRow();

                ImGui.TableNextColumn();

                if (ImGui.Button("+"))
                {
                    Plugin.Configuration.AddNewConfig("New Config");
                }

                ImGui.EndTable();

            }

            ImGui.EndTabItem();
        }
    }

    private static bool About()
    {
        var open = ImGui.BeginTabItem("About");
        if (open)
        {
            var buttonHeight = ImGui.CalcTextSize("RRRR").Y + (20.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginChild("AboutContent", new Vector2(0, -buttonHeight)))
            {
                ImGuiHelpers.ScaledDummy(5.0f);

                ImGui.TextUnformatted("Author:");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedGold, Plugin.Authors);

                ImGui.TextUnformatted("Discord:");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedGold, "@infi");

                ImGui.TextUnformatted("Version:");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedOrange, Plugin.Version);
            }
            ImGui.EndChild();

            ImGui.Separator();
            ImGuiHelpers.ScaledDummy(1.0f);

            if (ImGui.BeginChild("AboutBottomBar", new Vector2(0, 0), false, 0))
            {
                ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.ParsedBlue);
                if (ImGui.Button("Discord Thread"))
                    Dalamud.Utility.Util.OpenLink("https://canary.discord.com/channels/581875019861328007/1161515882690904074");
                ImGui.PopStyleColor();

                ImGui.SameLine();

                ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.DPSRed);
                if (ImGui.Button("Issues"))
                    Dalamud.Utility.Util.OpenLink("https://github.com/Infiziert90/XIVLogger/issues");
                ImGui.PopStyleColor();

                ImGui.SameLine();

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.12549f, 0.74902f, 0.33333f, 0.6f));
                if (ImGui.Button("Ko-Fi Tip"))
                    Dalamud.Utility.Util.OpenLink("https://ko-fi.com/infiii");
                ImGui.PopStyleColor();
            }
            ImGui.EndChild();

            ImGui.EndTabItem();
        }

        return open;
    }
}
