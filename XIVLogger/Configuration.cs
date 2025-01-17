using Dalamud.Configuration;
using Dalamud.Game.Text;

namespace XIVLogger;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public List<ChatConfig> configList = [];

    public ChatConfig defaultConfig;
    public ChatConfig activeConfig;
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
    public bool StoreEveryMessage = false;

    // This is for file names
    public bool fileSortableDatetime = false;


    public void Initialize()
    {
        var selectedConfig = configList.FirstOrDefault(c => c.IsActive);
        if (selectedConfig is not null)
        {
            activeConfig = selectedConfig;
            return;
        }

        // No active config found, so we set the default config
        if (defaultConfig == null)
        {
            defaultConfig = new ChatConfig();
            activeConfig = defaultConfig;
        }

        SetActiveConfig(defaultConfig);
    }

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }

    public void SetActiveConfig(ChatConfig aConfig)
    {
        activeConfig.IsActive = false;
        activeConfig = aConfig;
        activeConfig.IsActive = true;

        Save();
    }

    public ChatConfig AddNewConfig(string name)
    {
        configList.Add(new ChatConfig(name));

        return configList.Last();
    }

    public void RemoveConfig(ChatConfig aConfig)
    {
        if (aConfig == defaultConfig)
            return;

        if (aConfig.IsActive)
            SetActiveConfig(defaultConfig);

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
    private bool isActive;

    public Dictionary<int, bool> TypeConfig { get => typeConfig; set => typeConfig = value; }
    public string Name { get => name; set => name = value; }
    public bool IsActive { get => isActive; set => isActive = value; }

    public ChatConfig()
    {
        name = "Default";
        isActive = false;

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
            { 8266, true },
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
            { 8266, true },
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