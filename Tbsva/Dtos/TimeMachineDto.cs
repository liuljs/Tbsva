using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class TimeMachineDto
    {
        public int id { get; set; }
        public Guid timeMachineId { get; set; }
        public string name { get; set; }
        public string course { get; set; }
        public string brief { get; set; }
        public byte first { get; set; }
        public int sort { get; set; }
        public byte enabled { get; set; }
        public string imageURL01 { get; set; }
        public string imageURL02 { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string creationDate { get; set; }
        //public DateTime? updatedDate { get; set; }
    }
}