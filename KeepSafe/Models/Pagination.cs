using System;
namespace KeepSafe.Models
{
    public class Pagination
    {
        public string Url { get; set; }
        public bool HasNext { get; set; }
        public int Count { get; set; }
    }
}
