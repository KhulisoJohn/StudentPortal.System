
using System.ComponentModel.DataAnnotations;
using StudentPortalSystem.Models;
using System;

namespace StudentPortalSystem.Chat
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }

        public string? UserId { get; set; }  // foreign key to ApplicationUser

        public ApplicationUser? User { get; set; }  // navigation property

        public int ChatChannelId { get; set; }
        public ChatChannel? ChatChannel { get; set; }

        public string? Message { get; set; }

        public DateTime SentAt { get; set; }
    }
}
