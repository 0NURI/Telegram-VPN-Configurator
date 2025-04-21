using static VpnBot.FileManager;

namespace VpnBot
{
    public static class BotConfig
    {
        internal static string BOT_TOKEN = null!;
        internal static long SUPERADMIN_ID;
        internal static string yamlPath = null!;
        internal static string instructionPath = null!;
        internal static string usersPath = null!;
        internal static string adminsPath = null!;
        internal static string yamlLink = null!;
        public static async Task Initialise()
        {
            var conf = await DownloadFileAsLines("config.txt");
            BOT_TOKEN = conf[0];
            usersPath = conf[4];
            adminsPath = conf[5];
            SUPERADMIN_ID = long.Parse(conf[1]);
            var Users = await LoadUsers(usersPath);
            var Admins = await LoadUsers(adminsPath);
            if(!Users.ContainsKey(SUPERADMIN_ID)) await UploadFileFromLines(usersPath, [$"{SUPERADMIN_ID},Admin"]);
            if(!Admins.ContainsKey(SUPERADMIN_ID)) await UploadFileFromLines(adminsPath, [$"{SUPERADMIN_ID}"]);
            yamlPath = conf[2];
            instructionPath = conf[3];
            yamlLink = conf[6];
        }
    }
}
