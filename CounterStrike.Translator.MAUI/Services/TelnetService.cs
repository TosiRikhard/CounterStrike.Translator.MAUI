namespace CounterStrike.Translator.MAUI.Services;
public class TelnetService
{
    public enum ChatType
    {
        All,
        Team
    }

    public readonly TelnetConnection TelnetConnection;

    private readonly SemaphoreSlim _sendChatSemaphore = new SemaphoreSlim(1, 1);

    public bool Connected => TelnetConnection.Connected;

    public TelnetService(TelnetConnection telnetConnection)
    {
        TelnetConnection = telnetConnection;
        TelnetConnection.Connect();
    }

    public async Task<bool> ExecuteCsgoCommand(string command)
    {
        if (!Connected) return false;

        await TelnetConnection.ExecuteCommand(command);

        return true;
    }

    public async Task<bool> SendInChat(ChatType chatType, string message)
    {
        await _sendChatSemaphore.WaitAsync();

        try
        {
            await Task.Delay(1000);
            return chatType switch
            {
                ChatType.All => await ExecuteCsgoCommand($"say {message}"),
                ChatType.Team => await ExecuteCsgoCommand($"say_team {message}"),
                _ => false,
            };
        }
        finally
        {
            _sendChatSemaphore.Release();
        }
    }
}