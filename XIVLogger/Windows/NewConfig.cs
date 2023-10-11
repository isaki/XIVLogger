using Dalamud.Interface.Windowing;

namespace XIVLogger.Windows;

public class NewConfigWindow : Window, IDisposable
{
    private Plugin Plugin;
    public ChatConfig? SelectedConfig = null;

    public NewConfigWindow(Plugin plugin) : base("New Configuration##XIVLogger")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (SelectedConfig == null)
        {
            ImGui.TextUnformatted("No config selected...");
            return;
        }

        if (ImGui.Button("Set as Active Chat Configuration"))
            Plugin.Configuration.SetActiveConfig(SelectedConfig);

        ImGui.Text("Config Name:");
        ImGui.SameLine();
        ImGui.InputText("##confname", ref SelectedConfig.name, 256);
        ImGui.Spacing();

        if (ImGui.BeginTabBar("##indie tabs"))
        {
            if (ImGui.BeginTabItem("Chat Config"))
            {
                foreach (KeyValuePair<int, string> entry in Plugin.Configuration.PossibleChatTypes)
                {
                    var enabled = SelectedConfig.TypeConfig[entry.Key];
                    if (ImGui.Checkbox($"{entry.Value}", ref enabled))
                    {
                        SelectedConfig.TypeConfig[entry.Key] = enabled;
                        Plugin.Configuration.Save();
                    }

                }
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }
}
