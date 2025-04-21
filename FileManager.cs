using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using static VpnBot.BotConfig;

namespace VpnBot;


public static class FileManager
{

    public static List<string> Rules = new();
    public static async Task<List<string>> DownloadFileAsLines(string filePath)
    {
        return await File.ReadAllLinesAsync(filePath).ContinueWith(t => t.Result.ToList());
    }

    public static async Task<byte[]> DownloadFileAsBytes(string filePath)
    {
        return await File.ReadAllBytesAsync(filePath);
    }

    public static async Task UploadFileFromLines(string filePath, List<string> lines)
    {
        await File.WriteAllLinesAsync(filePath, lines);
    }

    public static string GetPublicUrl(string filePath)
    {
        return $"file://{filePath}";
    }

    public static async Task<Dictionary<long, string>> LoadUsers(string path)
    {
        var lines = await DownloadFileAsLines(path);
        Dictionary<long, string> result = [];
        foreach (var element in lines)
        {
            if (string.IsNullOrWhiteSpace(element)) continue;
            var IdName = element.Split(',');
            result[long.Parse(IdName[0])] = IdName[1];
        }
        return result;

    }
    public static async Task<List<string>> ReadFile(string path)
    {
        const string start = "#start";
        var lines = await DownloadFileAsLines(path);
        var rulesIndex = lines.FindIndex(line => line.Trim() == start);
        lines = lines.GetRange(rulesIndex + 1, lines.Count - rulesIndex - 2);
        return lines.Select(x => x.Split(',')[1].Trim().ToLower()).ToList();
    }

    public static async Task UpdateRulesClashYaml(List<string> rules)
    {
        const string start = "#start";
        var lines = await DownloadFileAsLines(yamlPath);
        var rulesIndex = lines.FindIndex(line => line.Trim() == start);
        lines = lines.Take(rulesIndex + 1).ToList();
        lines.AddRange(rules.Select(rule => $"  - DOMAIN-SUFFIX,{rule},Proxy"));
        lines.Add("  - MATCH,DIRECT");
        await UploadFileFromLines(yamlPath, lines);
    }

    public static async Task<Proxy> GetVPNinfo()
    {
        const string start = "proxies:";
        var lines = await DownloadFileAsLines(yamlPath);
        var rulesIndex = lines.FindIndex(line => line.Trim() == start);
        var RawInfo = lines.GetRange(rulesIndex + 1, 6).Select(x => x.Split(':')[1].Trim([' ', '"'])).ToList();
        return new Proxy(RawInfo[0], RawInfo[1], RawInfo[2], RawInfo[3], RawInfo[4], RawInfo[5]);
    }
    public static async Task SendFile(ITelegramBotClient bot, Message msg, string path)
    {
        var fileBytes = await DownloadFileAsBytes(path);
        await using var stream = new MemoryStream(fileBytes);
        InputFileStream inputFile = new(stream, path);
        await bot.SendDocument(msg.Chat.Id, inputFile);
    }

    public static async Task UploadYamlFromTelegramFile(ITelegramBotClient bot, string fileId)
    {
        var file = await bot.GetFile(fileId);
        using var stream = new FileStream(yamlPath, FileMode.Create);
        await bot.DownloadFile(file.FilePath!, stream);
    }

}
