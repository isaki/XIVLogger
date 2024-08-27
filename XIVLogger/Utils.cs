using Dalamud.Game.Text;

namespace XIVLogger;

public static class Utils
{
    public static string ToChatName(this XivChatType type, string sender)
    {
        var text = "";
        text += type switch
        {
            XivChatType.CustomEmote => $"[Emote]{sender}",
            XivChatType.StandardEmote => "[Emote]",
            XivChatType.TellIncoming => $"{sender} >> ",
            XivChatType.TellOutgoing => $">> {sender}: ",
            XivChatType.FreeCompany => $"[FC]{sender}: ",
            XivChatType.NoviceNetwork => $"[NN]{sender}: ",
            XivChatType.CrossLinkShell1 => $"[CWLS1]{sender}: ",
            XivChatType.CrossLinkShell2 => $"[CWLS2]{sender}: ",
            XivChatType.CrossLinkShell3 => $"[CWLS3]{sender}: ",
            XivChatType.CrossLinkShell4 => $"[CWLS4]{sender}: ",
            XivChatType.CrossLinkShell5 => $"[CWLS5]{sender}: ",
            XivChatType.CrossLinkShell6 => $"[CWLS6]{sender}: ",
            XivChatType.CrossLinkShell7 => $"[CWLS7]{sender}: ",
            XivChatType.CrossLinkShell8 => $"[CWLS8]{sender}: ",
            XivChatType.Ls1 => $"[LS1]{sender}: ",
            XivChatType.Ls2 => $"[LS2]{sender}: ",
            XivChatType.Ls3 => $"[LS3]{sender}: ",
            XivChatType.Ls4 => $"[LS4]{sender}: ",
            XivChatType.Ls5 => $"[LS5]{sender}: ",
            XivChatType.Ls6 => $"[LS6]{sender}: ",
            XivChatType.Ls7 => $"[LS7]{sender}: ",
            XivChatType.Ls8 => $"[LS8]{sender}: ",
            XivChatType.PvPTeam => $"[PvP]{sender}: ",
            XivChatType.Debug => "[DBG] ",
            XivChatType.Say => $"[Say]{sender}: ",
            XivChatType.Shout => $"[Shout]{sender}: ",
            XivChatType.Yell => $"[Yell]{sender}: ",
            XivChatType.Party or XivChatType.CrossParty => $"[Party]{sender}: ",
            XivChatType.Alliance => $"[Alliance]{sender}: ",
            _ => ""
        };

        return text;
    }
}