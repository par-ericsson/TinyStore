﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public string StreetAddress { get; set; }
        public string City { get; set; }
    }
}