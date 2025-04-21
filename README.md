# VPN Configurator Bot

🇬🇧 [English](#english) | 🇷🇺 [Русский](#русский)

---

## English

# VPN Configurator Telegram Bot

This Telegram bot allows you to collaboratively manage a VPN configuration (YAML) file that can be used with Clash, Shadowrocket, and similar tools. Each user can contribute domain rules, which are then applied to all clients.

## ✨ Features

- Rule management (add/remove)
- VPN settings configuration
- User/admin control
- YAML config file distribution
- Broadcasting announcements

## 🧩 Technologies Used

- .NET 8
- Telegram.Bot 22.4.4
- C#

## ⚙️ Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- Telegram bot token (get it via [@BotFather](https://t.me/botfather))

## 📦 Installation

1. Clone this repository:
```bash
git clone https://github.com/your-user/your-repo-name.git
cd your-repo-name
```

2. Install .NET 8 if needed:
```bash
sudo apt install dotnet-sdk-8.0
```

3. Fill `config.txt` (one line per setting):
```
<YOUR_BOT_TOKEN>
SUPERADMIN_CHATID_HERE
/path/to/clash.yaml
/path/to/instruction.txt
/path/to/users.txt
/path/to/admins.txt
http://link.to/download/clash.yaml
```

4. Launch bot once and send it any message. It will print your `chat_id`. Replace `SUPERADMIN_CHATID_HERE` with it, then restart the bot.

```bash
dotnet run
```

## 🧪 Example Usage

```
📋 View rules
🔨 Add rule
💥 Remove rule
🚨 Instruction
🌐 VPN info
🐈 Download YAML
🔗 Share YAML link
⚙️ Change VPN settings
👥 View users
👤 Add user
💥 Remove user
🗣 Broadcast
♻️ Replace config file
```

## 🛡 YAML Format

```yaml
proxies:
  - name: "YOR-NAME"
    type: YOUR-TYPE
    server: YOUR-SERVER
    port: YOUR-PORT
    cipher: YOUR-CIPHER
    password: YOUR-PASSWORD

rules:
  - DOMAIN-SUFFIX,const.example,Proxy
#start
  - DOMAIN-SUFFIX,changeable.example,Proxy
  - MATCH,DIRECT
```

Lines after `#start` are editable. Lines above are constants. Do **not** end with `- MATCH,DIRECT` if you want your rules to apply.

## 📄 License

MIT


---

## Русский

# VPN Конфигуратор

Телеграм-бот для управления YAML конфигурацией VPN (Clash, Shadowrocket и др.) с помощью простого и защищённого интерфейса.

## 📌 Возможности

- Добавление и удаление правил обхода блокировок
- Изменение настроек VPN (сервер, порт, шифр, пароль)
- Рассылка объявлений пользователям
- Отправка YAML-файла и ссылки на него
- Удобный контроль доступа по чат ID (пользователи и админы)

## ⚙️ Требования

- .NET 8: https://dotnet.microsoft.com/download
- Telegram Bot API Token (через @BotFather)
- ОС: Windows, Linux, macOS (поддерживается кроссплатформенно)

## 🛠 Установка

1. Установите .NET 8.
2. Клонируйте репозиторий:
```bash
git clone https://github.com/yourname/Telegram-VPN-Configurator.git
```
3. Откройте проект в Visual Studio Code или любом редакторе.
4. Отредактируйте `config.txt`, добавив по строкам:
    1. токен Telegram-бота,
    2. SUPERADMIN_CHATID_HERE (замените после первого запуска),
    3. путь к clash.yaml,
    4. путь к instruction.txt,
    5. путь к users.txt,
    6. путь к admins.txt,
    7. ссылка для на скачивание файла yaml для shadowrocket или другого приложения.
5. Запустите:
```bash
dotnet run
```

После запуска напишите боту — получите свой chat ID, замените им SUPERADMIN_CHATID_HERE в конфиге, перезапустите бота.

## 📋 Доступные команды

Список доступных команд (меню отображается автоматически):
- 📋 Список правил
- 🔨 Добавить правила
- 💥 Удалить правила
- 🌐 Информация о VPN
- 🐈 Clash файл
- 🔗 Ссылка Clash
- ⚙️ Изменить настройки
- 👤 Добавить пользователя
- 💥 Удалить пользователя
- 👥 Список пользователей
- 🗣 Объявление
- ♻️ Заменить CLASH файл

## 📂 YAML структура

Файл `clash.yaml` должен иметь следующую структуру:
```yaml
rules:
  - DOMAIN-SUFFIX,const.example,Proxy
#start
  - DOMAIN-SUFFIX,editable.example,Proxy
```
Всё, что до `#start` — неизменяемо, всё после — редактируется пользователями.

## 📝 Лицензия

MIT
