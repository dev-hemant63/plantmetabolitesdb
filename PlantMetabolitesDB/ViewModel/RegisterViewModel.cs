using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;

namespace PlantMetabolitesDB.ViewModel
{
    public class RegisterViewModel : CommonViewModel
    {
        public Int16 UserKey { get; set; }
        [DisplayName("Salutation")]
        public string Salutation { get; set; }
        public string Code { get; set; }
        [DisplayName("User Name (Email Id)")]
        public string Username { get; set; }
        [DisplayName("User Type")]
        public Int16 UserType { get; set; }
        [DisplayName("Password")]
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        [DisplayName("First Name")]
        public string FName { get; set; }
        [DisplayName("Last Name")]
        public string LName { get; set; }
        public string FullName { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Phone")]
        public string Phone { get; set; }
        [DisplayName("Country")]
        public Int16 CountryKey { get; set; }
        [DisplayName("Organization Type")]
        public Int16 OrganizationTypeKey { get; set; }
        [DisplayName("Affiliation")]
        public string Affiliation { get; set; }
        [DisplayName("Mobile")]
        public string Mobile { get; set; }
        public bool IsEmailVerified { get; set; }
        public string VerificationCode { get; set; }
        public Int64 cntLogin { get; set; }
        public string CountryName { get; set; }
        public string OrganizationTypeName { get; set; }
        public List<OrganizationTypeViewModel> OrganizationTypes { get; set; }
        public List<Master_Country> Countries { get; set; }
        public string UserTypeName { get; set; }
        public List<SelectListItem> UserTypes { get; set; }
    }
}