using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using System.Reflection;
using ImGuiNET;
using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace XIVLogger
{

    public class Plugin : IDalamudPlugin
    {
        private const string commandName = "/xivlogger";

        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static IChatGui Chat { get; private set; } = null!;
        [PluginService] public static IFramework Framework { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static IPluginLog Log { get; private set; }  = null!;
        [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;

        private Configuration configuration;
        public ChatLog log;
        private PluginUI ui;
        private bool loggingIn = false;
        private bool loggedIn = false;

        public string Location { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public Plugin()
        {
            this.configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.configuration.Initialize(PluginInterface);

            this.ui = new PluginUI(this.configuration);

            CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
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

            this.log = new ChatLog(configuration);
            this.ui.log = log;

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += () => DrawConfigUI();
            ClientState.Login += OnLogin;
            ClientState.Logout += OnLogout;
            Chat.ChatMessage += OnChatMessage;

            Framework.Update += OnUpdate;
        }

        private void OnLogin()
        {
            loggingIn = true;
        }

        private void OnLogout()
        {
            if (configuration.fAutosave && loggedIn)
            {
                log.autoSave();
                log.wipeLog();
            }
            Log.Debug("Logged out!");
            loggedIn = false;
        }

        private void OnUpdate(IFramework framework)
        {

            if (loggingIn && ClientState.LocalPlayer != null)
            {
                loggingIn = false;
                loggedIn = true;
                log.setupAutosave(ClientState.LocalPlayer.Name.ToString());
            }

            if (configuration.fAutosave)
            {
                if (configuration.checkTime())
                {
                    log.autoSave();
                    configuration.updateAutosaveTime();

                }

            }
        }

        private void OnChatMessage(XivChatType type, uint id, ref SeString sender, ref SeString message, ref bool handled)
        {
            log.addMessage(type, sender.TextValue, message.TextValue);

            //PluginLog.Log("Chat message from type {0}: {1}", type, message.TextValue);
        }

        public void Dispose()
        {

            CommandManager.RemoveHandler(commandName);
            CommandManager.RemoveHandler("/savelog");
            CommandManager.RemoveHandler("/copylog");

            Framework.Update -= OnUpdate;
            Chat.ChatMessage -= OnChatMessage;
            ClientState.Login -= OnLogin;
            ClientState.Logout -= OnLogout;
        }

        private void OnCommand(string command, string args)
        {
            this.ui.SettingsVisible = true;
        }

        private void OnSaveCommand(string command, string args)
        {
            log.printLog(args);
        }

        private void OnCopyCommand(string command, string args)
        {
            ImGui.SetClipboardText(log.printLog(args, aClipboard: true));
        }


        private void DrawUI()
        {
            this.ui.Draw();
        }

        private void DrawConfigUI()
        {
            this.ui.SettingsVisible = true;
        }
    }
}
