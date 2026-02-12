using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Models
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est requis")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le contenu est requis")]
        public string Content { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        // Relation avec ApplicationUser
       
        public string? UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}




























