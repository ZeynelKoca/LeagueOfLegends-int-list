using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOL_int_list_GUI_v2
{
    class Summoner
    {
        [Key]
        public string summonerName { get; set; }
    }
}
