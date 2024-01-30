namespace Api.Divnolidi.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public ulong Id_Dc { get; set; }
        public string? First_Name { get; set; }
        public string? Nation { get; set; }
        public int? Naroz_Den { get; set; }
        public int? Naroz_Mesic { get; set; }
        public string? Dc_Name { get; set; }
        public string? Log_Code { get; set; }
    }
}
