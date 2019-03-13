using EmirApp.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmirApp.Models
{
    public class Person : ISoftDelete
    {
        //9.03.2019
        //validacija na backendu
        //Ubacivanje uslova da prvo slovo imena i prezime bude velikim slovom bez obzira sta user ukuca
        private string _LastName;
        private string _FirstName;
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName {
            get
            {
                if (string.IsNullOrEmpty(_LastName))
                {
                    return _LastName;
                }
                return char.ToUpper(_LastName[0]) + _LastName.Substring(1);
            }
            set
            {
                _LastName = value;
            }
        }
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstMidName {
            get
            {
                if (string.IsNullOrEmpty(_FirstName))
                {
                    return _FirstName;
                }
                return char.ToUpper(_FirstName[0]) + _FirstName.Substring(1);
            }
            set
            {
                _FirstName = value;
            }
        }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }

        public bool IsDeleted { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}