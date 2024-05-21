using PublicCodeLongNV.ExportExcel;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using VsLogistics.DataModel;
using VsLogistics.DataModel.Common;

namespace AppCMC.Controllers
{
    public class TranportController : ApiController
    {
        private LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString());
        #region Hàm dùng chung
        public void UpdateChuyen(tblDieuPhoiVanChuyen _Chuyen, tblDieuPhoiVanChuyenNewDto _object)
        {
            _Chuyen.NgayDongHang = _object.NgayDongHang;
            _Chuyen.NgayTraHang = _object.NgayTraHang;
            _Chuyen.IDDMCustomer = _object.IDKhachHang;
            _Chuyen.IDDiemDi = _object.IDDiemDi;
            _Chuyen.IDDiemDen = _object.IDDiemDen;
            _Chuyen.IDDMHangHoa = _object.IDHangHoa;
            _Chuyen.SoKG = _object.SoKG;
            _Chuyen.SoKhoi = _object.SoKhoi;
            _Chuyen.SoPL = _object.SoPL;
            _Chuyen.FlagHangVe = _object.FlagHangVe ;
            _Chuyen.ThoiGianVe = _object.ThoiGianVe;
            _Chuyen.IDDMLoaiXe = _object.IDLoaiXe;
        }
        #endregion
        #region Danh mục
        [HttpGet]
        [Route("api/GettblDMXeOto")]
        public IHttpActionResult GettblDMXeOto(string ProductKey) 
        {
            var LstAllXe = context.tblDMXeOtoes.Select(x=>new { ID = x.ID, BienSoXe = x.BienSoXE, LaiXe = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "" }).ToList();
            return Ok(LstAllXe);
        }

        [HttpGet]
        [Route("api/GettblDMLoaiXe")]
        public IHttpActionResult GettblDMLoaiXe(string ProductKey)
        {
            var LstAllXe = context.tblDMLoaiXes.Select(x => new { ID = x.ID,Code = x.Code, Name = x.NameVI, TrongTai = x.TrongTai }).ToList();
            return Ok(LstAllXe);
        }

        [HttpGet]
        [Route("api/GettblNhanSu")]
        public IHttpActionResult GettblNhanSu(string ProductKey)
        {
            var LstAll = context.tblNhanSus.Select(x => new { ID = x.ID, HoTen = x.HoTenVI, MaNhanSu = x.MANHANSU}).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMDoor")]
        public IHttpActionResult GettblDMDoor(string ProductKey)
        {
            var LstAll = context.tblDMDoors.Select(x => new { ID = x.ID, Name = x.NameVI, Address = x.AddressVI }).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMHangHoa")]
        public IHttpActionResult GettblDMHangHoa(string ProductKey)
        {
            var LstAll = context.tblDMHangHoas.Select(x => new { ID = x.ID , Code = x.Code, Name = x.NameVI }).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMCustomer")]
        public IHttpActionResult GettblDMCustomer(string ProductKey)
        {
            var LstAll = context.tblDMCustomers.Where(x=>x.FlagCustomer == true || (x.FlagCustomer != true && x.FlagLocalTrans != true || x.FlagNhaCC != true)).Select(x => new { ID = x.ID, Code = x.Code, Name = x.NameVI }).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMDonViVanTai")]
        public IHttpActionResult GettblDMDonViVanTai(string ProductKey)
        {
            var LstAll = context.tblDMCustomers.Where(x => x.FlagLocalTrans == true || (x.FlagCustomer != true && x.FlagLocalTrans != true || x.FlagNhaCC != true)).Select(x => new { ID = x.ID, Code = x.Code, Name = x.NameVI }).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMTrangThaiVanChuyen")]
        public IHttpActionResult GettblDMTrangThaiVanChuyen(string ProductKey)
        {
            var LstAll = context.tblDMTrangThaiVanChuyens.Select(x => new { ID = x.ID, RGB = x.RGB, Name = x.NameVI }).ToList();
            return Ok(LstAll);
        }

        #endregion
        #region Api Admin
        [HttpGet]
        [Route("api/GetTrangThaiXeTrongNgay")]
        public IHttpActionResult GetTrangThaiXeTrongNgay(string ProductKey,int Page,int Limit) // danh sách xe trên màn hình chính Admin
        {
            List<tblDMXeDto> LstAllXeSelect = new List<tblDMXeDto>();
            int _skip =(Page -1)* Limit;
            var LstAllXe = context.tblDMXeOtoes.ToList();
            int _count = LstAllXe.Count();
            foreach(var _xe in LstAllXe)
            {
                tblDMXeDto _New = new tblDMXeDto()
                {
                    IDXe = _xe.ID,
                    BienSoXe = _xe.BienSoXE,
                    SoLuongChuyen = context.tblDieuPhoiVanChuyens.Where(x => x.NgayDongHang != null && x.NgayDongHang.Value.Year == DateTime.Now.Year && x.NgayDongHang.Value.Month == DateTime.Now.Month && x.IDDMXeOto == _xe.ID).Count(),
                };
                var _ChuyenHT = context.tblDieuPhoiVanChuyens.Where(x => x.NgayDongHang != null && x.NgayDongHang <= DateTime.Now && x.NgayTraHang >= DateTime.Now && x.IDDMXeOto == _xe.ID).FirstOrDefault();
                if(_ChuyenHT != null)
                {
                    _New.TrangThai = _ChuyenHT.ListTrangThaiVanChuyen.OrderByDescending(x => x.NgayGioThucHien).FirstOrDefault()?.tblDMTrangThaiVanChuyen?.TenText;
                    _New.RGB = _ChuyenHT.ListTrangThaiVanChuyen.OrderByDescending(x => x.NgayGioThucHien).FirstOrDefault()?.tblDMTrangThaiVanChuyen?.RGB;
                }
                LstAllXeSelect.Add(_New);
            }
            var res = new
            {
                data = LstAllXeSelect.OrderByDescending(x => x.SoLuongChuyen).Skip(_skip).Take(Limit).ToList(),
                TotalCount = _count,
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GetListChuyenXe")]
        public IHttpActionResult GetListChuyenXe(string ProductKey,long IDXe , DateTime dtNow, int Page, int Limit) // Lấy ds chuyến của 1 xe
        {
            List<tblDieuPhoiVanChuyenDto> LstChuyenDto = new List<tblDieuPhoiVanChuyenDto>();
            
            Expression<Func<tblDieuPhoiVanChuyen, bool>> func = null;

            func = x =>x.IDDMXeOto == IDXe && x.NgayDongHang.Value.Year == dtNow.Year && x.NgayDongHang.Value.Month == dtNow.Month && x.NgayDongHang.Value.Day == dtNow.Day;
            int _total = context.tblDieuPhoiVanChuyens.Where(func).Count();
           
            if(_total > 0)
            {
               LstChuyenDto = context.tblDieuPhoiVanChuyens.Where(func).Skip((Page - 1) * Limit).Take(Limit).Select(x =>
              new tblDieuPhoiVanChuyenDto
              {
                  IDChuyen = x.ID,
                  BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
                  DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
                  DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
                  NgayDongHangCal = x.NgayDongHang,
                  NgayTraHangCal = x.NgayTraHang,
              }).ToList();
                var res = new
                {
                    TotalCount = _total,
                    Page = Page,
                    Limit = Limit,
                    ProductKey = ProductKey,
                    data = LstChuyenDto,
                };
                return Ok(res);
            }    
            else
            {
                return Content(HttpStatusCode.NotFound, $"Xe không có chuyến trong ngày {dtNow.ToString("dd/MM/yyyy")} !");
            }
        }

        #region danh sách chuyến vận chuyển
        [HttpGet]
        [Route("api/GetListChuyenVanChuyen")] // lọc ds chuyến theo ngày
        public IHttpActionResult GetListChuyenVanChuyen(string ProductKey,long IDUSer, DateTime dtS, DateTime dtE, int Page, int Limit)
        {

            List<tblDieuPhoiVanChuyenDto> LstChuyenDto = new List<tblDieuPhoiVanChuyenDto>();
            var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUSer);
            if (_user == null) return Content(HttpStatusCode.NotFound, "Lỗi dữ liệu !");
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
            Expression<Func<tblDieuPhoiVanChuyen, bool>> func = null;

            func = x => x.FlagDaDieuPhoi != true && (x.NgayDongHang == null || (x.NgayDongHang >= dtS && x.NgayDongHang <= dtE));
            int _total = context.tblDieuPhoiVanChuyens.Where(func).Count();
            if(_total > 0)
            {
                LstChuyenDto = context.tblDieuPhoiVanChuyens.Where(func).OrderByDescending(x => x.NgayDongHang).Skip((Page - 1) * Limit).Take(Limit).Select(x => new tblDieuPhoiVanChuyenDto
                {
                    IDChuyen = x.ID,
                    BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
                    DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
                    DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
                    DonViVanTai = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? "CMC" : (x.tblDMCustomer1 != null ? x.tblDMCustomer1.NameVI : ""),
                    HangHoa = x.tblDMHangHoa != null ? x.tblDMHangHoa.NameVI : "",
                    HangVe = x.FlagHangVe == true ? "1" : "0",
                    KhachHang = x.tblDMCustomer != null ? x.tblDMCustomer.NameVI : "",
                    LaiXe = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? (x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "") : x.LaiXe,
                    NgayDongHangCal = x.NgayDongHang,
                    NgayTraHangCal = x.NgayDongHang,
                    SoKhoi = x.SoKhoi != null ? x.SoKhoi.Value.ToString() : "",
                    SoPL = x.SoPL != null ? x.SoPL.Value.ToString() : "",
                    SoKG = x.SoKG != null ? x.SoKG.Value.ToString() : "",
                    LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "",
                    ThoiGianVeCal = x.ThoiGianVe,
                }).ToList();
            }
            var res = new
            {
                TotalCount = _total,
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey,
                data = LstChuyenDto,
            };

            return Ok(res);
        }

        private tblDieuPhoiVanChuyenDto NewSelectDieuPhoi(tblDieuPhoiVanChuyen x)
        {
            return new tblDieuPhoiVanChuyenDto
            {
                IDChuyen = x.ID,
                BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
                DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
                DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
                DonViVanTai = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? "CMC" : (x.tblDMCustomer1 != null ? x.tblDMCustomer1.NameVI : ""),
                HangHoa = x.tblDMHangHoa != null ? x.tblDMHangHoa.NameVI : "",
                HangVe = x.FlagHangVe == true ? "1" : "0",
                KhachHang = x.tblDMCustomer != null ? x.tblDMCustomer.NameVI : "",
                LaiXe = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? (x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "") : x.LaiXe,
                NgayDongHangCal = x.NgayDongHang,
                NgayTraHangCal = x.NgayDongHang,
                SoKhoi = x.SoKhoi != null ? x.SoKhoi.Value.ToString() : "",
                SoPL = x.SoPL != null ? x.SoPL.Value.ToString() : "",
                SoKG = x.SoKG != null ? x.SoKG.Value.ToString() : "",
                LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "",
                ThoiGianVeCal = x.ThoiGianVe,
                TrangThaiDieuPhoiIn = x.EnumTrangThaiDieuPhoi,
                TrangThaiVanChuyen = x.tblDMTrangThaiVanChuyen != null ? x.tblDMTrangThaiVanChuyen.NameVI : "",
                SoGioCho = x.SoGioCho != null ? x.SoGioCho.Value.ToString() : "",
                SoCaLuu = x.SoCaLuu != null ? x.SoCaLuu.Value.ToString() : "",
                VeBenBai = x.VeBenBai != null ? x.VeBenBai.Value.ToString() : "",
                PhatSinhKhac = x.PhatSinhKhac,
                GhiChu = x.GhiChu,
                MaDieuVan = x.CodeDieuVan,
            };
        }

        [HttpPost]
        [Route("api/PostChuyenVanChuyen")] // mới
        public IHttpActionResult PostChuyenVanChuyen([FromBody] tblDieuPhoiVanChuyenNewDto _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");
           
            if (_object.IDUser == null) return Content(HttpStatusCode.LengthRequired, "Người dùng không tồn tại");
            if (_object.NgayDongHang == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống Ngày đóng hàng");
            if (_object.IDKhachHang == null || _object.IDDiemDi == null || _object.IDDiemDen == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống khách hàng/Điểm đi/điểm đến");
            try
            {
                tblDieuPhoiVanChuyen _chuyen = null;
                _chuyen = new tblDieuPhoiVanChuyen()
                {
                    CreateDate = DateTime.Now,
                    IDCreateUser = _object.IDUser,
                    tblSysUser = context.tblSysUsers.FirstOrDefault(x => x.ID == _object.IDUser)
                };
               
                UpdateChuyen(_chuyen, _object);
                _chuyen.SaveData(context);
                var _newDt = NewSelectDieuPhoi(_chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

            
        }

        [HttpGet]
        [Route("api/GetChuyenVanChuyen")]
        public IHttpActionResult GetChuyenVanChuyen(string ProductKey,long IDChuyen) // lấy ra chuyến cần sửa
        {
            try
            {
                var _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");

                return Ok(new { NgayDongHang = _Chuyen.NgayDongHang, NgayTraHang = _Chuyen.NgayTraHang, IDDiemDi = _Chuyen.IDDiemDi, IDDiemDen = _Chuyen.IDDiemDen, IDDMHangHoa = _Chuyen.IDDMHangHoa ,SoKG = _Chuyen.SoKG , SoKhoi = _Chuyen.SoKhoi, SoPL = _Chuyen.SoPL, FlagHangVe = _Chuyen.FlagHangVe,ThoiGianVe = _Chuyen.ThoiGianVe , IDKhachHang = _Chuyen.IDDMCustomer, IDLoaiXe = _Chuyen.IDDMLoaiXe});
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }
        }


        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("api/PutChuyenVanChuyen")] // sửa
        public IHttpActionResult PutChuyenVanChuyen([FromBody] tblDieuPhoiVanChuyenNewDto _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");
            if (_object.IDChuyen == 0) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu truyền vào !");

            if (_object.NgayDongHang == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống Ngày đóng hàng");
            if (_object.IDKhachHang == null || _object.IDDiemDi == null || _object.IDDiemDen == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống khách hàng/Điểm đi/điểm đến");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");
                UpdateChuyen(_Chuyen, _object);
                _Chuyen.SaveData(context);
                var _newDt = NewSelectDieuPhoi(_Chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

            
        }

        [HttpPost]
        [Route("api/DeleteChuyenVanChuyen")]
        public IHttpActionResult DeleteChuyenVanChuyen([FromBody] tblDieuPhoiVanChuyenNewDto _object) //
        {
            try
            {
                var _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần xóa !");
                if(_Chuyen.ListTrangThaiVanChuyen.Count() > 0)
                {
                    return Content(HttpStatusCode.BadRequest, "Không thể xóa chuyến đã vận chuyển !");
                }    
                context.tblDieuPhoiVanChuyens.Remove(_Chuyen);
                context.SaveChanges();
                var res = new
                {
                    result = "Xóa dữ liệu thành công !",
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Không thể xóa chuyến !");
            }
        }
        #endregion
        #region điều phối xe
        // Điều phối xe

        [HttpGet]
        [Route("api/GetListDieuPhoiVanChuyen")] // lọc ds chuyến theo ngày
        public IHttpActionResult GetListDieuPhoiVanChuyen(string ProductKey,long IDUSer, DateTime dtS, DateTime dtE, int Page, int Limit, int TrangThai=0)
        {

            List<tblDieuPhoiVanChuyenDto> LstChuyenDto = new List<tblDieuPhoiVanChuyenDto>();
            var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUSer);
            if (_user == null) return Content(HttpStatusCode.NotFound, "Lỗi dữ liệu !");
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
            List<Expression<Func<tblDieuPhoiVanChuyen, bool>>> Lstfunc = new List<Expression<Func<tblDieuPhoiVanChuyen, bool>>>();
            Expression<Func<tblDieuPhoiVanChuyen, bool>> func = null;
            Expression<Func<tblDieuPhoiVanChuyen, bool>> funcTT = null;
           
            if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) //  lái xe
            {
                func = x => x.NgayDongHang >= dtS && x.NgayDongHang <= dtE && x.IDLaiXe == _user.IDNhanVien;
            }
            else
            {
                func = x => x.NgayDongHang == null || (x.NgayDongHang >= dtS && x.NgayDongHang <= dtE);
            }
            if (TrangThai == (int)EnumTrangThaiDieuPhoiFilterApp.DaNhan) funcTT  = x => x.EnumTrangThaiDieuPhoi == (int)EnumTrangThaiDieuPhoi.NhanLenh;
            else if (TrangThai == (int)EnumTrangThaiDieuPhoiFilterApp.DuocGiao) funcTT = x => x.EnumTrangThaiDieuPhoi == null ;
            else if (TrangThai == (int)EnumTrangThaiDieuPhoiFilterApp.HoanThanh) funcTT = x => x.EnumTrangThaiDieuPhoi == (int)EnumTrangThaiDieuPhoi.HoanThanh;

            Lstfunc.Add(func);
           

            IQueryable<tblDieuPhoiVanChuyen> _dp = context.tblDieuPhoiVanChuyens;
            _dp = _dp.Where(func);
            if (funcTT != null) _dp = _dp.Where(func);
            int _total = _dp.Count();
            if(_total > 0)
            {
                LstChuyenDto = _dp.OrderByDescending(x => x.NgayDongHang).Skip((Page - 1) * Limit).Take(Limit).Select(x =>
          new tblDieuPhoiVanChuyenDto
          {
              IDChuyen = x.ID,
              TrangThaiDieuPhoiIn = x.EnumTrangThaiDieuPhoi,
              TrangThaiVanChuyen = x.tblDMTrangThaiVanChuyen != null ? x.tblDMTrangThaiVanChuyen.NameVI : "",
              NgayDongHangCal = x.NgayDongHang,
              KhachHang = x.tblDMCustomer != null ? x.tblDMCustomer.NameVI : "",
              DonViVanTai = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? "CMC" : (x.tblDMCustomer1 != null ? x.tblDMCustomer1.NameVI : ""),
              HangHoa = x.tblDMHangHoa != null ? x.tblDMHangHoa.NameVI : "",
              DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
              DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
              BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
              LaiXe = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? (x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "") : x.LaiXe,
              SoPL = x.SoPL != null ? x.SoPL.Value.ToString() : "",
              SoKhoi = x.SoKhoi != null ? x.SoKhoi.Value.ToString() : "",
              SoKG = x.SoKG != null ? x.SoKG.Value.ToString() : "",
              LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "",
              NgayTraHangCal = x.NgayDongHang,
              ThoiGianVeCal = x.ThoiGianVe,
              HangVe = x.FlagHangVe == true ? "1" : "0",
              SoGioCho = x.SoGioCho != null ? x.SoGioCho.Value.ToString() : "",
              SoCaLuu = x.SoCaLuu != null ? x.SoCaLuu.Value.ToString() : "",
              VeBenBai = x.VeBenBai != null ? x.VeBenBai.Value.ToString() : "",
              PhatSinhKhac = x.PhatSinhKhac,
              GhiChu = x.GhiChu,
              MaDieuVan = x.CodeDieuVan,
          }).ToList();
            }    
            var res = new
            {
                TotalCount = _total,
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey,
                data = LstChuyenDto,
            };
            return Ok(res);
        }
        

        // lấy ra chuyến cần điều phối
        [HttpGet]
        [Route("api/GetChuyenDieuPhoi")]
        public IHttpActionResult GetChuyenDieuPhoi(string ProductKey,long IDChuyen)
        {
            try
            {
                var _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");

                return Ok(new {BienSoXe = _Chuyen.BienSoXe, LaiXe = _Chuyen.LaiXe,DTLaiXe = _Chuyen.DTLaiXe, IDDonViVanTai = _Chuyen.IDDMCustomerTranport ,  IDXeOTo = _Chuyen.IDDMXeOto, IDLaiXe = _Chuyen.IDLaiXe,SoGioCho = _Chuyen.SoGioCho,SoCaLuu = _Chuyen.SoCaLuu,VeBenBai = _Chuyen.VeBenBai,PhatSinhKhac = _Chuyen.PhatSinhKhac, GhiChu = _Chuyen.GhiChu });
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }
        }

        // lấy ra ds xe ưu tiên
        [HttpGet]
        [Route("api/GetListXeOtoUuTien")]
        public IHttpActionResult GetListXeOtoUuTien(string ProductKey, long IDChuyen)
        {
            try
            {
                var _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");
                var lst = PublicCodeShare.GetListXeDieuPhoiUuTien(context, _Chuyen, context.tblDMXeOtoes.ToList());

                return Ok(lst.Select(x=>x.BienSoXE).ToList());
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }
        }

        // lấy ra thông tin mặc định khi chọn xe
        [HttpGet]
        [Route("api/GetThongTinTuXe")]
        public IHttpActionResult GetThongTinTuXe(string ProductKey, long IDXe)
        {
            try
            {
                var _xe = context.tblDMXeOtoes.FirstOrDefault(x => x.ID == IDXe);
                if (_xe == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy xe!");
                return Ok(new {IDLaiXe = _xe.IDTaiXe});
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }
        }

        [HttpPost]
        [Route("api/UpdateDieuPhoi")] // sửa
        public IHttpActionResult UpdateDieuPhoi([FromBody] DieuPhoiXeDto _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");
            if (_object.IDChuyen == 0) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu truyền vào !");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");
                _Chuyen.BienSoXe = _object.BienSoXe;
                _Chuyen.LaiXe = _object.LaiXe;
                _Chuyen.DTLaiXe = _object.DTLaiXe;
                _Chuyen.IDDMCustomerTranport = _object.IDDonViVanTai;
                _Chuyen.IDDMXeOto = _object.IDXeOTo;
                _Chuyen.IDLaiXe = _object.IDLaiXe;
                _Chuyen.IDUserDieuPhoi = _object.IDUser;
                _Chuyen.SoGioCho = LongNVExport.Getlong(_object.SoGioCho);
                _Chuyen.SoCaLuu = LongNVExport.Getlong(_object.SoCaLuu);
                _Chuyen.VeBenBai = LongNVExport.Getlong(_object.VeBenBai);
                _Chuyen.PhatSinhKhac = _object.PhatSinhKhac;
                _Chuyen.EnumThueXeOrXeMinh = _Chuyen.IDDMXeOto != null || _Chuyen.IDLaiXe != null ? (int)EnumThueXeOrXeMinhJOB.Company : (int)EnumThueXeOrXeMinhJOB.Rental;
                _Chuyen.GhiChu = (_object.GhiChu + "").Length > 0 ? _object.GhiChu : _Chuyen.GhiChu;
                var _userDieuPhoi = context.tblSysUsers.FirstOrDefault(x => x.ID == _object.IDUser);
                _Chuyen.SaveData(context, _userDieuPhoi);
                var _newDt = NewSelectDieuPhoi(_Chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

        }

        // Gửi lệnh
        [HttpPost]
        [Route("api/UpdateGuiLenh")] // sửa
        public IHttpActionResult UpdateGuiLenh([FromBody] DieuPhoiXeDto _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");
            if (_object.IDChuyen == 0) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu truyền vào !");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến !");
                if(_Chuyen.FlagDaDieuPhoi != true || _Chuyen.IDLaiXe == null || _Chuyen.IDDMXeOto == null || _Chuyen.EnumTrangThaiDieuPhoi != null)
                {
                    return Content(HttpStatusCode.Conflict, "Không thể thực hiện gửi lệnh trên chuyến này !");
                }
                _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.GuiLenh;
                context.SaveChanges() ;
                var _newDt = NewSelectDieuPhoi(_Chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

           
        }


        // Bỏ Gửi lệnh
        [HttpPost]
        [Route("api/UpdateBoGuiLenh")] // sửa
        public IHttpActionResult UpdateBoGuiLenh([FromBody] DieuPhoiXeDto _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");
            if (_object.IDChuyen == 0) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu truyền vào !");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến !");
                if (_Chuyen.FlagDaDieuPhoi != true || _Chuyen.IDLaiXe == null || _Chuyen.IDDMXeOto == null || _Chuyen.EnumTrangThaiDieuPhoi != (int)EnumTrangThaiDieuPhoi.GuiLenh)
                {
                    return Content(HttpStatusCode.Conflict, "Không thể thực hiện bỏ gửi lệnh trên chuyến này !");
                }
                _Chuyen.EnumTrangThaiDieuPhoi = null;
                context.SaveChanges();

                var _newDt = NewSelectDieuPhoi(_Chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

           
        }


        // Hủy chuyến
        [HttpPost]
        [Route("api/UpdateHuyChuyen")] // sửa
        public IHttpActionResult UpdateHuyChuyen([FromBody] DieuPhoiXeDto _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");
            if (_object.IDChuyen == 0) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu truyền vào !");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến !");
                if (_Chuyen.FlagDaDieuPhoi != true || _Chuyen.IDLaiXe == null || _Chuyen.IDDMXeOto == null )
                {
                    return Content(HttpStatusCode.Conflict, "Không thể thực hiện hủy chuyến trên chuyến này !");
                }
                _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.ChuyenHuy;
                context.SaveChanges();
                var _newDt = NewSelectDieuPhoi(_Chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }
        }

        #endregion
        #region Báo cáo
        [HttpGet]
        [Route("api/GetListSuachuaXe")] // lọc ds chuyến theo ngày
        public IHttpActionResult GetListSuachuaXe(string ProductKey, long IDUSer, DateTime dtS, DateTime dtE, int Page, int Limit)
        {
            List<ObjectCal> LstObject = new List<ObjectCal>();
            var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUSer);
            if (_user == null) return Content(HttpStatusCode.NotFound, "Lỗi dữ liệu !");
            dtS = dtS.Date;
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
            Expression<Func<tblQuanLySuaChuaXe, bool>> func = null;

            if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) //  lái xe
            {
                func = x => x.NgaySua == null || (x.NgaySua >= dtS && x.NgaySua <= dtE && x.IDLaiXe == _user.IDNhanVien);
            }
            else
            {
                func = x => x.NgaySua == null || (x.NgaySua >= dtS && x.NgaySua <= dtE);
            }
            int _total = context.tblQuanLySuaChuaXes.Where(func).Count();
            if(_total > 0)
            {
                LstObject = context.tblQuanLySuaChuaXes.Where(func).OrderByDescending(x => x.NgaySua).Skip((Page - 1) * Limit).Take(Limit).Select(x => new ObjectCal
                {
                    BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : "",
                    NoiDungSuaChua = x.NoiDungSuaChua,
                    SoLuong = x.SoLuong,
                    DonGia = x.DonGia,
                    ThanhTien = x.ThanhTien,
                    LaiXe = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",
                    NgaySuaCal = x.NgaySua,
                    NgayHoanThanhCal = x.NgayHoanThanh,
                    GARAGE = x.GaraSua,
                    IDUser = x.IDCreateUser,
                }).ToList();
            }    
            var res = new
            {
                TotalCount = _total,
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey,
                data = LstObject,
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GetListDoDau")] // lọc ds chuyến theo ngày
        public IHttpActionResult GetListDoDau(string ProductKey, long IDUSer, DateTime dtS, DateTime dtE, int Page, int Limit)
        {
            List<ObjectCal> LstObject = new List<ObjectCal>();
            var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUSer);
            if (_user == null) return Content(HttpStatusCode.NotFound, "Lỗi dữ liệu !");
            dtS = dtS.Date;
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
            Expression<Func<tblQuanLyDoDau, bool>> func = null;

            if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) //  lái xe
            {
                func = x => x.NgayDoDau == null || (x.NgayDoDau >= dtS && x.NgayDoDau <= dtE && x.IDLaiXe == _user.IDNhanVien);
            }
            else
            {
                func = x => x.NgayDoDau == null || (x.NgayDoDau >= dtS && x.NgayDoDau <= dtE);
            }
            int _total = context.tblQuanLyDoDaus.Where(func).Count();
            if(_total > 0)
            {
                LstObject = context.tblQuanLyDoDaus.Where(func).OrderByDescending(x => x.NgayDoDau).Skip((Page - 1) * Limit).Take(Limit).Select(x => new ObjectCal
                {
                    BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : "",
                    GhiChu = x.GhiChu,
                    SoLuong = x.SoLuong,
                    DonGia = x.DonGia,
                    ThanhTien = x.ThanhTien,
                    LaiXe = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",
                    NgayDoDauCal = x.NgayDoDau,
                    IDUser = x.IDCreateUser,
                    IDXeOTo = x.IDDMXeOto,
                    IDLaiXe = x.IDLaiXe
                }).ToList();
            }    
            
            var res = new
            {
                TotalCount = _total,
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey,
                data = LstObject,
            };
            return Ok(res);
        }

        #endregion
        #endregion
        #region Api lái xe
        // Bỏ Gửi lệnh
        [HttpPost]
        [Route("api/UpdateTrangThaiDieuPhoi")] // sửa
        public IHttpActionResult UpdateTrangThaiDieuPhoi([FromBody] DieuPhoiXeDto _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");
            if (_object.IDChuyen == 0) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu truyền vào !");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến !");
                if (_Chuyen.FlagDaDieuPhoi != true || _Chuyen.IDLaiXe == null || _Chuyen.IDDMXeOto == null )
                {
                    return Content(HttpStatusCode.Conflict, "Chuyến thiếu thông tin.Không thể thực hiện trên chuyến này !");
                }
                if (_object.TrangThai == null)
                {
                    return Content(HttpStatusCode.Conflict, "Nhập trạng thái !");
                }
                if (_object.TrangThai == (int)EnumTrangThaiDieuPhoi.NhanLenh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.NhanLenh;
                else if (_object.TrangThai == -1)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.GuiLenh;
                else if (_object.TrangThai == (int)EnumTrangThaiDieuPhoi.KhongNhanLenh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.KhongNhanLenh;
                else if (_object.TrangThai == (int)EnumTrangThaiDieuPhoi.HoanThanh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.HoanThanh;
                else return Content(HttpStatusCode.NotFound, "Trạng thái không hợp lệ !");
                context.SaveChanges();
                var _newDt = NewSelectDieuPhoi(_Chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

        }


        [HttpPost]
        [Route("api/UpdateTrangThaiVanChuyen")] // sửa
        public IHttpActionResult UpdateTrangThaiVanChuyen([FromBody] DieuPhoiXeDto _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");
            if (_object.IDChuyen == 0) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu truyền vào !");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến !");
                if (_Chuyen.FlagDaDieuPhoi != true || _Chuyen.IDLaiXe == null || _Chuyen.IDDMXeOto == null)
                {
                    return Content(HttpStatusCode.Conflict, "Chuyến thiếu thông tin.Không thể thực hiện trên chuyến này !");
                }
                if (_object.TrangThai == null)
                {
                    return Content(HttpStatusCode.Conflict, "Nhập trạng thái !");
                }
                if (_object.TrangThai == (int)EnumTrangThaiDieuPhoi.NhanLenh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.NhanLenh;
                else if (_object.TrangThai == -1)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.GuiLenh;
                else if (_object.TrangThai == (int)EnumTrangThaiDieuPhoi.KhongNhanLenh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.KhongNhanLenh;
                else if (_object.TrangThai == (int)EnumTrangThaiDieuPhoi.HoanThanh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoi.HoanThanh;
                else return Content(HttpStatusCode.NotFound, "Trạng thái không hợp lệ !");
                context.SaveChanges();
                var _newDt = NewSelectDieuPhoi(_Chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

        }

        // thêm sửa xóa đổ dầu
        [HttpPost]
        [Route("api/UpdateDoDau")] // mới
        public IHttpActionResult UpdateDoDau([FromBody] ObjectCal _object)
        {
            if (_object == null) return Content(HttpStatusCode.NoContent, "Đối tượng rỗng !");

            if (_object.IDUser == null) return Content(HttpStatusCode.LengthRequired, "Người dùng không tồn tại");
            try
            {
                tblQuanLyDoDau _chuyen = null;
                if(_object.ID != null && _object.ID > 0)
                {
                    _chuyen = context.tblQuanLyDoDaus.FirstOrDefault(x => x.ID == _object.ID.Value);
                    if(_chuyen == null)
                    {
                        return Content(HttpStatusCode.LengthRequired, "ID không đúng. Dữ liệu không tồn tại");
                    }    
                }    
                else
                {
                    var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == _object.IDUser);
                    _chuyen = new tblQuanLyDoDau()
                    {
                        CreateDate = DateTime.Now,
                        IDCreateUser = _object.IDUser,
                        tblSysUser = _user,
                        IDLaiXe = _user?.IDNhanVien,
                        tblNhanSu = _user?.tblNhanSu
                    };
                }
                _chuyen.DonGia = _object.DonGia;
                _chuyen.SoLuong = _object.SoLuong ?? 1;
                _chuyen.ThanhTien = _chuyen.ThanhTienCal;
                _chuyen.IDDMXeOto = _object.IDXeOTo;
                _chuyen.NgayDoDau = _object.NgayDoDauCal ?? DateTime.Now;
                _chuyen.GhiChu = _object.GhiChu;
                _chuyen.SaveData(context);
                var _newDt = SelectDataDoDau(_chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }


        }

        private ObjectCal SelectDataDoDau(tblQuanLyDoDau _doDau)
        {
            var _ob = new ObjectCal
            {
                BienSoXe = _doDau.tblDMXeOto != null ? _doDau.tblDMXeOto.BienSoXE : "",
                GhiChu = _doDau.GhiChu,
                SoLuong = _doDau.SoLuong,
                DonGia = _doDau.DonGia,
                ThanhTien = _doDau.ThanhTien,
                LaiXe = _doDau.tblNhanSu != null ? _doDau.tblNhanSu.HoTenVI : "",
                NgayDoDauCal = _doDau.NgayDoDau,
                IDUser = _doDau.IDCreateUser,
                IDXeOTo = _doDau.IDDMXeOto,
                IDLaiXe = _doDau.IDLaiXe,
                ID = _doDau.ID
            };
            return _ob;
        }
        [HttpGet]
        [Route("api/GetDoDau")]
        public IHttpActionResult GetDoDau(string ProductKey, long ID) // lấy ra cần sửa
        {
            try
            {
                var _doDau = context.tblQuanLyDoDaus.FirstOrDefault(x => x.ID == ID);
                if (_doDau == null) return Content(HttpStatusCode.NotFound, "ID không đúng.Không tìm thấy dữ liệu cần sửa !");
                var _ob = SelectDataDoDau(_doDau);
                return Ok(_ob);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }
        }

        [HttpPost]
        [Route("api/DeleteDoDau")]
        public IHttpActionResult DeleteDoDau([FromBody] ObjectCal _object) //
        {
            try
            {
                var _Chuyen = context.tblQuanLyDoDaus.FirstOrDefault(x => x.ID == _object.ID);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu cần xóa !");
                context.tblQuanLyDoDaus.Remove(_Chuyen);
                context.SaveChanges();
                var res = new
                {
                    result = "Xóa dữ liệu thành công !",
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Không thể xóa chuyến !");
            }
        }
        #endregion
    }
}