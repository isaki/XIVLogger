using System.IO;
using Dalamud.Game.Text;

namespace XIVLogger;

public class ChatMessage
{
    public XivChatType Type;
    public string Message;
    public string Sender;
    public DateTime Timestamp;

    public ChatMessage(XivChatType type, string sender, string message)
    {
        Sender = sender;
        Type = type;
        Message = message;
        Timestamp = DateTime.Now;
    }
}

public class ChatStorage
{
    private List<ChatMessage> LogList;
    private Configuration Config;

    private int AutoMsgCounter;

    public ChatStorage(Configuration config)
    {
        Config = config;
        LogList = new List<ChatMessage>();
    }

    public void WipeLog()
    {
        LogList = new List<ChatMessage>();
    }

    public void AddMessage(XivChatType type, string sender, string message)
    {
        LogList.Add(new ChatMessage(type, sender, message));
    }

    private static string GetTimeStamp()
    {
        return DateTime.Now.ToString("dd-MM-yyyy_hh.mm.ss");
    }

    private static bool CheckValidPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        return Path.IsPathRooted(path) && Directory.Exists(path);
    }

    private static string ReplaceInvalidChars(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }

    public string PrintLog(string args, bool aClipboard = false)
    {
        var lastN = 0;
        if (!string.IsNullOrEmpty(args))
            int.TryParse(args, out lastN);

        var printedLog = PrepareLog(aLastN: lastN, aTimestamp: Config.fTimestamp, aTimeSeconds: Config.fTimeSeconds, a24hTimestamp: Config.f24hTimestamp, aDatestamp: Config.fDatestamp);

        if (aClipboard)
        {
            var clip = string.Empty;

            foreach (var message in printedLog)
            {
                clip += message;
                clip += Environment.NewLine;
            }

            if (lastN > 0)
            {
                Plugin.Chat.Print(new XivChatEntry
                {
                    Message = $"Last {lastN} messages copied to clipboard.",
                    Type = XivChatType.Echo
                });
            }
            else
            {
                Plugin.Chat.Print(new XivChatEntry
                {
                    Message = $"Chat log copied to clipboard.",
                    Type = XivChatType.Echo
                });
            }

            return clip;
        }

        var name = GetTimeStamp();
        var folder = !CheckValidPath(Config.filePath)
            ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            : Config.filePath;

        if (!string.IsNullOrEmpty(Config.fileName) && !string.IsNullOrWhiteSpace(Config.fileName))
            name = ReplaceInvalidChars(Config.fileName);

        var path = folder + @"\" + name + ".txt";
        var count = 0;

        while (File.Exists(path))
        {
            count++;
            path = folder + @"\" + name + count + ".txt";
        }

        using (var file = new StreamWriter(path, true))
        {
            file.WriteLine(name + "\n");

            foreach (var message in printedLog)
                file.WriteLine(message);
        }

        Plugin.Chat.Print(lastN > 0 ? $"Last {lastN} messages saved at {path}." : $"Chat log saved at {path}.");
        return path;
    }

    private List<string> PrepareLog(int aLastN = 0, bool aTimestamp = false, bool aTimeSeconds = false, bool a24hTimestamp = false, bool aDatestamp = false, bool auto = false)
    {
        var activeConfig = Config.activeConfig;
        var result = new List<string>();

        foreach (var message in LogList)
        {
            if (activeConfig.TypeConfig.ContainsKey((int)message.Type) && activeConfig.TypeConfig[(int)message.Type])
            {
                var text = string.Empty;

                var sender = message.Sender;

                if(activeConfig.NameReplacements.TryGetValue(sender, out var replacement))
                    sender = replacement;

                if (aTimestamp)
                {
                    text += "[";
                    if (aDatestamp)
                    {
                        text += $"{message.Timestamp:yyyy}-{message.Timestamp:MM}-{message.Timestamp:dd} ";
                    }
                    if (a24hTimestamp)
                    {
                        if (aTimeSeconds)
                        {
                            text += $"{message.Timestamp:HH}:{message.Timestamp:mm}:{message.Timestamp:ss}";
                        } else {
                            text += $"{message.Timestamp:HH}:{message.Timestamp:mm}";
                        }
                    } else {
                        text += $"{message.Timestamp:t}";
                    }
                    text += "] ";
                }

                switch (message.Type)
                {
                    case XivChatType.CustomEmote:
                        text += sender + message.Message;
                        break;
                    case XivChatType.StandardEmote:
                        text += message.Message;
                        break;
                    case XivChatType.TellIncoming:
                        text += sender + " >> " + message.Message;
                        break;
                    case XivChatType.TellOutgoing:
                        text += ">> " + sender + ": " + message.Message;
                        break;
                    case XivChatType.FreeCompany:
                        text += "[FC]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.NoviceNetwork:
                        text += "[NN]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.CrossLinkShell1:
                        text += "[CWLS1]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.CrossLinkShell2:
                        text += "[CWLS2]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.CrossLinkShell3:
                        text += "[CWLS3]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.CrossLinkShell4:
                        text += "[CWLS4]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.CrossLinkShell5:
                        text += "[CWLS5]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.CrossLinkShell6:
                        text += "[CWLS6]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.CrossLinkShell7:
                        text += "[CWLS7]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.CrossLinkShell8:
                        text += "[CWLS8]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Ls1:
                        text += "[LS1]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Ls2:
                        text += "[LS2]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Ls3:
                        text += "[LS3]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Ls4:
                        text += "[LS4]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Ls5:
                        text += "[LS5]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Ls6:
                        text += "[LS6]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Ls7:
                        text += "[LS7]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Ls8:
                        text += "[LS8]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.PvPTeam:
                        text += "[PvP]" + sender + ": " + message.Message;
                        break;
                    case XivChatType.Debug:
                        text += "[DBG]" + message.Message;
                        break;
                    case XivChatType.Say:
                    case XivChatType.Shout:
                    case XivChatType.Yell:
                    case XivChatType.Party:
                    case XivChatType.CrossParty:
                    case XivChatType.Alliance:
                        text += sender + ": " + message.Message;
                        break;
                    default:
                        text += message.Message;
                        break;
                }

                result.Add(text);
            }
        }

        if (aLastN > 0)
            result = result.Skip(Math.Max(0, result.Count - aLastN)).ToList();

        // Only return the messages which have been received since the last autosave, and update the counter
        if (auto)
        {
            result = result.Skip(AutoMsgCounter).ToList();
            AutoMsgCounter += result.Count;
        }

        return result;
    }

    public void SetupAutosave()
    {
        Config.autoFileName = $"{GetTimeStamp()} {Plugin.ClientState.LocalPlayer?.Name.ToString() ?? "No Character"}";
        Config.Save();
    }

    public void AutoSave()
    {
        if (!Config.fAutosave)
            return;

        var printedLog = PrepareLog(aLastN: 0, aTimestamp: Config.fTimestamp, aTimeSeconds: Config.fTimeSeconds, a24hTimestamp: Config.f24hTimestamp, aDatestamp: Config.fDatestamp, true);
        var folder = !CheckValidPath(Config.autoFilePath)
            ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            : Config.autoFilePath;

        var path = Path.Combine(folder, $"{Config.autoFileName}.txt");
        using (var file = new StreamWriter(path, true))
        {
            foreach (var message in printedLog)
                file.WriteLine(message);
        }

        if (Config.fAutosaveNotif)
            Plugin.Chat.Print(new XivChatEntry { Message = $"Autosaved chat log to {path}.", Type = XivChatType.Echo });
    }
}