using System.ComponentModel.DataAnnotations;
using StudentPortalSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortalSystem.Chat
{
    public class UserChatChannel
    {
        [Key]
        [Column(Order = 0)]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ChatChannelId { get; set; }

        [ForeignKey("ChatChannelId")]
        public ChatChannel? ChatChannel { get; set; }
    }
}
