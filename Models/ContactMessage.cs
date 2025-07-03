using System;
using System.ComponentModel.DataAnnotations;

namespace AcrylicGame.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
