using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_payable.Models.DBGenerated;

namespace accounts_payable.Models.ViewModels
{
    public class vendorListModel
    {
        public List<Vendor> Vendors { get; set; }

        public List<string> vendorsNameFilter { get; set; }

        public string selectedvendorFilter { get; set; }

        public List<NameGroupFilter> NameGroupFilters { get; set; }

        public int selectedGroupNameFilterId { get; set; }


        public string GetActivevendorFilter(string vendorsNameFilter)
        {
            return selectedvendorFilter == vendorsNameFilter ? "active" : "";
        }


        public string GetActiveGroupNameFilter(int groupNameFilterId)
        {
            return selectedGroupNameFilterId == groupNameFilterId ? "active" : "";
        }
    }
}
