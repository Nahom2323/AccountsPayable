using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace accounts_payable.Models
{
    public class NameGroupFilter
    {
        public int NameGroupId { get; set; }

        public string GroupName { get; set; }

        public char LowerLetter { get; set; }

        public char UpperLetter { get; set; }
    }
}
