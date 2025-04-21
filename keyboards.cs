using Telegram.Bot.Types.ReplyMarkups;

namespace VpnBot;

public static class BotKeyboards
{
    public static readonly ReplyKeyboardMarkup RulesEditKeyboardEmpty = new(
    [
        [new KeyboardButton("🔨 Добавить правила")],
        [new KeyboardButton("📑 Меню")]
    ])
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false
    };

    public static readonly ReplyKeyboardMarkup MenuKeyboard = new(
    [
        [new KeyboardButton("📋 Список правил")],
        [new KeyboardButton("🌐 Информация о VPN")],
        [new KeyboardButton("👥 Список пользователей")],
        [new KeyboardButton("🗣 Объявление")],
        [new KeyboardButton("🚨 Инструкция и назначение 🚨")]
    ])
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false
    };

    public static readonly ReplyKeyboardMarkup RulesEditKeyboard = new(
    [
        [new KeyboardButton("🔨 Добавить правила"), new KeyboardButton("💥 Удалить правила")],
        [new KeyboardButton("📑 Меню")]
    ])
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false
    };

    public static readonly ReplyKeyboardMarkup VpnEditKeyboard = new(
    [
        [new KeyboardButton("⚙️ Изменить настройки")],
        [new KeyboardButton("🐈 Clash файл"), new KeyboardButton("🔗 Ссылка Clash")],
        [new KeyboardButton("♻️ Заменить Clash файл")],
        [new KeyboardButton("📑 Меню")]
    ])
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false
    };


    public static readonly ReplyKeyboardMarkup UsersEditKeyboard = new(
    [
        [new KeyboardButton("👤 Добавить пользователя")],
        [new KeyboardButton("💥 Удалить пользователя")],
        [new KeyboardButton("📑 Меню")]
    ])
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false
    };

}