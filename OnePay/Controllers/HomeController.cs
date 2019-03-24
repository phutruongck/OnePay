using OnePay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnePay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Tich hop he thong thanh toan onepay vao web asp.net mvc3,4";
            ProductItems productItems = new ProductItems();

            return View(productItems.GetProductItems);
        }

        /// <summary>
        /// Chuyen huong url sang trang thay toan cua onepay
        /// </summary>
        public ActionResult Onepay()
        {
            decimal totalPrices = 0;
            ProductItems productItems = new ProductItems();
            List<Product> lstProducts = productItems.GetProductItems;
            foreach (Product product in lstProducts)
            {
                totalPrices += product.Price;
            }
            string amount = (totalPrices * 100).ToString();
            string url = RedirectOnepay(RandomString(), amount, "192.186.1.7");
            return Redirect(url);
        }

        public ActionResult OnepayResponse()
        {
            string hashvalidateResult = "";

            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(OnepayProperty.URL_ONEPAY_TEST);
            conn.SetSecureSecret(OnepayProperty.HASH_CODE);

            // Xu ly tham so tra ve va du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);

            // Lay tham so tra ve tu cong thanh toan
            string vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode");
            string amount = conn.GetResultField("vpc_Amount");
            string localed = conn.GetResultField("vpc_Locale");
            string command = conn.GetResultField("vpc_Command");
            string version = conn.GetResultField("vpc_Version");
            string cardType = conn.GetResultField("vpc_Card");
            string orderInfo = conn.GetResultField("vpc_OrderInfo");
            string merchantID = conn.GetResultField("vpc_Merchant");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef");
            string transactionNo = conn.GetResultField("vpc_TransactionNo");
            string acqResponseCode = conn.GetResultField("vpc_AcqResponseCode");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message");

            // Kiem tra 2 tham so tra ve quan trong nhat
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                return View("PaySuccess");
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
            {
                return View("PayPending");
            }
            else
            {
                return View("PayUnSuccess");
            }
        }

        /// <summary>
        /// View thong tin the
        /// </summary>
        public PartialViewResult InforBase()
        {
            return PartialView();
        }

        /// <summary>
        /// Sinh ky tu ngau nhien
        /// </summary>
        private string RandomString()
        {
            var str = new StringBuilder();
            var random = new Random();
            for (int i = 0; i <= 5; i++)
            {
                var c = Convert.ToChar(Convert.ToInt32(random.Next(65, 68)));
                str.Append(c);
            }
            return str.ToString().ToLower();
        }

        /// <summary>
        /// Redirect den onepay
        /// </summary>
        public string RedirectOnepay(string codeInvoice, string amount, string ip)
        {
            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(OnepayProperty.URL_ONEPAY_TEST);
            conn.SetSecureSecret(OnepayProperty.HASH_CODE);

            // Gan cac thong so de truyen sang cong thanh toan onepay
            conn.AddDigitalOrderField("AgainLink", OnepayProperty.AGAIN_LINK);
            conn.AddDigitalOrderField("Title", "Tich hop onepay vao web asp.net mvc3,4");
            conn.AddDigitalOrderField("vpc_Locale", OnepayProperty.PAYGATE_LANGUAGE);
            conn.AddDigitalOrderField("vpc_Version", OnepayProperty.VERSION);
            conn.AddDigitalOrderField("vpc_Command", OnepayProperty.COMMAND);
            conn.AddDigitalOrderField("vpc_Merchant", OnepayProperty.MERCHANT_ID);
            conn.AddDigitalOrderField("vpc_AccessCode", OnepayProperty.ACCESS_CODE);
            conn.AddDigitalOrderField("vpc_MerchTxnRef", RandomString());
            conn.AddDigitalOrderField("vpc_OrderInfo", codeInvoice);
            conn.AddDigitalOrderField("vpc_Amount", amount);
            conn.AddDigitalOrderField("vpc_ReturnURL", Url.Action("OnepayResponse", "Home", null, Request.Url.Scheme, null));

            // Thong tin them ve khach hang. De trong neu khong co thong tin
            conn.AddDigitalOrderField("vpc_SHIP_Street01", "");
            conn.AddDigitalOrderField("vpc_SHIP_Provice", "");
            conn.AddDigitalOrderField("vpc_SHIP_City", "");
            conn.AddDigitalOrderField("vpc_SHIP_Country", "");
            conn.AddDigitalOrderField("vpc_Customer_Phone", "");
            conn.AddDigitalOrderField("vpc_Customer_Email", "");
            conn.AddDigitalOrderField("vpc_Customer_Id", "");
            conn.AddDigitalOrderField("vpc_TicketNo", ip);

            string url = conn.Create3PartyQueryString();
            return url;
        }

        public ActionResult About()
        {
            return View();
        }
    }
}