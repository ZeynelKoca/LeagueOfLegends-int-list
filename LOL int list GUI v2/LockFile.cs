using System.ComponentModel.DataAnnotations;

namespace LOL_int_list_GUI_v2
{
    class LockFile
    {
        [Key]
        public string FilePath { get; set; }
    }
}
