using System.ComponentModel.DataAnnotations;

namespace VolleyballTools
{
    public class Player
    {
        [Required]
        public int? Number { get; set; } = null;
        public string? Name { get; set; } = null;
    }
}
