using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReportesWeb1_2.ModelsReportes
{
    public class Users
    {
        [StringLength(128)]
        public string Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Role { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string PasswordConfirm { get; set; }
    }
}