using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using System.Configuration;
using System.Data;
using AccountManagement.Models;
using PagedList;

namespace AccountManagement.Models.Controllers
{
    public class HomeController : Controller
    {
        public static List<AccountManagementViewModel> _accountData ;

        public ActionResult Index(int? page,string searchKey,string keyType)
        {
            int pageSize = 3;
            if (string.IsNullOrEmpty(searchKey))
            {
                var data = (_accountData == null || _accountData.Count == 0) ? ReadExcelData() : _accountData;                
                int pageNumber = (page ?? 1);
                var accountManagement = new AccountManagement()
                {
                    AccountManagementModel = data.ToPagedList(pageNumber, pageSize),                    
                };
                return View(accountManagement);
            }
            else
            {
                var accountData = new List<AccountManagementViewModel>();
                ViewBag.CurrentFilter = searchKey;                
                int pageNumber;
                GetAccounts(searchKey, keyType, page, out accountData, out pageSize, out pageNumber);              
                var accountManagement = new AccountManagement()
                {
                    AccountManagementModel = accountData.ToPagedList(pageNumber, pageSize),
                    IsAccount = (keyType == "Account") ? true : false,
                    IsBAName = (keyType == "BAName") ? true : false,
                    IsCustomer =  (keyType == "Customer") ? true : false,
                    IsProjectName = (keyType == "ProjectName") ? true : false,
                    IsTechStack = (keyType == "TechStack") ? true : false,
                    SearchKey = searchKey

                };
                return View(accountManagement);
            }

        }

        [HttpGet]
        public ActionResult AccountSearch(string searchKey,string keyType)
        {
            var accountData = new List<AccountManagementViewModel>();
            ViewBag.CurrentFilter = searchKey;
            ViewBag.KeyType = keyType;
            int pageSize, pageNumber;
            GetAccounts(searchKey, keyType, 0 ,out accountData, out pageSize, out pageNumber);
            return PartialView("~/Views/AccountGrid.cshtml", accountData.ToPagedList(pageNumber, pageSize));
        }

        private static void GetAccounts(string searchKey, string keyType, int? page, out List<AccountManagementViewModel> accountData, out int pageSize, out int pageNumber)
        {
            switch (keyType)
            {
                case "Account":
                    accountData = _accountData.Where(p => p.Account.Contains(searchKey) ||
                                                          p.Account.Contains(searchKey.ToLower()) ||
                                                          p.Account.Contains(searchKey.ToUpper())).ToList();
                    break;
                case "ProjectName":
                    accountData = _accountData.Where(p => p.ProjectName.Contains(searchKey) ||
                                                          p.ProjectName.Contains(searchKey.ToLower()) ||
                                                          p.ProjectName.Contains(searchKey.ToUpper())).ToList();
                    break;
                case "Solution":
                    accountData = _accountData.Where(p => p.Solution.Contains(searchKey) ||
                                                          p.Solution.ToLower().Contains(searchKey.ToLower()) ||
                                                          p.Solution.ToUpper().Contains(searchKey.ToUpper())||
                                                          p.Solution.StartsWith(searchKey)).ToList();
                    break;
                case "IndicativePrimaryTechnology":
                    accountData = _accountData.Where(p => p.IndicativePrimaryTechnology.Contains(searchKey) ||
                                                          p.IndicativePrimaryTechnology.Contains(searchKey.ToLower()) ||
                                                          p.IndicativePrimaryTechnology.Contains(searchKey.ToUpper())).ToList();
                    break;
                
                default:
                    accountData = _accountData.Where(p => p.Account.Contains(searchKey) ||
                                                          p.Account.Contains(searchKey.ToLower()) ||
                                                          p.Account.Contains(searchKey.ToUpper())).ToList();
                    break;
            }
            
            page = page > 0 ? page : 1;
            pageSize = 10;
            pageNumber = (page ?? 1);
        }

        [HttpPost]
        public ActionResult AutoComplete(string searchKey, string keyType)
        {
            var accountData = new List<string>();
            switch (keyType)
            {
                case "Account":
                    accountData = _accountData.Where(p => p.Account.StartsWith(searchKey.ToUpper())).Select(p=>p.Account.ToUpper()).ToList();
                    break;
                case "ProjectName":
                    accountData = _accountData.Where(p => p.ProjectName.StartsWith(searchKey.ToUpper())).Select(p => p.ProjectName.ToUpper()).ToList();
                    break;
                case "Solution":
                    accountData = _accountData.Where(p => p.Solution.StartsWith(searchKey) || p.Solution.StartsWith(searchKey.ToUpper())|| p.Solution.StartsWith(searchKey.ToLower())).Select(p => p.Solution.ToLower()).ToList();
                    break;
                case "IndicativePrimaryTechnology":
                    accountData = _accountData.Where(p => p.IndicativePrimaryTechnology.StartsWith(searchKey.ToUpper())).Select(p => p.IndicativePrimaryTechnology.ToUpper()).ToList();
                    break;

                default:
                    accountData = _accountData.Where(p => p.Account.StartsWith(searchKey.ToUpper())).Select(p => p.Account.ToUpper()).ToList();
                    break;
            }
            return Json(accountData, JsonRequestBehavior.AllowGet); 
        }

        [HttpGet]
        public ActionResult AccountDetails(string searchKey)
        {
            var accountDetail = _accountData.Where(p => p.ProjectName == searchKey).FirstOrDefault();
            return PartialView("~/Views/AccountDetail.cshtml", accountDetail);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private List<AccountManagementViewModel> ReadExcelData()
        {
            string path = System.IO.Path.GetFullPath(ConfigurationManager.AppSettings["FilePath"]);
            OleDbConnection oledbConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + path + ";Extended Properties =\"Excel 8.0;HDR=Yes;IMEX=2\"");
            oledbConn.Open();
            OleDbCommand cmd = new OleDbCommand();
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet ds = new DataSet();
            cmd.Connection = oledbConn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM [Project Repository$]";
            oleda = new OleDbDataAdapter(cmd);
            oleda.Fill(ds, "Sheet1");

            DataTable data = ds.Tables["Sheet1"];

            _accountData = (from dt in data.AsEnumerable()
                                          select new AccountManagementViewModel()
                                          {
                                              Account = dt[0].ToString().ToUpper(),
                                              ProjectName = dt[1].ToString().ToUpper(),
                                              BAName = dt[2].ToString(),
                                              Customer = dt[3].ToString(),
                                              ProjectDescription = dt[4].ToString(),
                                              Solution = dt[5].ToString(),
                                              TechStack = dt[6].ToString().ToUpper(),
                                              IndicativePrimaryTechnology = dt[7].ToString().ToUpper(),
                                              PracticeArea = dt[8].ToString(),
                                              MeasureableBusinessValue = dt[9].ToString(),
                                              HowAreWeInvolved = dt[10].ToString(),
                                              PeakTeamSize = dt[11].ToString(),
                                              OnShorePresence = dt[12].ToString()
                                          }).ToList();

            return _accountData;

        }
    }
}