using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Models
{
    [Keyless]
    public class KalendarFakt
    {
        public int? Den { get; set; }
        public int? Mesic { get; set; }
        public string? Fakt { get; set; }
    }
}
