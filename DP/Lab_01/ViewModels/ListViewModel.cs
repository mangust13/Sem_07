using System.ComponentModel.DataAnnotations;

namespace Lab_01.ViewModels;

public class ListViewModel
{
    [Display(Name = "Введіть елементи через пробіл (числа/символи/слова)")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string Input { get; set; } = "";

    public string? ParsedAs { get; set; }
    public string? ResultAs { get; set; }
    public int? ZeroCount { get; set; }

    public string? HeadValue { get; set; }
    public string? TailAs { get; set; }

}
