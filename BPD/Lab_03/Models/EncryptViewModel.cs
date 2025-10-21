namespace Lab_03.Models;

public class EncryptViewModel
{
    public string Password { get; set; }
    public IFormFile File { get; set; }
    public string Mode { get; set; }
}
