using System;

namespace CopyCompanyInfo.DataAccess
{
    class CompanyModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string RepresentName { get; set; }
        public string RepresentPhone { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ActivitiesDate { get; set; }

        public CompanyModel()
        {
            CompanyName = string.Empty;
            CompanyAddress = string.Empty;
            RepresentName = string.Empty;
            RepresentPhone = string.Empty;
            CityId = 0;
            DistrictId = 0;
            IssuedDate = DateTime.Now;
            ActivitiesDate = DateTime.Now;
        }
    }
}
