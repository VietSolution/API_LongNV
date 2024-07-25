
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using VsLogistics.DataModel;

namespace AppCMC.Controllers
{
    public enum EnumTrangThaiDieuPhoiFilterApp
    {
        All = 0,
        DuocGiao =1,
        DaNhan =2,
        HoanThanh =3
    }
    public enum EnumObjectType
    {
        Customer = 1,
        Shipper = 2,
        Consigne = 3,
        Ailine = 4,
        NhaCC = 5,
        Port = 6,
        NhanVien =7
    }
    public class DMDoiTuong
    {
        public string EnumType { get; set; } // 1 : Customer , 2: Shipper, 3:Consigne,4:Airline,5:NhaCC, 6: Cảng , 7:Nhân viên
        public string TaxCode { get; set; }
        public string Code { get; set; }
        public string NameVI { get; set; }
        public string NameEN { get; set; }
        public string AddressVI { get; set; }
        public string AddressEN { get; set; }
        public string SoCMT { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string IATACode { get; set; }
    }
    public class tblSysUserDto
    {
        public bool FlagHoanThanhDoDau { get; set; } = false;
        public bool FlagQuanLy { get; set; } 
        public string Username { get; set; }
        public string Pass { get; set; }
        public string Key { get; set; }
        public string HoTen { get; set; }
        public string DatabaseServerName { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUserName { get; set; }
        public string DatabasePassword { get; set; }
        public long IDUser { get; set; }
    }
    public class tblDieuPhoiVanChuyenDto
    {
        public int STTChuyen { get; set; }
        public string RGB { get; set; }
        public string ProductKey { get; set; }
        public long IDChuyen { get; set; }
        public long? IDLaiXe { get; set; }
        public DateTime? NgayDongHangCal { get; set; }
        public string NgayDongHang { 
            get 
            {
                return NgayDongHangCal?.ToString("HH:mm dd/MM/yyyy");
            } 
            set {  }
        }
        public DateTime? NgayTraHangCal { get; set; }
        public string NgayTraHang
        {
            get
            {
                return NgayTraHangCal?.ToString("HH:mm dd/MM/yyyy");
            }
            set { }
        }
        public string DiemDi { get; set; }
        public string DiemDen { get; set; }
        public string SoPL { get; set; }
        public string SoKhoi { get; set; }
        public string SoGioCho { get; set; }
        public string SoCaLuu { get; set; }
        public string VeBenBai { get; set; }
        public string PhatSinhKhac { get; set; }
        public string GhiChu { get; set; }
        public string MaDieuVan { get; set; }
        public string SoKG { get; set; }
        public string BienSoXe { get; set; }
        public string LaiXe { get; set; }
        public string LoaiXe { get; set; }
        public string DonViVanTai { get; set; }
        public string KhachHang { get; set; }
        public string HangHoa { get; set; }
        public string HangVe { get; set; }
        public int? TrangThaiDieuPhoiIn { get; set; }
        public string TrangThaiDieuPhoiOut => (TrangThaiVanChuyen + "").Length > 0 ? TrangThaiVanChuyen : (this.TrangThaiDieuPhoiIn != null ? ((EnumTrangThaiDieuPhoiVC)TrangThaiDieuPhoiIn).GetDescription() : "");
        public DateTime? ThoiGianVeCal { get; set; }
        public int? TrangThaiVanChuyenIn { get; set; }
        public string TrangThaiVanChuyen { get; set; }
        public string ThoiGianVe
        {
            get
            {
                return ThoiGianVeCal?.ToString("HH:mm dd/MM/yyyy");
            }
            set { }
        }
        public string TenNutHienThi
        {
            get
            {
                if (this.TrangThaiDieuPhoiIn == null) return "Bắt đầu";
                else if (this.TrangThaiDieuPhoiIn == (int)EnumTrangThaiDieuPhoiVC.NhanLenh)
                {
                    if(this.TrangThaiVanChuyenIn == null)
                        return "Đến điểm đóng hàng";
                    else
                    {
                        if(this.TrangThaiVanChuyenIn == (int)EnumTrangThaiVanChuyen.DenDiemDong) return "Đến điểm trả hàng";
                        else if (this.TrangThaiVanChuyenIn == (int)EnumTrangThaiVanChuyen.DenDiemTra) return "Vỏ về";
                        else if (this.TrangThaiVanChuyenIn == (int)EnumTrangThaiVanChuyen.VoVe) return "Hoàn thành";
                    } 
                } 
                return "Bắt đầu";
            }
            set { }
        }

    }
    public class tblDieuPhoiVanChuyenNewDto
    {
        public string ProductKey { get; set; }
        public long IDChuyen { get; set; }
        public long? IDUser { get; set; }
        public DateTime? NgayDongHang { get; set; }
        public DateTime? NgayTraHang { get; set; }
        public long? IDDiemDi { get; set; }
        public long? IDDiemDen { get; set; }
        public long? IDLoaiXe { get; set; }
        public long? IDXeOto { get; set; }
        public long? IDLaiXe { get; set; }
        public double SoPL { get; set; }
        public double SoKG { get; set; }
        public double SoKhoi { get; set; }
        public long? IDKhachHang { get; set; }
        public long? IDHangHoa { get; set; }
        public bool FlagHangVe { get; set; }
        public DateTime? ThoiGianVe { get; set; }
    }

    public class tblObjectAll
    {
        public string BienSoXe { get; set; }
        public long ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class tblDMXeDto
    {
        public string ProductKey { get; set; }
        public long IDXe { get; set; }
        public string BienSoXe { get; set; }
        public int SoLuongChuyen { get; set; }
        public string TrangThai { get; set; }
        public string RGB { get; set; }
    }
    public class DieuPhoiXeDto
    {
        public string ProductKey { get; set; }
        public long IDUser { get; set; }
        public long IDChuyen { get; set; }
        public long? IDXeOto { get; set; }
        public string BienSoXe { get; set; }
        public long? IDLaiXe { get; set; }
        public int EnumThueXeOrXeMinh { get; set; }
        public long? IDDonViVanTai { get; set; }
        public string LaiXe { get; set; }
        public string DTLaiXe { get; set; }
        public string SoGioCho { get; set; }
        public string SoCaLuu { get; set; }
        public string VeBenBai { get; set; }
        public string PhatSinhKhac { get; set; }
        public string GhiChu { get; set; }
        public int? TrangThai { get; set; }

        public long? IDTrangThaiVanChuyen { get; set; }
    }
    public class ObjectCal
    {
        public long? ID { get; set; }
        public long? IDChuyen { get; set; }
        public long? IDTrangThaiVanChuyen { get; set; }
        public string ProductKey { get; set; }
        public long? IDUser { get; set; }
        public long? IDXeOto { get; set; }
        public string BienSoXe { get; set; }
        public string NoiDungSuaChua { get; set; }
        public int? SoLuong { get; set; }
        public long? DonGia { get; set; }
        public long? ThanhTien { get; set; }
        public string LaiXe { get; set; }
        public long? IDLaiXe { get; set; }
        public DateTime? NgaySuaCal { get; set; }
        public string NgaySua
        {
            get
            {
                return NgaySuaCal?.ToString("dd/MM/yyyy");
            }
            set { }
        }
        public DateTime? NgayHoanThanhCal { get; set; }
        public string NgayHoanThanh
        {
            get
            {
                return NgayHoanThanhCal?.ToString("dd/MM/yyyy");
            }
            set { }
        }
        public DateTime? NgayGioThucHien { get; set; }
        public DateTime? NgayDoDauCal { get; set; }
        public string NgayDoDau
        {
            get
            {
                return NgayDoDauCal?.ToString("dd/MM/yyyy");
            }
            set
            {
                
            }
        }

        public string GARAGE { get; set; }
        public string GhiChu { get; set; }
        public string SoVo { get; set; }

    }
    public class PublicCodeController : ApiController
    {
        enum ErrorCode
        {
            Insert_Successfully = 201,
            Update_Successfully = 204,
            Code_Exits = 409,
            MST_Exists= 400,
            Not_Found = 404,
            Bad_Request = 504,
            Object_Null = -100,
            Save_Error = -200,
            Type_Bad = -300,
            Code_Empty = -400,

        }

        private LGTICDBEntities db = new LGTICDBEntities(ConnectionTools.BuildConnectionString());

        private void UpdateDataCustomer(tblDieuPhoiVanChuyen _ob, DMDoiTuong _object, bool bNew)
        {
           // _ob.Code = _object.Code;
           // _ob.NameVI = _object.NameVI;
           // _ob.NameEN = _object.NameEN;
           // _ob.AddressEN = _object.AddressEN;
           // _ob.AddressVI = _object.AddressVI;
           // _ob.TaxCode = _object.TaxCode;
           // _ob.Phone = _object.SoDienThoai;
           // _ob.Email = _object.Email;
           // _ob.IATACode = _object.IATACode;
           //if(bNew == true)
           // {
           //     _ob.FlagCRM = true;
           //     _ob.FlagActive = true;
           //     _ob.DateCreate = DateTime.Now;
           // }    

        }
       
        //===================================================================API=====================================================
       
        //public IHttpActionResult GetListDieuPhoiVanChuyen()
        //{
        //    int a= db.tblDieuPhoiVanChuyens.Where(x=>x.NgayDongHang != null ).Count();
        //    if(a > 0) return Content(HttpStatusCode.OK, "có dl !");
        //    else return Content(HttpStatusCode.OK, "Cập nhật dữ liệu thành công !");

        //}

        // PUT: api/tblDMCustomers/5


        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("api/EditCustomer")]
        public IHttpActionResult EditCustomer([FromBody] DMDoiTuong _object)
        {
            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.PreconditionFailed, "Lỗi kiểu dữ liệu đầu vào");
            }
            if (_object == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống ĐỐI TƯỢNG");


            string _code = _object.Code?.Trim();
            if ((_code + "").Length == 0) return Content(HttpStatusCode.LengthRequired, "Không được để trống MÃ ĐỐI TƯỢNG");


            if ((_object.EnumType + "").Length == 0) return Content(HttpStatusCode.LengthRequired, "Không được để trống LOẠI ĐỐI TƯỢNG");

            try
            {
                string[] _type = null;


                if (_object.EnumType.Contains(",")) _type = _object.EnumType.Split(',');
                else if (_object.EnumType.Contains(";")) _type = _object.EnumType.Split(';');
                else
                {
                    int _t = int.Parse(_object.EnumType);
                    if (_t != 1 && _t != 2 && _t != 3 && _t != 4 && _t != 5 && _t != 6 && _t != 7)
                    {
                        return Content(HttpStatusCode.BadRequest, "LOẠI ĐỐI TƯỢNG không hợp lệ !");
                    }
                    else
                    {
                        _type = new string[] { _t.ToString() };
                    }
                }

                if (_type == null || _type?.Count() == 0) return Content(HttpStatusCode.BadRequest, "LOẠI ĐỐI TƯỢNG không hợp lệ !");

                if (_type.Where(x => int.Parse(x) == 7).Count() > 0)  // nhân viên
                {
                    tblNhanSu _ob = null;
                    if (_code?.Trim()?.Length > 0)
                    {
                        //_ob = db.tblNhanSus.Where(x => x.MANHANSU == _code).FirstOrDefault();
                        if (_ob == null) return Content(HttpStatusCode.NotFound, "MÃ ĐỐI TƯỢNG không tồn tại");
                    }
                   // UpdateDataNhanSu(_ob, _object);
                }
               
                else
                {
                    return Content(HttpStatusCode.BadRequest, "LOẠI ĐỐI TƯỢNG không hợp lệ !");
                }
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "LOẠI ĐỐI TƯỢNG không hợp lệ !");
            }
            
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Content(HttpStatusCode.InternalServerError, "Lưu dữ liệu bị lỗi");
            }

            return Content(HttpStatusCode.OK, "Cập nhật dữ liệu thành công !");
        }
        //=============================================================================================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}