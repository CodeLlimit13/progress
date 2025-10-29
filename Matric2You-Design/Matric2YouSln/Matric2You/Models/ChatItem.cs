using Microsoft.Maui.Controls;

namespace Matric2You.Models
{
    public class ChatItem
    {
        public bool IsUser { get; set; }
        public string Role { get; set; } = string.Empty; // "You" or "Bot"
        public string Content { get; set; } = string.Empty; // Plain text content for user or label for bot
        public HtmlWebViewSource? HtmlSource { get; set; } // For bot messages rendered with KaTeX
    }
}
