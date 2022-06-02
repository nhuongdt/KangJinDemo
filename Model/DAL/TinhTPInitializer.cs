using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL
{
    public class TinhTPInitializer
    {
        static Guid idVNi = QuocGiaInitializer.idQGia_VNi;

        #region lấy dữ liệu mẫu khai báo chi tiết
        #region DM_VungMien
        static Guid idMienBac = Guid.NewGuid();
        static Guid idMienTrung = Guid.NewGuid();
        static Guid idMienNam = Guid.NewGuid();
        public static List<DM_VungMien> lstVungsInitializer = new List<DM_VungMien>()
        {
            new DM_VungMien{ID=idMienBac, MaVung="MienBac",TenVung="Miền Bắc",  GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_VungMien{ID=idMienTrung, MaVung="MienTrung",TenVung="Miền Trung",  GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_VungMien{ID=idMienNam, MaVung="MienNam",TenVung="Miền Nam",  GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"}
        };
        #endregion

        #region DM_TinhThanh
        static Guid idHaNoi = Guid.NewGuid();
        static Guid idTPHCM = Guid.NewGuid();
        public static List<DM_TinhThanh> lstTinhTPsInitializer = new List<DM_TinhThanh>()
        {
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="AGiang",TenTinhThanh="An Giang",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BR-VT",TenTinhThanh="Bà Rịa - Vũng Tàu",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BGiang",TenTinhThanh="Bắc Giang",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BKan",TenTinhThanh="Bắc Kạn",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BLieu",TenTinhThanh="Bạc Liêu",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BNinh",TenTinhThanh="Bắc Ninh",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BTre",TenTinhThanh="Bến Tre",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BDinh",TenTinhThanh="Bình Định",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BDuong",TenTinhThanh="Bình Dương",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BPhuoc",TenTinhThanh="Bình Phước",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="BThuan",TenTinhThanh="Bình Thuận",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="CMau",TenTinhThanh="Cà Mau",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="CBang",TenTinhThanh="Cao Bằng",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="CTho",TenTinhThanh="Cần Thơ",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="DNang",TenTinhThanh="Đà Nẵng",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="DLak",TenTinhThanh="Đắk Lắk",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="DNong",TenTinhThanh="Đắk Nông",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="DBien",TenTinhThanh="Điện Biên",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="DNai",TenTinhThanh="Đồng Nai",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="DThap",TenTinhThanh="Đồng Tháp",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="GLai",TenTinhThanh="Gia Lai",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="HaGiang",TenTinhThanh="Hà Giang",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="HNam",TenTinhThanh="Hà Nam",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=idHaNoi ,MaTinhThanh="HNoi",TenTinhThanh="Hà Nội",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="HTinh",TenTinhThanh="Hà Tĩnh",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="HDuong",TenTinhThanh="Hải Dương",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="HPhong",TenTinhThanh="Hải Phòng",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="HauGiang",TenTinhThanh="Hậu Giang",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="HBinh",TenTinhThanh="Hòa Bình",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="HYen",TenTinhThanh="Hưng Yên",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="KHoa",TenTinhThanh="Khánh Hòa",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="KGiang",TenTinhThanh="Kiên Giang",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="KTum",TenTinhThanh="Kon Tum",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="LChau",TenTinhThanh="Lai Châu",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="LDong",TenTinhThanh="Lâm Đồng",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="LSon",TenTinhThanh="Lạng Sơn",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="LCai",TenTinhThanh="Lào Cai",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="LAn",TenTinhThanh="Long An",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="NDinh",TenTinhThanh="Nam Định",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="NAn",TenTinhThanh="Nghệ An",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="NBinh",TenTinhThanh="Ninh Bình",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="NThuan",TenTinhThanh="Ninh Thuận",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="PTho",TenTinhThanh="Phú Thọ",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="PYen",TenTinhThanh="Phú Yên",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="QBinh",TenTinhThanh="Quảng Bình",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="QNam",TenTinhThanh="Quảng Nam",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="QNgai",TenTinhThanh="Quảng Ngãi",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="QNinh",TenTinhThanh="Quảng Ninh",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="QTri",TenTinhThanh="Quảng Trị",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="STrang",TenTinhThanh="Sóc Trăng",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="SLa",TenTinhThanh="Sơn La",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="TNinh",TenTinhThanh="Tây Ninh",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="TBinh",TenTinhThanh="Thái Bình",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="TNguyen",TenTinhThanh="Thái Nguyên",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="THoa",TenTinhThanh="Thanh Hóa",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="TTHue",TenTinhThanh="Thừa Thiên Huế",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="TGiang",TenTinhThanh="Tiền Giang",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="TVinh",TenTinhThanh="Trà Vinh",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="TQuang",TenTinhThanh="Tuyên Quang",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="VLong",TenTinhThanh="Vĩnh Long",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="VPhuc",TenTinhThanh="Vĩnh Phúc",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=Guid.NewGuid() ,MaTinhThanh="YBai",TenTinhThanh="Yên Bái",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_TinhThanh{ID=idTPHCM ,MaTinhThanh="TPHCM",TenTinhThanh="TP HCM",ID_QuocGia=idVNi,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"}
        };
        #endregion

        #region DM_QuanHuyen
        //
        public static List<DM_QuanHuyen> lstQHuyensInitializer = new List<DM_QuanHuyen>()
        {
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_BDinh",TenQuanHuyen="Ba Đình",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_BTuLiem",TenQuanHuyen="Bắc Từ Liêm",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_CG",TenQuanHuyen="Cầu Giấy",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_DDa",TenQuanHuyen="Đống Đa",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_HDong",TenQuanHuyen="Hà Đông",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_HBT",TenQuanHuyen="Hai Bà Trưng",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_HKiem",TenQuanHuyen="Hoàn Kiếm",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_HMai",TenQuanHuyen="Hoàng Mai",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_LBien",TenQuanHuyen="Long Biên",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_NTuLiem",TenQuanHuyen="Nam Từ Liêm",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_THo",TenQuanHuyen="Tây Hồ",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            new DM_QuanHuyen{ID=Guid.NewGuid() ,MaQuanHuyen="HNoi_TXuan",TenQuanHuyen="Thanh Xuân",ID_TinhThanh=idHaNoi ,NgayTao=DateTime .Now ,NguoiTao="ssoftvn"}
        };
        #endregion
        #endregion

        #region  lấy dữ liệu từ database mẫu - banhang24vn
        public static List<DM_VungMien> GetVungMiensKT()
        {
            List<Model.DM_VungMien> lstVMiens = new List<Model.DM_VungMien>();

            Model_banhang24vn.BanHang24vnContext dbMacDinh = new Model_banhang24vn.BanHang24vnContext();
            var lstVMiens_MD = dbMacDinh.DM_VungMien;
            if (lstVMiens_MD != null && lstVMiens_MD.ToList().Count > 0)
            {
                foreach (Model_banhang24vn.DM_VungMien objItem_Mau in lstVMiens_MD)
                {
                    Model.DM_VungMien objNew = new Model.DM_VungMien();
                    objNew.GhiChu = objItem_Mau.GhiChu;
                    objNew.ID = objItem_Mau.ID;
                    objNew.MaVung = objItem_Mau.MaVung;
                    objNew.TenVung = objItem_Mau.TenVung;

                    objNew.NgayTao = DateTime.Now;
                    objNew.NguoiTao = "ssoftvn";

                    lstVMiens.Add(objNew);
                }
            }
            return lstVMiens;
        }

        public static List<DM_TinhThanh> GetTinhThanhsKT()
        {
            List<Model.DM_TinhThanh> lstTinhTPs = new List<Model.DM_TinhThanh>();

            Model_banhang24vn.BanHang24vnContext dbMacDinh = new Model_banhang24vn.BanHang24vnContext();
            var lstTinhTPs_MD = dbMacDinh.DM_TinhThanh;
            if (lstTinhTPs_MD != null && lstTinhTPs_MD.ToList().Count > 0)
            {
                foreach (Model_banhang24vn.DM_TinhThanh objItem_Mau in lstTinhTPs_MD)
                {
                    Model.DM_TinhThanh objNew = new Model.DM_TinhThanh();
                    objNew.GhiChu = objItem_Mau.GhiChu;
                    objNew.ID = objItem_Mau.ID;
                    objNew.ID_QuocGia = idVNi;
                    objNew.MaTinhThanh = objItem_Mau.MaTinhThanh;
                    objNew.TenTinhThanh = objItem_Mau.TenTinhThanh;

                    objNew.NgayTao = DateTime.Now;
                    objNew.NguoiTao = "ssoftvn";

                    lstTinhTPs.Add(objNew);
                }
            }
            return lstTinhTPs;
        }

        public static List<Model.DM_QuanHuyen> GetQuanHuyensKT()
        {
            List<Model.DM_QuanHuyen> lstQHuyens = new List<Model.DM_QuanHuyen>();

            Model_banhang24vn.BanHang24vnContext dbMacDinh = new Model_banhang24vn.BanHang24vnContext();
            var lstQHuyens_MD = dbMacDinh.DM_QuanHuyen;
            if (lstQHuyens_MD != null && lstQHuyens_MD.ToList().Count > 0)
            {
                foreach (Model_banhang24vn.DM_QuanHuyen objItem_Mau in lstQHuyens_MD)
                {
                    Model.DM_QuanHuyen objNew = new Model.DM_QuanHuyen();
                    objNew.GhiChu = objItem_Mau.GhiChu;
                    objNew.ID = objItem_Mau.ID;
                    objNew.ID_TinhThanh = objItem_Mau.ID_TinhThanh;
                    objNew.MaQuanHuyen = objItem_Mau.MaQuanHuyen;
                    objNew.TenQuanHuyen = objItem_Mau.TenQuanHuyen;

                    objNew.NgayTao = DateTime.Now;
                    objNew.NguoiTao = "ssoftvn";

                    lstQHuyens.Add(objNew);
                }
            }
            return lstQHuyens;
        }
        #endregion 
    }
}
