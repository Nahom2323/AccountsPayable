using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_payable.Models.DBGenerated;

namespace accounts_payable.Models.ViewModels
{
    public class VendorAddEditViewModel
    {
        public Vendor Vendor { get; set; }

        public string ActionName { get; set; }
        public List<Term> Terms { get; set; }

        /*        public List<InvoiceLineItem> invoiceLineItems { get; set; }
        */
        public int selectedGroupNameFilterId { get; set; }

        public List<GeneralLedgerAccount> GeneralLedgerAccounts { get; set; }

    }
}
