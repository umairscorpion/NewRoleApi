using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class FileManager
    {
        public string UserId { get; set; }
        public string AttachedFileName { get; set; }
        public string FileContentType { get; set; }
        public string FileExtention { get; set; }
        public string AttachedFileId { get; set; }
        public int DistrictId { get; set; }
        public int FileName { get; set; }
        public DateTime Date { get; set; }
    }
}
