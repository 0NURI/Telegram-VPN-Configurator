using System.Text;

namespace VpnBot;

public static class BoolChecks
{
    public static bool DomainCheck(string domain)
    {
        return
            domain.Contains('.') &&
            !domain.StartsWith("-") &&
            !domain.StartsWith('.') &&
            domain.All(x => char.IsLetterOrDigit(x) || x == '.' || x == '-') &&
            !domain.EndsWith('.') &&
            !domain.EndsWith('-') &&
            !domain.Contains("..") &&
            !domain.Contains(".-") &&
            !domain.Contains("-.") &&
            domain.Length > 3;
    }

    public static bool UserInQueue(Dictionary<long, bool> queue, long chatId) =>
    queue.TryGetValue(chatId, out var isWaiting) && isWaiting;
}
