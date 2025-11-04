namespace Cipher.Models;

public class EncryptViewModel
{
    // Для encrypt
    public IFormFile? File { get; set; }

    // Для decrypt
    public IFormFile? Rc5File { get; set; }
    public IFormFile? RsaFile { get; set; }
    public string? Password { get; set; }
    public string? Mode { get; set; }
}
