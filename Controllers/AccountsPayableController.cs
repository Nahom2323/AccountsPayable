using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using accounts_payable.Helpers;
using accounts_payable.Models;
using accounts_payable.Models.DBGenerated;
using accounts_payable.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace accounts_payable.Controllers
{
    public class AccountsPayableController : Controller
    {


        private apContext context;

        public AccountsPayableController(apContext ApContext)
        {
            context = ApContext;
        }

        [HttpGet]
        public IActionResult VendorsList(int nameGroupFilterId = 1)
        {
            var vendorList = new List<Vendor>();
            vendorList = context.Vendors.ToList();
/*            List<string> vendorsNameFilter = vendorList.Select(s => s.VendorName).Distinct().ToList();
*/
            List<NameGroupFilter> nameGroupFiltersList = VendorHelper.GetNameGroupFilters();

/*            vendorsNameFilter.Insert(0, "All");
*/            nameGroupFiltersList.Insert(0, new NameGroupFilter() { NameGroupId = 0, GroupName = "All", LowerLetter = 'A', UpperLetter = 'Z' });


            if(HttpContext.Request.Cookies["groupNameFilterId"] != null)
            {
                HttpContext.Response.Cookies.Append("groupNameFilterId", nameGroupFilterId.ToString());
            }



/*            var selectedvendorFilter = vendorFilter;
*/
              var selectedGroupNameFilterId = nameGroupFilterId;

           /* if(selectedvendorFilter != "All")
            {
                vendorList = vendorList.Where(w => w.VendorName == selectedvendorFilter).ToList();
            }
*/

            if(selectedGroupNameFilterId != 1)
            {
                var selectedNameGroup = nameGroupFiltersList.Where(W => W.NameGroupId == selectedGroupNameFilterId).FirstOrDefault();
                vendorList = vendorList
                    .Where(W => W.VendorName[0] >= selectedNameGroup.LowerLetter
                    && W.VendorName[0] <= selectedNameGroup.UpperLetter)
                    .OrderBy(ob => ob.VendorName).ToList();

            }

            vendorList = vendorList.Where(w => w.IsDeleted == false).ToList();

            vendorListModel vlm = new vendorListModel()
            {
                Vendors = vendorList,
/*                vendorsNameFilter = vendorsNameFilter,
*//*                selectedvendorFilter = selectedvendorFilter,
*/                selectedGroupNameFilterId = selectedGroupNameFilterId,
                NameGroupFilters = nameGroupFiltersList
            };
            return View(vlm);
        }

        public IActionResult SoftDelete(int vendorId)
        {
            var vendorRecod = context.Vendors.Find(vendorId);
            if(vendorRecod != null)
            {
                vendorRecod.IsDeleted = true;
                context.Update(vendorRecod);
                context.SaveChanges();

                TempData["DeletedvendorName"] = vendorRecod.VendorName;
                TempData["vendorID"] = vendorRecod.VendorId;
            }
            return RedirectToAction("VendorsList");
        }
        public IActionResult UndoDelete(int vendorId)
        {
            var vendorDeletedRecod = context.Vendors.Find(vendorId);
            if(vendorDeletedRecod != null)
            {
                vendorDeletedRecod.IsDeleted = false;
                context.Update(vendorDeletedRecod);
                context.SaveChanges();

            }

            return RedirectToAction("VendorsList");
        }

        [HttpGet]
        public IActionResult AddEditVendor(string actionType ,int groupNameFilterId, int vendorId = 0)
        {
            Vendor vendor = null;

            if(actionType == "Add")
            {
                vendor = new Vendor();

            }
            else if(actionType == "Edit")
            {
                vendor = context.Vendors.Find(vendorId);
            }


            HttpContext.Response.Cookies.Append("groupNameFilterId", groupNameFilterId.ToString());

            VendorAddEditViewModel VAEVM = new VendorAddEditViewModel()
            {
                Vendor = vendor,
                ActionName = actionType,
                selectedGroupNameFilterId = groupNameFilterId,
                Terms = context.Terms.ToList(),
                GeneralLedgerAccounts = context.GeneralLedgerAccounts.ToList()
/*                invoiceLineItems = context.InvoiceLineItems.ToList()
*/            };
            return View(VAEVM);
        }

        [HttpPost]
        public IActionResult AddEditVendor(Vendor vendor, string actionName)
        {
            if (ModelState.IsValid)
            {
                //Add
                if(vendor.VendorId == 0)
                {
                    context.Vendors.Add(vendor);
                    context.SaveChanges();

                }
                else
                {
                    //Edit
                    context.Update(vendor);
                    context.SaveChanges();
                }

                return RedirectToAction("VendorsList");
            }
            else
            {
                ModelState.AddModelError("", "The is some errors in the form below.");
                int groupNameFilterId = 0;

                if (HttpContext.Request.Cookies["groupNameFilterId"] != null)
                {
                    groupNameFilterId = int.Parse( HttpContext.Request.Cookies["groupNameFilterId"]);
                }
                VendorAddEditViewModel VAEVM = new VendorAddEditViewModel()
                {
                    Vendor = vendor,
                    ActionName = actionName,
                    selectedGroupNameFilterId = groupNameFilterId,
                    Terms = context.Terms.ToList(),
                    GeneralLedgerAccounts = context.GeneralLedgerAccounts.ToList()


                };
                return View(VAEVM);

            }

        }


        public  IActionResult   VendorInvoices(int vendorId, int  Invoiceid = 0)

        {

            var vendors = context.Vendors.Include(i => i.Invoices);
            
            var vendorRecord = vendors.Where(w => w.VendorId == vendorId).FirstOrDefault();
            var term = context.Terms.Find(vendorRecord.DefaultTermsId);
            
            var account = context.GeneralLedgerAccounts.Find(vendorRecord.DefaultAccountNumber);
            var invoices = vendorRecord.Invoices.ToList();
            var nameGroupFilter = new NameGroupFilter();


            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["groupNameFilter"]))
            {
                int nameGroupFilterId = int.Parse(HttpContext.Request.Cookies["groupNameFilter"]);
                nameGroupFilter = VendorHelper.GetNameGroupFilters()
                    .Where(w => w.NameGroupId == nameGroupFilterId)
                    .FirstOrDefault();
            }

            var selectedInvoiceID = 0;
            Invoice selectedInvoice = null;
            List<InvoiceLineItem> invoiceLineItems = new List<InvoiceLineItem>();

            if (Invoiceid != 0)
            {
                selectedInvoiceID = Invoiceid;
                selectedInvoice = invoices.Where(w => w.InvoiceId == selectedInvoiceID).FirstOrDefault();
                invoiceLineItems = context.InvoiceLineItems.Where(w => w.InvoiceId == selectedInvoiceID).ToList();            
            }
            else if (invoices.Count() > 0)
            {
                selectedInvoiceID = invoices.First().InvoiceId;
                selectedInvoice = invoices.First();
                 invoiceLineItems = context.InvoiceLineItems.Where(w => w.InvoiceId == selectedInvoiceID).ToList();
            }

            VendorInvoices VI = new VendorInvoices()
            {
                Vendor = vendorRecord,
                Term = term.TermsDescription,
                InvoiceNumber = account.AccountDescription,
                NameGroupFilter = nameGroupFilter,
                Invoices = invoices,
                SelectedInvoiceID = selectedInvoiceID,
                SelectedInvoice = selectedInvoice,
                InvoiceLineItems = invoiceLineItems
            };

            return View(VI);

        }


        public IActionResult AddNewInvoiceLineItem(int vendorId, int invoiceId, int invoiceNumber, string description, string amount)
        {
            var vendor = context.Vendors.Find(vendorId);

            var lastInvoiceSequence = context.InvoiceLineItems
                .Where(w => w.InvoiceId == invoiceId)
                .OrderByDescending(obd => obd.InvoiceSequence)
                .Select(s => s.InvoiceSequence)
                .FirstOrDefault();

            lastInvoiceSequence += 1;


            InvoiceLineItem lineItem = new InvoiceLineItem()
            {
                InvoiceId = invoiceId,
                InvoiceSequence = lastInvoiceSequence,
                AccountNumber = vendor.DefaultAccountNumber,
                LineItemAmount = decimal.Parse(amount),
                LineItemDescription = description
            };

            context.InvoiceLineItems.Add(lineItem);
            var invoice = context.Invoices.Find(invoiceId);
            invoice.InvoiceTotal += decimal.Parse(amount);

            context.Invoices.Update(invoice);
            context.SaveChanges();

            return RedirectToAction("VendorInvoices", new { vendorId, invoiceId });
        }

    }
}
