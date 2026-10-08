using System.ComponentModel.DataAnnotations;

namespace WebApplication26.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Назва події")]
        public string Title { get; set; } = "";

        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Дата та час")]
        public DateTime EventDate { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email користувача")]
        public string UserEmail { get; set; } = "";

        public bool NotificationSent { get; set; }
    }
}
