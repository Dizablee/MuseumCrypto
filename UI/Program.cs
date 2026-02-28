using MuseumCrypto.Data;
using MuseumCrypto.UI;

namespace MuseumCrypto;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Db.EnsureCreated();

        ApplicationConfiguration.Initialize();
        Application.Run(new LoginForm());
    }
}