namespace VpnBot;

public static class SessionState
{
    public static Dictionary<long, bool> RulesAddingQueue = [];
    public static Dictionary<long, bool> RulesDeletingQueue = [];
    public static Dictionary<long, bool> VPNChangingQueue = [];
    public static Dictionary<long, bool> UserAddingQueue = [];
    public static Dictionary<long, bool> UserDeletingQueue = [];
    public static Dictionary<long, bool> SpeakingQueue = [];
    public static Dictionary<long, bool> FileQueue = [];
    public static Dictionary<long, List<string>> UserRuleSnapshots = [];
    public static bool isProcessing = false;
    public static object queueLock = new();
    public static Queue<Func<Task>> EditQueue = new();


    public static async Task ProcessQueue()
    {
        while (true)
        {
            Func<Task> next;
            lock (queueLock)
            {
                if (EditQueue.Count == 0)
                {
                    isProcessing = false;
                    return;
                }
                next = EditQueue.Dequeue();
            }
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Ошибка в задаче: {ex.Message}");
            }
        }
    }

    public static void EnqueueEdit(Func<Task> action)
    {
        lock (queueLock)
        {
            EditQueue.Enqueue(action);
            if (!isProcessing)
            {
                isProcessing = true;
                _ = ProcessQueue();
            }
        }
    }
}
