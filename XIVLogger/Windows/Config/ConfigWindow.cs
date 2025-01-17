using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using XIVLogger.Resources;

namespace XIVLogger.Windows.Config;

public partial class ConfigWindow : Window, IDisposable
{
    private readonly Plugin Plugin;

    public ConfigWindow(Plugin plugin) : base("Configuration##XIVLogger")
    {
        Plugin = plugin;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(560, 520),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose() { }

    public override void Draw()
    {
        var aboutOpen = false;
        var buttonHeight = ImGui.GetFrameHeightWithSpacing() + ImGui.GetStyle().WindowPadding.Y + GetSeparatorPaddingHeight;
        using (var contentChild = ImRaii.Child("Content", new Vector2(0, -buttonHeight)))
        {
            if (contentChild)
            {
                using var tabBar = ImRaii.TabBar("##ConfigTabBar");
                if (!tabBar.Success)
                    return;

                Settings();

                ChatTypes();

                Advanced();

                aboutOpen = About();
            }
        }

        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(1.0f);

        using var bottomChild = ImRaii.Child("BottomBar", new Vector2(0, 0), false, 0);
        if (bottomChild)
        {
            if (aboutOpen)
            {
                DrawAboutButtons();
            }
            else
            {
                if (ImGui.Button(Language.ButtonSaveConfig))
                    Plugin.Configuration.Save();

                ImGui.SameLine();

                if (ImGui.Button(Language.ButtonSaveLog))
                {
                    var latestLogTime = Plugin.ChatLog.PrintLog("");
                    Plugin.Notification.AddNotification(new Notification {Content = string.Format(Language.NotificationLogSaved, latestLogTime), Type = NotificationType.Success});
                    Plugin.Configuration.Save();
                }

                ImGui.SameLine();

                if (ImGui.Button(Language.ButtonSaveClipboard))
                {
                    var clip = Plugin.ChatLog.PrintLog("", aClipboard: true);
                    ImGui.SetClipboardText(clip);
                    Plugin.Notification.AddNotification(new Notification {Content = Language.NotificationLogCopy, Type = NotificationType.Success});
                    Plugin.Configuration.Save();
                }
            }
        }
    }
}
