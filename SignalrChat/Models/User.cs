using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web;

namespace SignalrChat.Models
{
    public class User
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "*Username is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*Password is required")]
        [MinLength(3, ErrorMessage = "Password must be 3 characters or more"), MaxLength(20)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Empty field or passwords do not match")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }

        public string ConnectionId { get; set; }

        public string Base64Image { get; set; }

        [DisplayName("Upload File")]
        public string ImagePath { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public List<Message> Messages { get; set; }
    }
}