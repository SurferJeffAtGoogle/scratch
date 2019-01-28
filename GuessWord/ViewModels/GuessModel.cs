using System.ComponentModel.DataAnnotations;

namespace GuessWord.ViewModels
{
    public class GuessModel
    {
        [Required]
        public string Word { get; set; }
    }
}