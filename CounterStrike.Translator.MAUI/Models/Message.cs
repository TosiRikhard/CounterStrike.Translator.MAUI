using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrike.Translator.MAUI.Services;

namespace CounterStrike.Translator.MAUI.Models;
public class Message
{
    public string Sender { get; set; }
    public bool SenderIsUser { get; set; }
    public string MessageText { get; set; }
    public TelnetService.ChatType SourceChatType { get; set; }
    public TelnetService.ChatType DestinationChatType { get; set; } = TelnetService.ChatType.None;
    public string CommandType { get; set; } = string.Empty;
    public string LanguageParameter { get; set; } = string.Empty;

    public bool ValidMessage { get; set; } = true;

}
