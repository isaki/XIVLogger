using System.Reflection;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using XIVLogger.Windows;

namespace XIVLogger;

public class Plugin : IDalamudPlugin
{
    private const string CommandName = "/xivlogger";

    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static IChatGui Chat { get; private set; } = null!;
    [PluginService] public static IFramework Framework { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static INotificationManager Notification { get; private set; } = null!;

    public Configuration Configuration;
    public ChatStorage ChatLog;

    private WindowSystem WindowSystem = new("XIVLogger");
    private ConfigWindow ConfigWindow { get; init; }
    public NewConfigWindow NewConfigWindow { get; init; }

    public static readonly string Authors = "Infi, Cadaeix";
    public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize();

        ConfigWindow = new ConfigWindow(this);
        NewConfigWindow = new NewConfigWindow(this);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(NewConfigWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens settings window for XIVLogger"
        });

        CommandManager.AddHandler("/savelog", new CommandInfo(OnSaveCommand)
        {
            HelpMessage = "Saves a chat log as a text file with the current settings, /savelog <number> to save the last <number> messages"
        });

        CommandManager.AddHandler("/copylog", new CommandInfo(OnCopyCommand)
        {
            HelpMessage = "Copies a chat log to your clipboard with the current settings, /copylog <number> to copy the last <number> messages"
        });

        ChatLog = new ChatStorage(Configuration);

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;

        ClientState.Login += OnLogin;
        ClientState.Logout += OnLogout;
        Chat.ChatMessage += OnChatMessage;

        Framework.Update += OnUpdate;

        // Call it just to make sure a name is set, if login wasn't called
        ChatLog.SetupAutosave();
    }

    private void OnLogin()
    {
        ChatLog.SetupAutosave();
    }

    private void OnLogout()
    {
        if (!Configuration.fAutosave)
            return;

        ChatLog.AutoSave();
        ChatLog.WipeLog();
    }

    private void OnUpdate(IFramework framework)
    {
        if (!Configuration.fAutosave)
            return;

        if (!ClientState.IsLoggedIn)
            return;

        if (!Configuration.CheckTime())
            return;

        ChatLog.AutoSave();
        Configuration.UpdateAutosaveTime();
    }

    private void OnChatMessage(XivChatType type, int _, ref SeString sender, ref SeString message, ref bool handled)
    {
        ChatLog.AddMessage(type, sender.TextValue, message.TextValue);
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        CommandManager.RemoveHandler(CommandName);
        CommandManager.RemoveHandler("/savelog");
        CommandManager.RemoveHandler("/copylog");

        Framework.Update -= OnUpdate;
        Chat.ChatMessage -= OnChatMessage;
        ClientState.Login -= OnLogin;
        ClientState.Logout -= OnLogout;
    }

    private void OnCommand(string command, string args)
    {
        OpenConfig();
    }

    private void OnSaveCommand(string command, string args)
    {
        ChatLog.PrintLog(args);
    }

    private void OnCopyCommand(string command, string args)
    {
        ImGui.SetClipboardText(ChatLog.PrintLog(args, aClipboard: true));
    }

    private void DrawUI() => WindowSystem.Draw();
    private void OpenConfig() => ConfigWindow.IsOpen = true;
}