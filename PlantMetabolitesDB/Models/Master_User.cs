using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_User")]
    public class Master_User
    {   
        [Key]
        public Int16 UserKey { get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public Int16 UserType { get; set; }
        public string Password { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullName { get; private set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Int16 CountryKey { get; set; }
        public Int16? OrganizationTypeKey { get; set; }
        public string Affiliation { get; set; }
        public string Mobile { get; set; }
        public bool IsEmailVerified { get; set; }
        public string VerificationCode { get; set; }
        public Int64 cntLogin { get; set; }

        public bool IsActive { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        [ForeignKey("OrganizationTypeKey")]
        public virtual Master_OrganizationType OrganizationType { get; set; }
        [ForeignKey("CountryKey")]
        public virtual Master_Country Country { get; set; }

        public string HashPassword { get; set; }
        public string SaltPassword { get; set; }

    }
}