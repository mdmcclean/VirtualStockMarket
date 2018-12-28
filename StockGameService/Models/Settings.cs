using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockGameService.Models
{
    public class Settings
    {

        [Required]
        [Range(1, 99999, ErrorMessage = "Must be between 1 and 99999")]
        public int Timer { get; set; }


        [Required]
        [Range(100, 99999, ErrorMessage = "Must be between 100 and 99999")]
        public int AvailableShares { get; set; }
    }
}
