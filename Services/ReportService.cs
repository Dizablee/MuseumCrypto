using System.Text;
using MuseumCrypto.Models;

namespace MuseumCrypto.Services;

public sealed class ReportService
{
    public void ExportCsv(string path, IEnumerable<AuditLogEntry> logs)
    {
        static string Esc(string s)
        {
            s ??= "";
            if (s.Contains('"') || s.Contains(';') || s.Contains('\n'))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Timestamp;User;Operation;File;Status;Details");

        foreach (var x in logs)
        {
            sb.Append(Esc(x.Timestamp.ToString("u"))).Append(';');
            sb.Append(Esc(x.UserLogin)).Append(';');
            sb.Append(Esc(x.Operation.ToString())).Append(';');
            sb.Append(Esc(x.FileName)).Append(';');
            sb.Append(Esc(x.Status.ToString())).Append(';');
            sb.AppendLine(Esc(x.Details));
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }
}