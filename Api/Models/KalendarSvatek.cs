using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Models
{
    [Keyless]
    public class KalendarSvatek
    {
        public int? Den { get; set; }
        public int? Mesic { get; set; }
        public string? Svatek { get; set; }
    }
}
