using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    public class Master_Role
    {
        [Key]
        public int RoleKey { get; set; }
        public string RoleName { get; set; }
    }
}