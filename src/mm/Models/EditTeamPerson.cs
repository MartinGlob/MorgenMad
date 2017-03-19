using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mm.Models
{
    public class EditTeamPerson
    {
        public string Message { get; set; }

        public IList<SelectListItem> Teams { get; set; }
        public string TeamName { get; set; }
        public DayOfWeek Day { get; set; }
        public string TeamId { get; set; }

        public string Id { get; set; }
        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
