using CopyCompanyInfo.Common;
using System.Collections.Generic;
using System.Data;

namespace CopyCompanyInfo.DataAccess
{
    class AreaModel
    {
        public int AreaId { set; get; }
        public string AreaName { set; get; }
        public string AreaUrl { set; get; }
        public int ParentId { set; get; }

        public List<AreaModel> ListAreaModel { set; get; }

        public AreaModel()
        {
            AreaName = string.Empty;
            AreaUrl = string.Empty;
            ParentId = 0;
            ListAreaModel = new List<AreaModel>();
        }

        public DataSet GetListArea(int parentId = 0)
        {
            var query = string.Format("SELECT AreaId, AreaName, AreaUrl, ParentId FROM tblArea WHERE ParentId = {0}", parentId);
            var ds = DbHelper.ExecuteQuery(query);              
            return ds;
        }

        public List<AreaModel> GetDistrictByCityId(int cityId)
        {
            return null;
        }

        public void InsertAreaData(AreaModel model)
        {
            string query = "INSERT INTO tblArea (AreaName, AreaUrl, ParentId) VALUES ('{0}', '{1}', '{2}')";
            query = string.Format(query, model.AreaName, model.AreaUrl, model.ParentId);            
            DbHelper.ExecuteNoneQuery(query);
        }

        public override string ToString()
        {
            return string.Format("\nArea Id:{0} \nArea Name:{1}\nArea Url:{2} \nParent Id:{3}\n",
                AreaId, AreaName, AreaUrl, ParentId);
        }
    }
}
