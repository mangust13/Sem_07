using System.Text;

namespace Lab_03
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.Write("Âגוה³עü ןאנמכü: ");
            string password = Console.ReadLine();

            var encryptor = new RC5Encryptor(password, rounds: 12, keyBits: 128);

            encryptor.EncryptFile("input.txt", "encrypted.bin");
            Console.WriteLine("Ôאיכ חארטפנמגאםמ.");

            encryptor.DecryptFile("encrypted.bin", "output.txt");
            Console.WriteLine("Ôאיכ נמחרטפנמגאםמ.");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
