using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOL_int_list_GUI_v2
{
    class LockFile
    {
        [Key]
        public string FilePath { get; set; }
    }
}
