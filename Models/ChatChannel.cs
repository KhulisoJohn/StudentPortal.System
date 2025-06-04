using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class ChatChannel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Grade { get; set; }  // Channel per grade

        public int? SubjectId { get; set; }  // Optional channel per subject

        public Subject? Subject { get; set; }

        public ICollection<ChatMessage>? ChatMessages { get; set; }


        public ICollection<UserChatChannel> UserChatChannels { get; set; } = new List<UserChatChannel>();

        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
