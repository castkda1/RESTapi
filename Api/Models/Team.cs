namespace Api.Divnolidi.Api.Models
{
    public class Team
    {
        public int Id { get; set; }
        public int Id_Tournament { get; set; }
        public int Player1 { get; set; }
        public int Player2 { get; set; }
        public int Player3 { get; set; }
        public int Player4 { get; set; }
        public int Player5 { get; set; }
        public string Name { get; set; }
    }
}
