using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_payable.Helpers;
using accounts_payable.Models.DBGenerated;
using Microsoft.AspNetCore.Mvc;

namespace accounts_payable.Controllers
{
    public class ValidationController : Controller
    {

        private apContext context;
        public ValidationController(apContext apDBContext)
        {
            context = apDBContext;
        }

        public JsonResult CheckPhoneNumber(Vendor vendor)
        {
            string msg = validationHelper.PhoneNumberExists(context, vendor.VendorPhone);
            if (string.IsNullOrEmpty(msg))
            {
                TempData["okPhoneNumber"] = true;
                return Json(true);
            }
            else
            {
                return Json(msg);
            }
        }
    }
}
