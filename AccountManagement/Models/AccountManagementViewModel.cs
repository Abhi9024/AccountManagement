using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManagement.Models
{
    public class AccountManagementViewModel
    {
        public string Account { get; set; }

        public string ProjectName { get; set; }

        public string BAName { get; set; }

        public string Customer { get; set; }

        public string ProjectDescription { get; set; }

        public string Solution { get; set; }

        public string TechStack { get; set; }

        public string PracticeArea { get; set; }

        public string MeasureableBusinessValue { get; set; }

        public string HowAreWeInvolved { get; set; }

        public string PeakTeamSize { get; set; }

        public string OnShorePresence { get; set; }

        public string IndicativePrimaryTechnology { get; set; }

    }

    public class AccountManagement
    {
        public PagedList.IPagedList<AccountManagementViewModel> AccountManagementModel { get; set; }
        public bool IsAccount { get; set; }
        public bool IsProjectName { get; set; }
        public bool IsBAName { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsTechStack { get; set; }
        public string SearchKey { get; set; }

    }

}