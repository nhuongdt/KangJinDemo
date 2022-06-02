using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
    public class NganhNgheKinhDoanhService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<NganhNgheKinhDoanh> _NganhNgheKinhDoanh;
        public NganhNgheKinhDoanhService()
        {
            _NganhNgheKinhDoanh = unitOfWork.GetRepository<NganhNgheKinhDoanh>();
        }
        public IQueryable<NganhNgheKinhDoanh> Query { get { return _NganhNgheKinhDoanh.All(); } }

        public IQueryable<BusinessView> GetAllSelected()
        {
            return unitOfWork.GetRepository<NganhNgheKinhDoanh>().All().Select(o => new BusinessView
            {
                ID = o.ID,
                CreatDate = o.CreatDate,
                CreatedBy = o.CreatedBy,
                MaNganhNghe = o.MaNganhNghe,
                ModifiedBy = o.ModifiedBy,
                ModifiedDate = o.ModifiedDate,
                Status = o.Status,
                TenNganhNghe = o.TenNganhNghe,
                Image=o.Image,
                ImageMobile=o.ImageMobile
            }).OrderByDescending(o => o.CreatDate);
        }



        public NganhNgheKinhDoanh GetByMa(string MaNganhNghe)
        {
            if (!string.IsNullOrWhiteSpace(MaNganhNghe))
            {
                return _NganhNgheKinhDoanh.Filter(o => o.MaNganhNghe.Equals(MaNganhNghe.Trim())).FirstOrDefault();
            }
            return null;
        }
        public IQueryable<BusinessView> SearchSelected(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return GetAllSelected();
            }
            text = StaticVariable.RemoveSign4VietnameseString(text);
            return GetAllSelected().AsEnumerable().Where(o => StringExtensions.ConvertToUnSign(o.TenNganhNghe).Contains(text) || o.MaNganhNghe.Contains(text)).AsQueryable();
        }

        public IQueryable<HT_Quyen> GetRole()
        {
            return unitOfWork.GetRepository<HT_Quyen>().All();
        }

        public IQueryable<HT_Quyen_NganhNgheKinhDoanh> GetDetailRole(Guid? Id)
        {
            return unitOfWork.GetRepository<HT_Quyen_NganhNgheKinhDoanh>().All().Where(o => o.ID_NganhKinhDoanh == Id);
        }

        public IEnumerable<HT_Quyen> getDetailRole(Guid? Id)
        {
            var data = unitOfWork.GetRepository<HT_Quyen_NganhNgheKinhDoanh>().All().Where(o => o.ID_NganhKinhDoanh == Id).Select(o => o.MaQuyen);
            return unitOfWork.GetRepository<HT_Quyen>().All().AsEnumerable().Where(o => data.Contains(o.MaQuyen)
                                    && !string.IsNullOrWhiteSpace(o.QuyenCha) && !checkRole(o.MaQuyen)).AsEnumerable();
        }
        public JsonViewModel<string> Update(BusinessRole model)
        {

            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            var data = unitOfWork.GetRepository<NganhNgheKinhDoanh>().GetById(model.ID);
            if (data == null)
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Bản ghi đã bị xóa, vui lòng tải lại trang.";
            }
            else
            {
                if (!model.TenNganhNghe.Equals(data.TenNganhNghe))
                {
                    char[] whitespace = new char[] { ' ', '\t' };
                    var maNnkd = string.Join("", model.TenNganhNghe.Split(whitespace).Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Substring(0, 1)));
                    maNnkd = StaticVariable.RemoveSign4VietnameseString(maNnkd.ToUpper());
                    if (_NganhNgheKinhDoanh.Filter(o => o.MaNganhNghe.Equals(maNnkd) && o.ID!=data.ID).Any())
                    {
                        var filter = maNnkd + "_";
                        if (_NganhNgheKinhDoanh.Filter(o => o.MaNganhNghe.Contains(filter)).Count() == 0)
                        {
                            data.MaNganhNghe = maNnkd + "_1";
                        }
                        else
                        {
                            var maCount = _NganhNgheKinhDoanh.Filter(o => o.MaNganhNghe.Contains(filter))
                                .Select(o => o.MaNganhNghe).ToList()
                                .Select(o => int.Parse(o.Split('_')[1])).Max() + 1;
                            data.MaNganhNghe = maNnkd + "_" + maCount;
                        }
                    }
                    else
                    {
                        data.MaNganhNghe = maNnkd;
                    }
                }
                data.TenNganhNghe = model.TenNganhNghe;
                data.ModifiedBy = model.UserCurent;
                data.ModifiedDate = DateTime.Now;
                if(!string.IsNullOrWhiteSpace(model.Image))
                {
                    data.Image = model.Image;
                }
                if (!string.IsNullOrWhiteSpace(model.ImageMobile))
                {
                    data.ImageMobile = model.ImageMobile;
                }
                data.Status = model.Status;
                var _HT_Quyen_NganhNgheKinhDoanh = unitOfWork.GetRepository<HT_Quyen_NganhNgheKinhDoanh>();
                List<string> quyenold = _HT_Quyen_NganhNgheKinhDoanh.Get(p => p.ID_NganhKinhDoanh == model.ID).Select(p => p.MaQuyen).ToList();
                var listParent = new List<string>();
                List<string> quyennew = new List<string>();
                if (model.checkList != null)
                {
                    _HT_Quyen_NganhNgheKinhDoanh.Delete(o => o.ID_NganhKinhDoanh == model.ID);
                    foreach (var item in model.checkList)
                    {
                        GetRoleParent(item, model.checkList, ref listParent);
                        _HT_Quyen_NganhNgheKinhDoanh.Create(new HT_Quyen_NganhNgheKinhDoanh { ID = Guid.NewGuid(), ID_NganhKinhDoanh = model.ID, MaQuyen = item });
                    }
                    foreach (var ite in listParent)
                    {
                        _HT_Quyen_NganhNgheKinhDoanh.Create(new HT_Quyen_NganhNgheKinhDoanh { ID = Guid.NewGuid(), ID_NganhKinhDoanh = model.ID, MaQuyen = ite });
                    }
                    quyennew.AddRange(listParent.OrderBy(p=>p));
                    quyennew.AddRange(model.checkList);
                }
                else
                {
                    _HT_Quyen_NganhNgheKinhDoanh.Delete(o => o.ID_NganhKinhDoanh == model.ID);
                }
                List<string> quyenRemove = quyenold.Except(quyennew).ToList();
                List<string> quyenAdd = quyennew.Except(quyenold).ToList();
                List<string> lstData = GetDataBaseList();
                var _CuaHangDangky = unitOfWork.GetRepository<CuaHangDangKy>();
                List<string> lstCuahang = _CuaHangDangky.Get(p => p.ID_NganhKinhDoanh == model.ID).Select(p => "SSOFT_" + p.SubDomain.ToUpper()).ToList();
                List<string> connectionData = lstData.Where(p => lstCuahang.Contains(p)).ToList();
                foreach (var item in connectionData)
                {
                    UpdateQuyenToDatabase(item, quyenAdd, quyenRemove);
                }

                unitOfWork.Save();
            }
            return result;

        }

        public void UpdateQuyenToDatabase(string dataname, List<string> quyenAdd, List<string> quyenRemove)
        {
            var _ht_quyen = unitOfWork.GetRepository<HT_Quyen>();
            string conString = "server=data2.ssoft.vn;uid=sa;pwd=123asd!@#123;database=" + dataname;
            string insertcmd = "INSERT INTO HT_Quyen (MaQuyen, TenQuyen, MoTa, QuyenCha, DuocSuDung) VALUES ";
            if(quyenAdd.Count > 0)
            {
                List<HT_Quyen> _lstHTQuyenAdd = _ht_quyen.Get(p => quyenAdd.Contains(p.MaQuyen)).ToList();
                string values = "";
                foreach(var item in _lstHTQuyenAdd)
                {
                    values = values + insertcmd +  "('" + item.MaQuyen + "', N'" + item.TenQuyen + "', N'" + item.MoTa + "', '" + item.QuyenCha + "', 'true');";
                }
                //values = values.Remove(values.Length - 1);
                insertcmd = values;
            }

            string deleteQuyenNhomcmd = "DELETE FROM HT_Quyen_Nhom WHERE MaQuyen in (";
            string deleteQuyencmd = "DELETE FROM HT_Quyen WHERE MaQuyen in (";                 
            if (quyenRemove.Count > 0)
            {
                string invalue = "";
                foreach (var item in quyenRemove)
                {
                    invalue = invalue + "'" + item + "',";
                }
                invalue = invalue.Remove(invalue.Length - 1);
                deleteQuyenNhomcmd = deleteQuyenNhomcmd + invalue + ");";
                deleteQuyencmd = deleteQuyencmd + invalue + ");";
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    if (quyenAdd.Count > 0)
                    {
                        SqlCommand cmdAdd = new SqlCommand(insertcmd, con);
                        cmdAdd.ExecuteNonQuery();
                    }
                    if (quyenRemove.Count > 0)
                    {
                        SqlCommand cmdRemoveQuyenNhom = new SqlCommand(deleteQuyenNhomcmd, con);
                        cmdRemoveQuyenNhom.ExecuteNonQuery();
                        SqlCommand cmdRemoveQuyen = new SqlCommand(deleteQuyencmd, con);
                        cmdRemoveQuyen.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception)
            {
                
            }
        }

        public List<string> GetDataBaseList()
        {
            List<string> list = new List<string>();
            string conString = "server=data2.ssoft.vn;uid=sa;pwd=123asd!@#123;database=master";
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT name FROM sys.databases", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
                con.Close();
            }
            return list;
        }
        public JsonViewModel<string> Insert(NganhNgheKinhDoanh model, List<string> listRole)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            char[] whitespace = new char[] { ' ', '\t' };
            var maNnkd = string.Join("", model.TenNganhNghe.Split(whitespace).Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Substring(0, 1)));
            model.MaNganhNghe = StaticVariable.RemoveSign4VietnameseString(maNnkd.ToUpper());
            if (_NganhNgheKinhDoanh.Filter(o => o.MaNganhNghe.Equals(model.MaNganhNghe)).Any())
            {
                var filter = model.MaNganhNghe + "_";
                if (_NganhNgheKinhDoanh.Filter(o => o.MaNganhNghe.Contains(filter)).Count() == 0)
                {
                    model.MaNganhNghe = model.MaNganhNghe + "_1";
                }
                else
                {
                    var maCount = _NganhNgheKinhDoanh.Filter(o => o.MaNganhNghe.Contains(filter))
                        .Select(o => o.MaNganhNghe).ToList()
                        .Select(o => int.Parse(o.Split('_')[1])).Max() + 1;
                    model.MaNganhNghe = model.MaNganhNghe + "_" + maCount;
                }
            }
            _NganhNgheKinhDoanh.Create(model);
            var _HT_Quyen_NganhNgheKinhDoanh = unitOfWork.GetRepository<HT_Quyen_NganhNgheKinhDoanh>();
            var listParent = new List<string>();
            if (listRole != null)
            {
                foreach (var item in listRole)
                {
                    GetRoleParent(item, listRole, ref listParent);

                    _HT_Quyen_NganhNgheKinhDoanh.Create(new HT_Quyen_NganhNgheKinhDoanh { ID = Guid.NewGuid(), ID_NganhKinhDoanh = model.ID, MaQuyen = item });
                }
            }
            foreach(var ite in listParent)
            {
                _HT_Quyen_NganhNgheKinhDoanh.Create(new HT_Quyen_NganhNgheKinhDoanh { ID = Guid.NewGuid(), ID_NganhKinhDoanh = model.ID, MaQuyen = ite });
            }
            unitOfWork.Save();
            return result;

        }
        private void GetRoleParent(string role, List<string> listInput, ref List<string> listOutPut)
        {
            var data = unitOfWork.GetRepository<HT_Quyen>().All().FirstOrDefault(o => o.MaQuyen.Equals(role));
                if (data != null 
                && !string.IsNullOrWhiteSpace(data.QuyenCha) 
                && !listOutPut.Any(o=>o.Equals(data.QuyenCha)) 
                && !listInput.Any(o=>o.Equals(data.QuyenCha)))
                {
                    listOutPut.Add(data.QuyenCha);
                    GetRoleParent(data.QuyenCha, listInput, ref listOutPut);
                }

        }
        private bool checkRole(string role)
        {
            return unitOfWork.GetRepository<HT_Quyen>().Filter(o => o.QuyenCha.Equals(role)).Any();
        }

        public JsonViewModel<string> Delete(Guid id)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            var model = _NganhNgheKinhDoanh.Find(id);
            if (model == null)
            {
                result.Data = "ngành nghề không tồn tại hoặc đã bị xóa, vui lòng kiểm tra lại.";
                result.ErrorCode = (int)Notification.ErrorCode.error;
            }
            else
            {
                unitOfWork.GetRepository<HT_Quyen_NganhNgheKinhDoanh>().Delete(o => o.ID_NganhKinhDoanh == model.ID);
                var _TinhNangNghanhNghe= unitOfWork.GetRepository<TinhNangNghanhNghe>();
               var listTinhNang= _TinhNangNghanhNghe.Filter(o => o.Id_NganhNghe == model.ID).Select(o=>o.Id).ToList();
                
                unitOfWork.GetRepository<AnhTinhNangNghanhNghe>().Delete(o => listTinhNang.Contains(o.Id_NganhNgheTinhNang??-1));
                _TinhNangNghanhNghe.Delete(o => o.Id_NganhNghe == model.ID);
                _NganhNgheKinhDoanh.Delete(model);
                unitOfWork.Save();
            }
            return result;
        }
    }
}
