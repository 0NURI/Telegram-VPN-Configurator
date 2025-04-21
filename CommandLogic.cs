using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static VpnBot.BotConfig;
using static VpnBot.SessionState;
using static VpnBot.FileManager;
using static VpnBot.BoolChecks;
using static VpnBot.BotKeyboards;

namespace VpnBot;

public static class CommandLogic
{
    async public static Task VpnChanging(ITelegramBotClient bot, Message msg)
    {
        if (msg.Text == "👎 Не хочу изменять настройки")
        {
            VPNChangingQueue[msg.Chat.Id] = false;
            return;
        }
        if (string.IsNullOrWhiteSpace(msg.Text)){
            await bot.SendMessage(chatId: msg.Chat.Id, text: "❌ Пустой ввод, попробуй еще раз");
            FileQueue[msg.Chat.Id] = false;
        }
        else
        {
            var lines = await DownloadFileAsLines(yamlPath);
            var InputInfo = msg.Text.Split('\n');
            if (InputInfo.Length < 5)
            {
                await bot.SendMessage(msg.Chat.Id, "❌ Неполные данные. Убедись, что ввёл все 5 строк.");
                VPNChangingQueue[msg.Chat.Id] = false;
                return;
            }
            int start = lines.FindIndex(line => line == "  - name: \"MYVPN\"");
            int end = lines.FindIndex(start + 1, line => line == "proxy-groups:") - 1;
            lines.RemoveRange(start + 1, end - start - 1);
            string[] OutputInfo = [$"    type: {InputInfo[0]}", $"    server: {InputInfo[1]}", $"    port: {InputInfo[2]}", $"    cipher: {InputInfo[3]}", $"    password: \"{InputInfo[4]}\""];
            lines.InsertRange(start + 1, OutputInfo);
            await UploadFileFromLines(yamlPath, lines);
            VPNChangingQueue[msg.Chat.Id] = false;
            await bot.SendMessage(msg.Chat.Id, "✅ Настройки VPN успешно изменены!");
        }
    }

    async public static Task RulesAdding(ITelegramBotClient bot, Message msg)
    {
        Rules = await ReadFile(yamlPath);
        if (msg.Text == "👎 Не хочу добавлять правила")
        {
            RulesAddingQueue[msg.Chat.Id] = false;
            return;
        }
        if (string.IsNullOrWhiteSpace(msg.Text)){
            await bot.SendMessage(chatId: msg.Chat.Id, text: "❌ Пустой ввод, попробуй еще раз");
            FileQueue[msg.Chat.Id] = false;
        }
        else
        {
            List<string> DomainToAdd = [];
            foreach (string element in msg.Text.Split('\n'))
            {
                var domain = element.Trim().ToLower();
                if (!Rules.Contains(domain) && DomainCheck(domain))
                {
                    DomainToAdd.Add(domain.Trim().ToLower());
                }
                else if (Rules.Contains(domain)) await bot.SendMessage(chatId: msg.Chat.Id, text: $"❌ `{domain}`" + " - домен уже был записан ранее", parseMode: ParseMode.Markdown);
                else await bot.SendMessage(chatId: msg.Chat.Id, text: $"❌ `{domain}`" + " - недопустимый формат домена", parseMode: ParseMode.Markdown);
            }
            Rules.AddRange(DomainToAdd);
            await bot.SendMessage(chatId: msg.Chat.Id, $"✅ Домены, прошедшие проверку, добавлены!", parseMode: ParseMode.Markdown);
            await UpdateRulesClashYaml(Rules);
            RulesAddingQueue[msg.Chat.Id] = false;
            if (DomainToAdd.Count > 0) await bot.SendMessage(SUPERADMIN_ID, $"🔨 @{msg.Chat.Username} добавил домены\n<code>{string.Join('\n', DomainToAdd)}</code>", parseMode: ParseMode.Html);

        }
    }

    public static async Task RulesDeleting(ITelegramBotClient bot, Message msg)
    {
        if (msg.Text == "👎 Не хочу удалять правила")
        {
            RulesDeletingQueue[msg.Chat.Id] = false;
            return;
        }
        if (string.IsNullOrWhiteSpace(msg.Text)){
            await bot.SendMessage(chatId: msg.Chat.Id, text: "🤷 Нечего удалять, попробуй еще раз");
            FileQueue[msg.Chat.Id] = false;
        }
        else
        {
            Rules = await ReadFile(yamlPath);
            var splitNums = msg.Text.Split(' ').OrderByDescending(x => x).ToList();
            List<string> Repeated = [];
            List<string> DeleteAwaiting = [];
            foreach (string element in splitNums)
            {
                if (!Int32.TryParse(element, out int num)){
                    await bot.SendMessage(chatId: msg.Chat.Id, $"❌ `{element}` - неверный формат числа", parseMode: ParseMode.Markdown);
                    FileQueue[msg.Chat.Id] = false;
                }
                else if (splitNums.Count(x => x == num.ToString()) > 1)
                {
                    if (!Repeated.Contains(element))
                    {
                        await bot.SendMessage(chatId: msg.Chat.Id, $"❌ `{element}` - была попытка удалить правило как минимум дважды", parseMode: ParseMode.Markdown);
                        Repeated.Add(element);
                    }
                }
                else if (num > UserRuleSnapshots[msg.Chat.Id].Count || num < 1) await bot.SendMessage(chatId: msg.Chat.Id, $"❌ `{element}` - выход за рамки списка правил", parseMode: ParseMode.Markdown);
                else DeleteAwaiting.Add(UserRuleSnapshots[msg.Chat.Id][num - 1]);
            }
            foreach (string element in DeleteAwaiting)
            {
                if (Rules.Contains(element)) Rules.Remove(element);
            }

            await bot.SendMessage(chatId: msg.Chat.Id, $"✅ Домены с верно записанными номерами удалены!", parseMode: ParseMode.Markdown);
            await UpdateRulesClashYaml(Rules);
            RulesDeletingQueue[msg.Chat.Id] = false;
            if (DeleteAwaiting.Count > 0) await bot.SendMessage(SUPERADMIN_ID, $"💥 @{msg.Chat.Username} удалил домены\n<code>{string.Join('\n', DeleteAwaiting)}</code>", parseMode: ParseMode.Html);
        }
    }
    public static async Task UserAdding(ITelegramBotClient bot, Message msg)
    {
        if (msg.Text == "👎 Не хочу добавлять пользователя")
        {
            UserAddingQueue[msg.Chat.Id] = false;
            return;
        }
        if (string.IsNullOrWhiteSpace(msg.Text)){
            await bot.SendMessage(chatId: msg.Chat.Id, text: "❌ Пустой ввод, попробуй еще раз");
            FileQueue[msg.Chat.Id] = false;
        }
        else
        {
            var UserInfo = msg.Text.Split(',');
            if (UserInfo.Count() != 2 || !long.TryParse(UserInfo[0], out long num) || string.IsNullOrWhiteSpace(UserInfo[0]) || string.IsNullOrWhiteSpace(UserInfo[0]))
            {
                await bot.SendMessage(msg.Chat.Id, "❌ Неправильный ввод данных");
                UserAddingQueue[msg.Chat.Id] = false;
                return;
            }
            var lines = await DownloadFileAsLines(usersPath);
            string result = string.Join(',', UserInfo);
            if (lines.Any(x => x.Substring(0, x.IndexOf(',')) == UserInfo[0]))
            {
                await bot.SendMessage(msg.Chat.Id, "❌ Попытка добавить пользователя повторно");
                UserAddingQueue[msg.Chat.Id] = false;
                return;
            }
            if (!lines.Contains(result))
            {
                lines.Add(string.Join(',', UserInfo.Select(x => x.Trim())));
                await UploadFileFromLines(usersPath, lines);
                await bot.SendMessage(msg.Chat.Id, "✅ Пользователь успешно добавлен!");
                UserAddingQueue[msg.Chat.Id] = false;
            }
            else await bot.SendMessage(msg.Chat.Id, "❌ Попытка добавить пользователя повторно");
            UserAddingQueue[msg.Chat.Id] = false;
        }
    }
    public static async Task UserDeleting(ITelegramBotClient bot, Message msg)
    {
        if (msg.Text == "👎 Не хочу удалять пользователя")
        {
            UserDeletingQueue[msg.Chat.Id] = false;
            return;
        }
        if (string.IsNullOrWhiteSpace(msg.Text)){
            await bot.SendMessage(chatId: msg.Chat.Id, text: "❌ Пустой ввод, попробуй еще раз");
            FileQueue[msg.Chat.Id] = false;
        }
        else
        {
            var UserInfo = msg.Text;
            if (msg.Text.Equals(msg.Chat.Id.ToString()))
            {
                await bot.SendMessage(msg.Chat.Id, "❌ Себя удалить нельзя");
                UserDeletingQueue[msg.Chat.Id] = false;
                return;
            }
            var lines = await DownloadFileAsLines(usersPath);
            foreach (var element in lines)
            {
                if (msg.Text.Equals(element[..element.IndexOf(',')]))
                {
                    lines.Remove(element);
                    await UploadFileFromLines(usersPath, lines);
                    await bot.SendMessage(msg.Chat.Id, "✅ Пользователь успешно удален!");
                    UserDeletingQueue[msg.Chat.Id] = false;
                    return;
                }
            }
            await bot.SendMessage(msg.Chat.Id, "❌ Такого пользователя не нашлось, попробуй еще раз");
            UserDeletingQueue[msg.Chat.Id] = false;

        }
    }
    public static async Task Speaking(ITelegramBotClient bot, Message msg)
    {
        if (msg.Text == "👎 Не хочу делать объявление")
        {
            SpeakingQueue[msg.Chat.Id] = false;
            return;
        }
        if (string.IsNullOrWhiteSpace(msg.Text)){
            await bot.SendMessage(chatId: msg.Chat.Id, text: "🤷 Пустой ввод, попробуй еще раз");
            FileQueue[msg.Chat.Id] = false;
        }
        else
        {
            var Users = await LoadUsers(usersPath);
            foreach (var chatId in Users)
            {
                await bot.SendMessage(chatId.Key, msg.Text);
            }
            await bot.SendMessage(chatId: msg.Chat.Id, $"✅ Объявлено!", parseMode: ParseMode.Markdown);
            SpeakingQueue[msg.Chat.Id] = false;
        }
    }
    public static async Task FileReplacing(ITelegramBotClient bot, Message msg)
    {
        if (msg.Text == "👎 Не хочу делать замену")
        {
            FileQueue[msg.Chat.Id] = false;
            return;
        }
        if (msg.Type != MessageType.Document)
        {
            await bot.SendMessage(chatId: msg.Chat.Id, text: "❌ Это не файл, попробуй еще раз");
            FileQueue[msg.Chat.Id] = false;
        }
        else
        {
            var fileName = msg.Document?.FileName?.ToLower();
            if(fileName == null || !fileName.EndsWith(".yaml") || msg.Document == null)
            {
                await bot.SendMessage(msg.Chat.Id, "❌ Неверный формат файла, попробуй ещё раз");
                FileQueue[msg.Chat.Id] = false;
            }
            else {
                var file = await bot.GetFile(msg.Document.FileId);

                using (var fs = new FileStream(yamlPath, FileMode.Create))
                {
                    await bot.DownloadFile(file.FilePath!, fs);
                }
                await bot.SendMessage(msg.Chat.Id, "✅ Файл успешно заменен!", replyMarkup: MenuKeyboard);
                FileQueue[msg.Chat.Id] = false;
            }
        }
    }
}
