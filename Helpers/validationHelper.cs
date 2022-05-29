using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using accounts_payable.Models.DBGenerated;

namespace accounts_payable.Helpers
{
    public class validationHelper
    {
       public static string PhoneNumberExists(apContext context, string vendorPhone)
        {
            string msg = "";
            if (!string.IsNullOrEmpty(vendorPhone))
            {
                string numericPhoneNumber = new string(vendorPhone.Where(char.IsDigit).ToArray());
                bool match = false;
                foreach (var vendor in context.Vendors.ToList())
                {
                    if(vendor.VendorPhone != null)
                    {
                        string vendorPhoneNumber = new string(vendor.VendorPhone.Where(char.IsDigit).ToArray());
                        match = vendorPhoneNumber == numericPhoneNumber;

                        if (match)
                        {
                            break;
                        }
                    }
                }
                if (match)
                    msg = $"Phone number {vendorPhone}is already in use.";
            }
            return msg;
        }
    }
}
