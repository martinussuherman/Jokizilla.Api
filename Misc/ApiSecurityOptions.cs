namespace Jokizilla.Api.Misc
{
    public class ApiSecurityOptions
    {
        public const string OptionsName = "ApiSecurity";

        public string Authority { get; set; }
        public string Audience { get; set; }
    }
}