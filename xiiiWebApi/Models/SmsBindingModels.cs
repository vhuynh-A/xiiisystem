using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace xiiiWebApi.Models
{
    public class SendSmsBindingModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "From")]
        public string From { get; set; }

        [Display(Name = "Message")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false)]
        public string Message { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "To")]
        [Required(AllowEmptyStrings = false)]
        public string To { get; set; }
    }
}