using System.ComponentModel.DataAnnotations;

namespace RandomPokemon.Models
{
    public class AccountViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "You must enter a valid email address")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
