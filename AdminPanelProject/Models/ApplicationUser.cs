using Microsoft.AspNetCore.Identity;

namespace AdminPanelProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string AvatarUrl { get; set; } // Kullanıcı avatarı
        public DateTime DateOfBirth { get; set; } // Doğum tarihi
        public string Address { get; set; } // Kullanıcı adresi
        public string City { get; set; } // Kullanıcı şehri
        public string Country { get; set; } // Kullanıcı ülkesi
        public string Gender { get; set; } // Kullanıcı cinsiyeti
        public string PreferredLanguage { get; set; } // Tercih edilen dil
        public DateTime LastLoginDate { get; set; } // Son giriş tarihi
        public bool IsActive { get; set; } // Kullanıcı aktif mi?
        public bool ReceiveNotifications { get; set; } // Kullanıcı bildirim almak istiyor mu?
    }
}
