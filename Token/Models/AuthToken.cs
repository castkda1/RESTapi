namespace Api.Token.Models
{
    public class AuthToken
    {
        public long Id { get; set; }
        public string User_name { get; set; }
        public string Auth_token { get;  set; }
    }
}
