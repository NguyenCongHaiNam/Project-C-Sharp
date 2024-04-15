using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace FE.Models
{
    public partial class Log
    {
        [Key]
        public int logID { get; set; }
        public int idUser { get; set; }
        public string logContent { get; set; }
        public DateTime dateTime { get; set; }
        // Các trường khác bạn muốn lưu
    }
}
