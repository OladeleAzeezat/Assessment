using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentMaui.Model;
class AppSettings
{
    public static string SecretKey { get; set; }
    public static string Issuer { get; set; }
    public static string Audience { get; set; }
    public static string JwtToken { get; set; }

    public static implicit operator AppSettings(string v)
    {
        throw new NotImplementedException();
    }
}

