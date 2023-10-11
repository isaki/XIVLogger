using Dalamud.Configuration;
using Dalamud.Game.Text;
using Dalamud.Plugin;

namespace XIVLogger
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public List<ChatConfig> configList;
        public ChatConfig defaultConfig;
        public ChatConfig activeConfig;
        public Dictionary<int, string> PossibleChatTypes;
        public string filePath = string.Empty;
        public string fileName = string.Empty;
        public bool fTimestamp = false;
        public bool fTimeSeconds = false;
        public bool f24hTimestamp = false;
        public bool fDatestamp = false;
        public bool fAutosave = false;
        public bool fAutosaveNotif = true;
        public DateTime lastAutosave;
        public int fAutoMin = 5;
        public string autoFilePath = string.Empty;
        public string autoFileName = string.Empty;

        [NonSerialized]
        public DalamudPluginInterface PluginInterface;


        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            PluginInterface = pluginInterface;

            PossibleChatTypes = new Dictionary<int, string>
                {
                    { (int) XivChatType.Say, "Say"},
                    { (int) XivChatType.Shout, "Shout" },
                    { (int) XivChatType.Yell, "Yell" },
                    { (int) XivChatType.Party, "Party" },
                    { (int) XivChatType.CrossParty, "Cross World Party" },
                    { (int) XivChatType.Alliance, "Alliance" },
                    { (int) XivChatType.TellIncoming, "Tell Incoming" },
                    { (int) XivChatType.TellOutgoing, "Tell Outgoing" },
                    { (int) XivChatType.CustomEmote, "Custom Emotes" },
                    { (int) XivChatType.StandardEmote, "Standard Emotes" },
                    { 2122, "/random" },
                    { (int) XivChatType.CrossLinkShell1, "Cross Link Shell 1" },
                    { (int) XivChatType.CrossLinkShell2, "Cross Link Shell 2" },
                    { (int) XivChatType.CrossLinkShell3, "Cross Link Shell 3" },
                    { (int) XivChatType.CrossLinkShell4, "Cross Link Shell 4" },
                    { (int) XivChatType.CrossLinkShell5, "Cross Link Shell 5" },
                    { (int) XivChatType.CrossLinkShell6, "Cross Link Shell 6" },
                    { (int) XivChatType.CrossLinkShell7, "Cross Link Shell 7" },
                    { (int) XivChatType.CrossLinkShell8, "Cross Link Shell 8" },
                    { (int) XivChatType.Ls1, "Linkshell 1" },
                    { (int) XivChatType.Ls2, "Linkshell 2" },
                    { (int) XivChatType.Ls3, "Linkshell 3" },
                    { (int) XivChatType.Ls4, "Linkshell 4" },
                    { (int) XivChatType.Ls5, "Linkshell 5" },
                    { (int) XivChatType.Ls6, "Linkshell 6" },
                    { (int) XivChatType.Ls7, "Linkshell 7" },
                    { (int) XivChatType.Ls8, "Linkshell 8" },
                    { (int) XivChatType.PvPTeam, "PVP Team" },
                    { (int) XivChatType.NoviceNetwork, "Novice Network" },
                    { (int) XivChatType.FreeCompany, "Free Company" },
                    { (int) XivChatType.Echo, "Echo (Some System Messages)" },
                    { (int) XivChatType.SystemMessage, "System Messages" },
                    { (int) XivChatType.SystemError, "System Error" },
                    { (int) XivChatType.Notice, "Notice" },
                    { (int) XivChatType.Debug, "Debug" }
                };

            if (defaultConfig == null)
            {
                defaultConfig = new ChatConfig();
                activeConfig = defaultConfig;
            }

            if (configList == null)
            {
                configList = new List<ChatConfig>();
            }

            SetActiveConfig(defaultConfig);

            Save();
        }

        public void Save()
        {
            PluginInterface.SavePluginConfig(this);
        }

        public void SetActiveConfig(ChatConfig aConfig)
        {
            activeConfig.IsActive = false;
            activeConfig = aConfig;
            activeConfig.IsActive = true;
        }

        public ChatConfig AddNewConfig(string name)
        {
            configList.Add(new ChatConfig(name));

            return configList.Last();
        }

        public void RemoveConfig(ChatConfig aConfig)
        {
            if (aConfig == defaultConfig)
            {
                return;
            }

            if (aConfig.IsActive)
            {
                SetActiveConfig(defaultConfig);
            }

            configList.Remove(aConfig);
        }

        public bool CheckTime()
        {
            if (fAutoMin == 0)
                return false;

            return lastAutosave.AddMinutes(fAutoMin) < DateTime.UtcNow;
        }

        public void UpdateAutosaveTime()
        {
            lastAutosave = DateTime.UtcNow;
        }
    }

    public class ChatConfig
    {
        public string name;
        private Dictionary<int, bool> typeConfig;
        private Dictionary<string, string> nameReplacements;
        private bool isActive;

        public Dictionary<int, bool> TypeConfig { get => typeConfig; set => typeConfig = value; }
        public string Name { get => name; set => name = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
        public Dictionary<string, string> NameReplacements { get => nameReplacements; set => nameReplacements = value; }

        public ChatConfig()
        {
            name = "Default";

            isActive = false;

            nameReplacements = new Dictionary<string, string>();

            typeConfig = new Dictionary<int, bool>
                {
                    { (int) XivChatType.Say, true },
                    { (int) XivChatType.Shout, true },
                    { (int) XivChatType.Yell, true },
                    { (int) XivChatType.Party, true },
                    { (int) XivChatType.CrossParty, true },
                    { (int) XivChatType.Alliance, true },
                    { (int) XivChatType.TellIncoming, true },
                    { (int) XivChatType.TellOutgoing, true },
                    { (int) XivChatType.CustomEmote, true },
                    { (int) XivChatType.StandardEmote, true },
                    { 2122, true },
                    { (int) XivChatType.CrossLinkShell1, false },
                    { (int) XivChatType.CrossLinkShell2, false },
                    { (int) XivChatType.CrossLinkShell3, false },
                    { (int) XivChatType.CrossLinkShell4, false },
                    { (int) XivChatType.CrossLinkShell5, false },
                    { (int) XivChatType.CrossLinkShell6, false },
                    { (int) XivChatType.CrossLinkShell7, false },
                    { (int) XivChatType.CrossLinkShell8, false },
                    { (int) XivChatType.Ls1, false },
                    { (int) XivChatType.Ls2, false },
                    { (int) XivChatType.Ls3, false },
                    { (int) XivChatType.Ls4, false },
                    { (int) XivChatType.Ls5, false },
                    { (int) XivChatType.Ls6, false },
                    { (int) XivChatType.Ls7, false },
                    { (int) XivChatType.Ls8, false },
                    { (int) XivChatType.PvPTeam, false },
                    { (int) XivChatType.NoviceNetwork, false },
                    { (int) XivChatType.FreeCompany, false },
                    { (int) XivChatType.Echo, false },
                    { (int) XivChatType.SystemMessage, false },
                    { (int) XivChatType.SystemError, false },
                    { (int) XivChatType.Notice, false },
                    { (int) XivChatType.Debug, false }
                };
        }

        public ChatConfig(string aName)
        {
            name = aName;

            isActive = false;

            nameReplacements = new Dictionary<string, string>();

            typeConfig = new Dictionary<int, bool>
                {
                    { (int) XivChatType.Say, true },
                    { (int) XivChatType.Shout, true },
                    { (int) XivChatType.Yell, true },
                    { (int) XivChatType.Party, true },
                    { (int) XivChatType.CrossParty, true },
                    { (int) XivChatType.Alliance, true },
                    { (int) XivChatType.TellIncoming, true },
                    { (int) XivChatType.TellOutgoing, true },
                    { (int) XivChatType.CustomEmote, true },
                    { (int) XivChatType.StandardEmote, true },
                    { 2122, true },
                    { (int) XivChatType.CrossLinkShell1, false },
                    { (int) XivChatType.CrossLinkShell2, false },
                    { (int) XivChatType.CrossLinkShell3, false },
                    { (int) XivChatType.CrossLinkShell4, false },
                    { (int) XivChatType.CrossLinkShell5, false },
                    { (int) XivChatType.CrossLinkShell6, false },
                    { (int) XivChatType.CrossLinkShell7, false },
                    { (int) XivChatType.CrossLinkShell8, false },
                    { (int) XivChatType.Ls1, false },
                    { (int) XivChatType.Ls2, false },
                    { (int) XivChatType.Ls3, false },
                    { (int) XivChatType.Ls4, false },
                    { (int) XivChatType.Ls5, false },
                    { (int) XivChatType.Ls6, false },
                    { (int) XivChatType.Ls7, false },
                    { (int) XivChatType.Ls8, false },
                    { (int) XivChatType.PvPTeam, false },
                    { (int) XivChatType.NoviceNetwork, false },
                    { (int) XivChatType.FreeCompany, false },
                    { (int) XivChatType.Echo, false },
                    { (int) XivChatType.SystemMessage, false },
                    { (int) XivChatType.SystemError, false },
                    { (int) XivChatType.Notice, false },
                    { (int) XivChatType.Debug, false }
                };
        }
    }
}



