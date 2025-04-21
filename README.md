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

## ⚙️ Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- Telegram bot token (get it via [@BotFather](https://t.me/botfather))

## 📦 Installation

1. Clone this repository:
```bash
git clone https://github.com/0NURI/Telegram-VPN-Configurator.git
cd Telegram-VPN-Configurator
```

2. Install .NET 8 if needed:
```bash
sudo apt install dotnet-sdk-8.0
```

3. Edit `config.txt` (one line per setting):
```
BOT_TOKEN_HERE - TG Bot token, you get in when you create a bot via @BotFather.
SUPERADMIN_CHATID_HERE - Do not edit right now.
/path/to/clash.yaml - Path to your yaml file. Use and edit the provided yaml file. Keep in mind not to delete #start.
/path/to/instruction.txt - Path to your instruction file. Create one yourself or use the one provided.
/path/to/users.txt - Path to an empty users.txt file, create it by yourself.
/path/to/admins.txt - Path to an empty admins.txt file, create it by yourself.
http://link.to/download/clash.yaml - The link shadowrocket or clash will use to download the config. You can add it here when you implement this opportunity (using nginx for example).
```

4. Launch bot once and send it any message. It will print your `chat_id` in the console. Replace `SUPERADMIN_CHATID_HERE` with it, then restart the bot.

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

## ✨ Возможности

- Добавление и удаление правил обхода блокировок
- Изменение настроек VPN (сервер, порт, шифр, пароль)
- Рассылка объявлений пользователям
- Отправка YAML-файла и ссылки на него
- Удобный контроль доступа по чат ID (пользователи и админы)

## ⚙️ Требования

- .NET 8
- Telegram.Bot 22.4.4
  
## 🛠 Установка

1. Клонируйте репозиторий:
```bash
git clone https://github.com/0NURI/Telegram-VPN-Configurator.git
cd Telegram-VPN-Configurator
```

2. Установите .NET 8:
```bash
sudo apt install dotnet-sdk-8.0
```

3. Отредактируйте `config.txt`:
```
BOT_TOKEN_HERE - Токен телеграм бота, можно получить при создании бота через @BotFather.
SUPERADMIN_CHATID_HERE - На данном этапе не редактируйте.
/path/to/clash.yaml - Путь к вашему yaml файлу. Редактируйте и используте предоставленный  шаблон. Обратите внимание, что нельзя удалять строку #start.
/path/to/instruction.txt - Путь к вашему файлу с инструкцией для пользователей. Создайте файл самостоятельно или используйте предоставленный.
/path/to/users.txt - Путь к файлу с пользователями. Создайте его, но оставьте пустым.
/path/to/admins.txt - Путь к файлу с админами. Создайте его, но оставьте пустым.
http://link.to/download/clash.yaml - Ссылка на скачивание yaml файла для подгрузки в shadowrocket/clash. Можете добавить как реализуете такую возможность (например через nginx).
```

4. Запустите бота и отправьте ему сообщение. В консоли будет написан ваш `chat_id`. Теперь замените `SUPERADMIN_CHATID_HERE` в `confix.txt` и перезапустите бота.

```bash
dotnet run
```

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
- ♻️ Заменить Clash файл

## 📂 YAML структура

Файл `clash.yaml` должен иметь следующую структуру:
```yaml
rules:
  - DOMAIN-SUFFIX,const.example,Proxy
#start
  - DOMAIN-SUFFIX,editable.example,Proxy
```
Всё, что до `#start` — не будет выведено на изменение, всё после — редактируется пользователями.

## 📝 Лицензия

MIT
