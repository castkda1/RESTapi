namespace Api.Divnolidi.Api.Models
{
    public class LolAccount
    {
        public int Id { get; set; }
        public int? Id_User { get; set; }
        public string? Id_Summoner { get; set; }
        public string? Id_Account { get; set; }
        public string? Id_Puu { get; set; }
        public string? Tier { get; set; }
        public string? Rank { get; set; }
        public string? Summoner_Name { get; set; }
        public int? Mmr { get; set; }
        public string? Region { get; set; }
    }
}
