using System.Net.Sockets;
using System.Text;

namespace CounterStrike.Translator.MAUI.Services;

public class TelnetConnection
{
    private const string _hostname = "localhost";
    private const int _port = 2121;
    private TcpClient _tcpSocket;

    public bool Connected => _tcpSocket is { Connected: true };

    // Event to notify subscribers when a message is received (Counter strike console receives new line).
    public event Action<string> MessageReceived;

    // Method to initiate the connection and start the reading loop.
    public void Connect()
    {
        try
        {
            _tcpSocket = new TcpClient(_hostname, _port);
            // Starting the ReadLoopAsync on a separate Task for non-blocking asynchronous operation.
            Task.Factory.StartNew(ReadLoopAsync, TaskCreationOptions.LongRunning);
        }
        catch (Exception ex)
        {
            // Log the exception
        }
    }

    // Method to send a command to the server.
    public async Task ExecuteCommand(string command)
    {
        if (!Connected)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error",
                "Telnet not connected. Check Launch options for -netconport 2121", "Ok");
            return;
        }

        var utf8 = Encoding.UTF8;
        // Convert the command to bytes and escape any IAC characters.
        var buf = utf8.GetBytes((command + "\n").Replace("\0xFF", "\0xFF\0xFF"));

        try
        {
            _tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    // Asynchronous method to continuously read messages from the server.
    private async Task ReadLoopAsync()
    {
        while (Connected)
        {
            var message = await ReadTelnetAsync();
            // Invoke the event to notify subscribers of the received message.
            await Task.Delay(200);

            if (string.IsNullOrWhiteSpace(message)) continue;

            MessageReceived?.Invoke(message);
            // Adding a delay to prevent tight loop and high CPU usage.
        }
    }

    // Method to read a response from the server.
    private async Task<string> ReadTelnetAsync()
    {
        if (!Connected) return null;

        var sb = new StringBuilder();
        do
        {
            ParseTelnet(sb);
            // Adding a delay to prevent tight loop and high CPU usage.
            await Task.Delay(200);
        } while (_tcpSocket.Available > 0);

        return sb.ToString();
    }

    // Method to parse the Telnet protocol.
    private void ParseTelnet(StringBuilder sb)
    {
        var buffer = new List<byte>();
        while (_tcpSocket.Available > 0)
        {
            var input = ReadFromSocket();
            // Handle the input depending on its value.
            HandleInput(input, buffer, sb);
        }

        // Decode any remaining bytes in the buffer into a string and append them to the StringBuilder.
        if (buffer.Count > 0)
        {
            var str = Encoding.UTF8.GetString(buffer.ToArray());
            sb.Append(str);
        }
    }

    // Method to handle the input based on its value.
    private void HandleInput(int input, List<byte> buffer, StringBuilder sb)
    {
        switch (input)
        {
            case -1:
                // Do nothing if the input is -1.
                return;
            case (int)Verbs.Iac:
                // If the input is an IAC character, decode any bytes in the buffer into a string and append them to the StringBuilder.
                if (buffer.Count > 0)
                {
                    var str = Encoding.UTF8.GetString(buffer.ToArray());
                    sb.Append(str);
                    buffer.Clear();
                }

                ProcessCommand(sb);
                break;
            default:
                // Add the input byte to the buffer.
                buffer.Add((byte)input);
                break;
        }
    }

    // Method to read a single byte from the socket.
    private int ReadFromSocket()
    {
        return _tcpSocket.GetStream().ReadByte();
    }

    // Method to process a command received from the server.
    private void ProcessCommand(StringBuilder sb)
    {
        var inputVerb = ReadFromSocket();

        // Return early if the inputVerb is -1.
        if (inputVerb == -1)
            return;

        // If the inputVerb is an IAC character, append it to the StringBuilder.
        if (inputVerb == (int)Verbs.Iac)
            sb.Append(inputVerb);
        // If the inputVerb is a command, respond to it.
        else if (IsCommand(inputVerb))
            RespondToCommand(inputVerb);
    }

    // Check if the inputVerb is a command.
    private static bool IsCommand(int inputVerb)
    {
        return inputVerb is (int)Verbs.Do or (int)Verbs.Dont or (int)Verbs.Will or (int)Verbs.Wont;
    }

    // Respond to a command from the server.
    private void RespondToCommand(int inputVerb)
    {
        var inputOption = ReadFromSocket();

        // Return early if the inputOption is -1.
        if (inputOption == -1)
            return;

        // Write the IAC character to the stream.
        _tcpSocket.GetStream().WriteByte((byte)Verbs.Iac);

        // If the inputOption is SGA, respond with WILL or DO.
        if (inputOption == (int)Options.Sga)
            _tcpSocket.GetStream().WriteByte(inputVerb == (int)Verbs.Do ? (byte)Verbs.Will : (byte)Verbs.Do);
        // If the inputOption is not SGA, respond with WONT or DONT.
        else
            _tcpSocket.GetStream().WriteByte(inputVerb == (int)Verbs.Do ? (byte)Verbs.Wont : (byte)Verbs.Dont);

        // Write the inputOption to the stream.
        _tcpSocket.GetStream().WriteByte((byte)inputOption);
    }

    // Enumeration of the Telnet command verbs.
    private enum Verbs
    {
        Will = 251,
        Wont = 252,
        Do = 253,
        Dont = 254,
        Iac = 255
    }

    // Enumeration of the Telnet options.
    private enum Options
    {
        Sga = 3
    }
}