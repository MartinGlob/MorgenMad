using System.Collections.Generic;

namespace mm.Models
{
    public class BreakfastsView
    {
        public BreakfastsView()
        {
            Breakfasts = new List<Breakfast>();
        }

        public List<Breakfast> Breakfasts { get; set; }
    }
}
