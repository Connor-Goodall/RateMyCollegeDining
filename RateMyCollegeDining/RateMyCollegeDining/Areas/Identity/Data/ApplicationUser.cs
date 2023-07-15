using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RateMyCollegeDining.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string? Name { get; set; }

    [PersonalData]
    public string? Bio { get; set; }

    [PersonalData]
    public string? University { get; set; }

    public DateTime Date_Joined { get; set; }

    public byte[]? ProfilePicture { get; set; }


}

