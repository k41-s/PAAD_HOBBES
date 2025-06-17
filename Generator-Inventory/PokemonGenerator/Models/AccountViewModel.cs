using System.ComponentModel.DataAnnotations;

namespace PokemonGenerator.Models
{
    public class AccountViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
