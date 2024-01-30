namespace Api.Divnolidi.Api.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public int Participant_Count { get; set; }
        public string? Name { get; set; }
        public DateTime Start_Date_Time { get; set; }
        public bool Lock { get; set; }
    }
}
