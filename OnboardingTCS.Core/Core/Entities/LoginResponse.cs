namespace OnboardingTCS.Core.Entities
{
    public class LoginResponse
    {
        public string Mensaje { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public Usuario? Usuario { get; set; }
    }
}
