using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static VpnBot.BotConfig;
using static VpnBot.FileManager;
using static VpnBot.CommandLogic;
using static VpnBot.BotKeyboards;
using static VpnBot.SessionState;
using static VpnBot.BoolChecks;
using System.Text.Json;

await Initialise();
using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(BOT_TOKEN, cancellationToken: cts.Token);
var me = await bot.GetMe();
bot.StartReceiving(
    updateHandler: HandleUpdateAsync,
    errorHandler: HandleErrorAsync,
    receiverOptions: new ReceiverOptions
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    },
    cancellationToken: cts.Token
);

Console.WriteLine($"@{me.Username} is running...");
if(SUPERADMIN_ID != 0) await bot.SendMessage(SUPERADMIN_ID, $"@{me.Username} пересобран и запущен!");
else Console.WriteLine("[!] SUPERADMIN_ID не задан — бот запущен в тестовом режиме.");

Thread.Sleep(Timeout.Infinite);

async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source,CancellationToken cancellationToken)
{
    if (exception is TaskCanceledException or OperationCanceledException)
    {
        Console.WriteLine("Bot was force stopped...");
        return;
    }
    else if (exception is Telegram.Bot.Exceptions.RequestException reqEx)
    {
        if (reqEx.Message.Contains("timed out") || reqEx.Message.Contains("Resource temporarily"))
        {
            Console.WriteLine("Произошла несущественная RequestException: " + reqEx.Message);
            return;
        }
    }
    else {
        Console.WriteLine("ПРОИЗОШЛА ОШИБКА\n\n" + exception);
        await bot.SendMessage(SUPERADMIN_ID, exception.GetType().ToString(), cancellationToken: cancellationToken);
    }
}

async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update.Message is { } msg)
    {
        if (msg.Text != null)
        {
            await HandleMessage(msg);
        }
        else if (msg.Document != null)
        {
            if (UserInQueue(FileQueue, msg.Chat.Id))
                EnqueueEdit(() => FileReplacing(bot, msg));
            else
            {
                await bot.SendMessage(msg.Chat.Id, "Зачем мне это. Вот лучше меню посмотри.");
                EnqueueEdit(async () => await StartMenu(msg));
            }
        }
    }
}
async Task HandleMessage(Message msg)
{
    var Users = await LoadUsers(usersPath);
    var Admins = await LoadUsers(adminsPath);
    if (SUPERADMIN_ID == 0)
    {
        Console.WriteLine($"[!] Новый пользователь: Chat ID: {msg.Chat.Id} — @{msg.Chat.Username}");
        await bot.SendMessage(msg.Chat.Id, "🛠 Бот еще не настроен. Админ должен указать Chat ID в config.txt");
        return;
    }

    if (!Users.ContainsKey(msg.Chat.Id))
    {
        await bot.SendMessage(msg.Chat.Id, "⛔ У тебя нет доступа к этому боту.");
        await bot.SendMessage(SUPERADMIN_ID, $"⚠️ @{msg.Chat.Username} пытается использовать бота ⚠️\n\nChat Id пользователя:\n<code>{msg.Chat.Id}</code>", parseMode: ParseMode.Html);
        return;
    }
    if (UserInQueue(RulesAddingQueue, msg.Chat.Id)) EnqueueEdit(() => RulesAdding(bot, msg));
    if (UserInQueue(RulesDeletingQueue, msg.Chat.Id)) EnqueueEdit(() => RulesDeleting(bot, msg));
    if (UserInQueue(VPNChangingQueue, msg.Chat.Id)) EnqueueEdit(() => VpnChanging(bot, msg));
    if (UserInQueue(UserAddingQueue, msg.Chat.Id)) EnqueueEdit(() => UserAdding(bot, msg));
    if (UserInQueue(UserDeletingQueue, msg.Chat.Id)) EnqueueEdit(() => UserDeleting(bot, msg));
    if (UserInQueue(SpeakingQueue, msg.Chat.Id)) EnqueueEdit(() => Speaking(bot, msg));
    if (UserInQueue(FileQueue, msg.Chat.Id)) EnqueueEdit(() => FileReplacing(bot, msg));

    switch (msg.Text)
    {
        case "📋 Список правил":
            EnqueueEdit(async () => await PrintRules(msg));
            break;
        case "🔨 Добавить правила":
            EnqueueEdit(async () => await Change(msg, "add"));
            break;
        case "💥 Удалить правила":
            EnqueueEdit(async () => await Change(msg, "delete"));
            break;
        case "🚨 Инструкция и назначение 🚨":
            string instruction = string.Join('\n', await DownloadFileAsLines(instructionPath));
            await bot.SendMessage(msg.Chat.Id, instruction, ParseMode.Html);
            EnqueueEdit(async () => await StartMenu(msg));
            break;
        case "🌐 Информация о VPN":
            EnqueueEdit(async () => await PrintVPNinfo(msg));
            break;
        case "🐈 Clash файл":
            EnqueueEdit(async () => await SendFile(bot, msg, yamlPath));
            break;
        case "🔗 Ссылка Clash":
            await bot.SendMessage(msg.Chat.Id, $"Твоя ссылка (копируется при нажатии)\n\n<code>{yamlLink}</code>", parseMode: ParseMode.Html);
            break;
        case "⚙️ Изменить настройки":
            if (Admins.ContainsKey(msg.Chat.Id)) EnqueueEdit(async () => await Change(msg, "VPN"));
            else
            {
                await bot.SendMessage(msg.Chat.Id, "⛔ У тебя нет доступа к данной функции.");
                EnqueueEdit(async () => await StartMenu(msg));
            }
            break;
        case "👥 Список пользователей":
            if (Admins.ContainsKey(msg.Chat.Id)) EnqueueEdit(async () => await PrintUsers(msg));
            else
            {
                await bot.SendMessage(msg.Chat.Id, "⛔ У тебя нет доступа к данной функции.");
                EnqueueEdit(async () => await StartMenu(msg));
            }
            break;
        case "👤 Добавить пользователя":
            EnqueueEdit(async () => await Change(msg, "addUser"));
            break;
        case "💥 Удалить пользователя":
            EnqueueEdit(async () => await Change(msg, "deleteUser"));
            break;
        case "🗣 Объявление":
            EnqueueEdit(async () => await Change(msg, "voice"));
            break;
        case "♻️ Заменить Clash файл":
            if (Admins.ContainsKey(msg.Chat.Id)) EnqueueEdit(async () => await Change(msg, "file"));
            else {
                await bot.SendMessage(msg.Chat.Id, "⛔ У тебя нет доступа к данной функции.");
                EnqueueEdit(async () => await StartMenu(msg));
            }
            break;
        default:
            EnqueueEdit(async () => await StartMenu(msg));
            break;
    }
}

async Task PrintRules(Message msg)
{
    UserRuleSnapshots[msg.Chat.Id] = await ReadFile(yamlPath);
    if (UserRuleSnapshots[msg.Chat.Id].Count == 0) await bot.SendMessage(chatId: msg.Chat.Id, text: "📭 Пока что список пуст", replyMarkup: RulesEditKeyboardEmpty);
    else
    {
        var numbered = UserRuleSnapshots[msg.Chat.Id]
        .Select((rule, i) => $"{i + 1}. {rule}")
        .ToList();
        var RulesText = $"```\n{string.Join("\n", numbered)}\n```";
        await bot.SendMessage(chatId: msg.Chat.Id, text: "📋 Текущие правила:\n" + RulesText, parseMode: ParseMode.MarkdownV2);
        await bot.SendMessage(
            chatId: msg.Chat.Id,
            text: "❓ Выбери, что будем делать",
            replyMarkup: RulesEditKeyboard
        );
    }
}

async Task Change(Message msg, string flag)
{
    string textAlreadyIn;
    string textChangedMyMind;
    string textStarter;
    Dictionary<long, bool> Queue;
    switch (flag)
    {
        case "add":
            textAlreadyIn = "❌ Ты уже в режиме добавления. Введи правила или нажми «Не хочу добавлять правила».";
            textChangedMyMind = "👎 Не хочу добавлять правила";
            textStarter = "✍️ Введи список правил, каждое на новой строке, которые хочешь добавить.\n\nФормат ввода (нужное подчеркнуто):\nwww&#8203;.music&#8203;.<u>yandex&#8203;.com</u>/sobaka\nhtt&#8203;ps://www.&#8203;<u>google.&#8203;com</u>\nhttp&#8203;s://www.&#8203;open.&#8203;<u>chatgpt.&#8203;clash</u>\n\nИщем последнюю точку -> берем <b>СловоСлева.СловоСправа</b>";
            Queue = RulesAddingQueue;
            break;
        case "delete":
            textAlreadyIn = "❌ Ты уже в режиме удаления. Введи номера или нажми «Не хочу удалять правила».";
            textChangedMyMind = "👎 Не хочу удалять правила";
            textStarter = "✍️ Введи номера правил через пробел, которые хочешь удалить.\n\nФормат ввода:\n2 5 7 4";
            Queue = RulesDeletingQueue;
            break;
        case "VPN":
            textAlreadyIn = "❌ Ты уже в режиме изменения настроек VPN. Введи новые настройки или нажми «Не хочу изменять настройки».";
            textChangedMyMind = "👎 Не хочу изменять настройки";
            textStarter = "✍️ Введи новые настройки VPN в следующем формате (каждую строчку с новой строки):\n\n<i>Тип\nСервер\nПорт\nШифр\nПароль</i>\n\nПример:\n\n<i>shadowsocks\n<code>vpn.com.ru</code>\n15860\nchacha20-ietf-poly1305\n12345678</i>";
            Queue = VPNChangingQueue;
            break;
        case "addUser":
            textAlreadyIn = "❌ Ты уже в режиме добавления пользователя. Введи Chat Id пользователя или нажми «Не хочу добавлять пользователя».";
            textChangedMyMind = "👎 Не хочу добавлять пользователя";
            textStarter = "✍️ Введи Chat Id и имя нового пользователя через запятую";
            Queue = UserAddingQueue;
            break;
        case "deleteUser":
            textAlreadyIn = "❌ Ты уже в режиме удаления пользователя. Введи Chat Id пользователя или нажми «Не хочу удалять пользователя».";
            textChangedMyMind = "👎 Не хочу удалять пользователя";
            textStarter = "✍️ Введи Chat Id пользователя (можно скопировать нажав на нужный Chat Id)";
            Queue = UserDeletingQueue;
            break;
        case "voice":
            textAlreadyIn = "❌ Ты уже в режиме объявления. Введи объявление или нажми «Не хочу делать объявление».";
            textChangedMyMind = "👎 Не хочу делать объявление";
            textStarter = "✍️ Введи текст объявления";
            Queue = SpeakingQueue;
            break;
        case "file":
            textAlreadyIn = "❌ Ты уже в режиме замены. Скинь новый файл или нажми «Не хочу делать замену».";
            textChangedMyMind = "👎 Не хочу делать замену";
            textStarter = "📦 Отправь файл для замены";
            Queue = FileQueue;
            break;
        default:
            return;
    }
    if (Queue.TryGetValue(msg.Chat.Id, out bool isWaiting) && isWaiting)
    {
        await bot.SendMessage(msg.Chat.Id, textAlreadyIn);
        return;
    }
    Queue[msg.Chat.Id] = true;
    var ChangeKeyboard = new ReplyKeyboardMarkup(
        [
            [new KeyboardButton(textChangedMyMind)]
        ])
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false
    };
    await bot.SendMessage(chatId: msg.Chat.Id, text: textStarter, replyMarkup: ChangeKeyboard, parseMode: ParseMode.Html);
}

async Task StartMenu(Message msg)
{
    await bot.SendMessage(
        chatId: msg.Chat.Id,
        text: "😊 Что тебе показать?",
        replyMarkup: MenuKeyboard
    );
}
async Task PrintVPNinfo(Message msg)
{
    Proxy info = await GetVPNinfo();
    using var http = new HttpClient();
    var json = await http.GetStringAsync($"http://ip-api.com/json/{info.server}");
    var geo = JsonSerializer.Deserialize<GeoInfo>(json);
    if (geo == null || geo.status != "success")
    {
        await bot.SendMessage(msg.Chat.Id, "⚠️ Не удалось определить геолокацию IP.");
        return;
    }

    string vpninfo = $"🐷 Название:\n```{info.name}```\n\n🤖 Тип:\n```{info.type}```\n\n🔧 Сервер:\n```{info.server}```\n\n🔢 Порт:\n```{info.port}```\n\n🔒 Ширфт:\n```{info.cipher}```\n\n🔑 Пароль:\n```{info.password}```\n\n";
    string geoinfo = $"🌍 Геолокация:\n```{geo.country}, {geo.regionName}, {geo.city}\n{geo.zip}```";
    await bot.SendMessage(msg.Chat.Id, vpninfo + geoinfo, parseMode: ParseMode.Markdown);
    await bot.SendMessage(
        chatId: msg.Chat.Id,
        text: "❓ Выбери, что будем делать",
        replyMarkup: VpnEditKeyboard
    );
}

async Task PrintUsers(Message msg)
{
    var Users = await LoadUsers(usersPath);
    string AllUsers = string.Empty;
    foreach (var element in Users)
    {
        AllUsers += $"{element.Value}, <code>{element.Key}</code>\n";
    }
    await bot.SendMessage(msg.Chat.Id, $"👥 Список пользователей (Имя, Chat Id)\n\n" + AllUsers, parseMode: ParseMode.Html);
    await bot.SendMessage(
    chatId: msg.Chat.Id,
    text: "❓ Выбери, что будем делать",
    replyMarkup: UsersEditKeyboard
);
}

public class Proxy
{
    public string name, type, server, cipher, port, password;
    public Proxy(string name = "undefined", string type = "undefined", string server = "undefined", string port = "undefined", string cipher = "undefined", string password = "undefined")
    {
        this.name = name;
        this.type = type;
        this.server = server;
        this.port = port;
        this.cipher = cipher;
        this.password = password;
    }
}

public class GeoInfo
{
    required public string status { get; set; }
    required public string country { get; set; }
    required public string regionName { get; set; }
    required public string city { get; set; }
    required public string zip { get; set; }
}