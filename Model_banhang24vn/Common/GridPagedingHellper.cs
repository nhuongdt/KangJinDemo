using Model_banhang24vn.CustomView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.Common
{
   public static class GridPagedingHellper
    {
        public static string PageItems(int page,int pageCount,int rowCount)
        {
            return string.Format("Trang {0} trên {1} ({2} bản ghi)", page, pageCount, rowCount);
        }
        public  enum GridSort
        {
            SortUp = 0,
            SortDow,

        }
        public enum columtableStoreRegistration
        {
            mobile = 0,
            name,
            createDate,
            bussines,
            expiryDate,
            status,
        }
        public enum columtablePot
        {
            title=1,
            creatdate,
            creatby,
            Category,
            view
        }
        public enum columContact
        {
            Name = 0,
            Phone,
            Email,
            Adress,
            Note,
            CreateDate
        }
        public enum columContract
        {
            Name = 0,
            Phone,
            CreateDate,
            ModifiedDate,
            ModifiedBy,
            Status,
            Subdomain,
            IT_Name,
            IT_Phone
        }
        public enum columUserGroup
        {
            groupName = 0,
            detail,
            creatDate,
            creatBy,
            modifyDate,
            modifyBy,
            status
        }
        public enum columPostGroup
        {
            groupName = 0,
            groupParent,
            detail,
            creatDate,
            creatBy,
            status
        }
        public enum columCustomer
        {
            Name = 0,
            City,
            TypeBussines,
            Phone,
            Email,
            CreateDate,
            Prioritize,
            status
        }
        public enum columBusiness
        {
            ma = 0,
            ten,
            creatdate,
            creatby,
            modifieddate,
            modifiedby,
            status
        }
        public enum columUser
        {
            Name = 0,
            userName,
            BirthDay,
            Adress,
            Email,
            Phone,
            CreateDate,
            CreateBy,
            status
        }
    }
}
