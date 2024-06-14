

using Newtonsoft.Json;

using PublicCodeLongNV.ExportExcel;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using VsLogistics.DataModel;

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
            _Chuyen.tblDMDoor = context.tblDMDoors.FirstOrDefault(x => x.ID == _Chuyen.IDDiemDi);
            _Chuyen.IDDiemDen = _object.IDDiemDen;
            _Chuyen.tblDMDoor1 = context.tblDMDoors.FirstOrDefault(x => x.ID == _Chuyen.IDDiemDen);
            _Chuyen.IDDMHangHoa = _object.IDHangHoa;
            _Chuyen.tblDMCustomer = context.tblDMCustomers.FirstOrDefault(x => x.ID == _Chuyen.IDDMCustomer);
            _Chuyen.SoKG = _object.SoKG;
            _Chuyen.SoKhoi = _object.SoKhoi;
            _Chuyen.SoPL = _object.SoPL;
            _Chuyen.FlagHangVe = _object.FlagHangVe ;
            _Chuyen.ThoiGianVe = _object.ThoiGianVe;
            _Chuyen.IDDMLoaiXe = _object.IDLoaiXe;
            _Chuyen.tblDMLoaiXe = context.tblDMLoaiXes.FirstOrDefault(x => x.ID == _Chuyen.IDDMLoaiXe);
        }
        #endregion
        #region Danh mục
        [HttpGet]
        [Route("api/GettblDMXeOto")]
        public IHttpActionResult GettblDMXeOto(string ProductKey) 
        {
            var LstAllXe = context.tblDMXeOtoes.Select(x=>new { ID = x.ID, BienSoXe = x.BienSoXE, LaiXe = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "" }).ToList();
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAllXe
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GettblDMLoaiXe")]
        public IHttpActionResult GettblDMLoaiXe(string ProductKey)
        {
           
            var LstAllXe = context.tblDMLoaiXes.Select(x => new { ID = x.ID,Code = x.Code, Name = x.NameVI, TrongTai = x.TrongTai }).ToList();
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAllXe
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GettblNhanSu")]
        public IHttpActionResult GettblNhanSu(string ProductKey)
        {
            var LstAll = context.tblNhanSus.Select(x => new { ID = x.ID, HoTen = x.HoTenVI, MaNhanSu = x.MANHANSU}).ToList();
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAll
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GettblDMDoor")]
        public IHttpActionResult GettblDMDoor(string ProductKey)
        {
            var LstAll = context.tblDMDoors.Select(x => new { ID = x.ID, Name = x.NameVI, Address = x.AddressVI }).ToList();

            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAll
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GettblDMHangHoa")]
        public IHttpActionResult GettblDMHangHoa(string ProductKey)
        {
            var LstAll = context.tblDMHangHoas.Select(x => new { ID = x.ID , Code = x.Code, Name = x.NameVI }).ToList();
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAll
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GettblDMCustomer")]
        public IHttpActionResult GettblDMCustomer(string ProductKey)
        {
            var LstAll = context.tblDMCustomers.Where(x=>x.FlagCustomer == true || (x.FlagCustomer != true && x.FlagLocalTrans != true || x.FlagNhaCC != true)).Select(x => new { ID = x.ID, Code = x.Code, Name = x.NameVI }).ToList();
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAll
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GettblDMDonViVanTai")]
        public IHttpActionResult GettblDMDonViVanTai(string ProductKey)
        {
            var LstAll = context.tblDMCustomers.Where(x => x.FlagLocalTrans == true || (x.FlagCustomer != true && x.FlagLocalTrans != true || x.FlagNhaCC != true)).Select(x => new { ID = x.ID, Code = x.Code, Name = x.NameVI }).ToList();
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAll
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("api/GettblDMTrangThaiVanChuyen")]
        public IHttpActionResult GettblDMTrangThaiVanChuyen(string ProductKey)
        {
            var LstAll = context.tblDMTrangThaiVanChuyens.Select(x => new { ID = x.ID, RGB = x.RGB, Name = x.NameVI }).ToList();
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAll
            };
            return Ok(res);
        }

        #endregion
        #region Api Admin
        [HttpGet]
        [Route("api/GetTrangThaiXeTrongNgay")]
        public IHttpActionResult GetTrangThaiXeTrongNgay(string ProductKey,int Page,int Limit) // danh sách xe trên màn hình chính Admin
        {
            int _skip =(Page -1)* Limit;
            var LstAllXe = context.tblDMXeOtoes.Select(x=> new tblDMXeDto { IDXe = x.ID,BienSoXe = x.BienSoXE} ).ToList();
            int _count = LstAllXe.Count();
            foreach(var _xe in LstAllXe)
            {
                _xe.SoLuongChuyen = context.tblDieuPhoiVanChuyens.Where(x => x.NgayDongHang != null && x.NgayDongHang.Value.Year == DateTime.Now.Year && x.NgayDongHang.Value.Month == DateTime.Now.Month && x.IDDMXeOto == _xe.IDXe && x.EnumTrangThaiDieuPhoi != (int)EnumTrangThaiDieuPhoiVC.ChuyenHuy).Count();
                var _ChuyenHT = context.tblDieuPhoiVanChuyens.Where(x => x.NgayDongHang != null && x.EnumTrangThaiDieuPhoi != (int)EnumTrangThaiDieuPhoiVC.HoanThanh && x.EnumTrangThaiDieuPhoi != (int)EnumTrangThaiDieuPhoiVC.ChuyenHuy && x.IDDMXeOto == _xe.IDXe && x.NgayDongHang <= DateTime.Now && ((x.FlagHangVe != true && x.NgayTraHang >= DateTime.Now) || (x.FlagHangVe == true && x.ThoiGianVe >= DateTime.Now))).FirstOrDefault();
                if (_ChuyenHT != null)
                {
                    var _tt = _ChuyenHT.ListTrangThaiVanChuyen.OrderByDescending(x => x.NgayGioThucHien).FirstOrDefault();
                    _xe.TrangThai = _tt?.tblDMTrangThaiVanChuyen?.NameVI + $" - {_tt?.NgayGioThucHien?.ToString("HH:mm")}";
                    _xe.RGB = _tt?.tblDMTrangThaiVanChuyen?.RGB;
                }
            }
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                data = LstAllXe.OrderByDescending(x => x.SoLuongChuyen).Skip(_skip).Take(Limit).ToList(),
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
               LstChuyenDto = context.tblDieuPhoiVanChuyens.Where(func).OrderByDescending(x=>x.NgayDongHang).Skip((Page - 1) * Limit).Take(Limit).Select(x =>
              new tblDieuPhoiVanChuyenDto
              {
                  IDChuyen = x.ID,
                  BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
                  DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
                  DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
                  NgayDongHangCal = x.NgayDongHang,
                  NgayTraHangCal = x.NgayTraHang,
              }).ToList();
            }    
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
                TotalCount = _total,
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey,
                data = LstChuyenDto,
            };
            return Ok(res);
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
                    NgayTraHangCal = x.NgayTraHang,
                    SoKhoi = x.SoKhoi != null ? x.SoKhoi.Value.ToString() : "",
                    SoPL = x.SoPL != null ? x.SoPL.Value.ToString() : "",
                    SoKG = x.SoKG != null ? x.SoKG.Value.ToString() : "",
                    LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "",
                    ThoiGianVeCal = x.ThoiGianVe,
                }).ToList();
            }
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
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
                IDLaiXe = x.IDLaiXe,
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
                RGB = x.tblDMTrangThaiVanChuyen != null ? x.tblDMTrangThaiVanChuyen.RGB : "",
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

                var _newDt =  new { NgayDongHang = _Chuyen.NgayDongHang, NgayTraHang = _Chuyen.NgayTraHang, IDDiemDi = _Chuyen.IDDiemDi ,DiemDi = _Chuyen.tblDMDoor?.NameVI, IDDiemDen = _Chuyen.IDDiemDen, DiemDen = _Chuyen.tblDMDoor1?.NameVI, IDDMHangHoa = _Chuyen.IDDMHangHoa, HangHoa  = _Chuyen.tblDMHangHoa?.NameVI, SoKG = _Chuyen.SoKG , SoKhoi = _Chuyen.SoKhoi, SoPL = _Chuyen.SoPL, FlagHangVe = _Chuyen.FlagHangVe,ThoiGianVe = _Chuyen.ThoiGianVe , IDKhachHang = _Chuyen.IDDMCustomer, KhachHang = _Chuyen.tblDMCustomer?.NameVI, IDLoaiXe = _Chuyen.IDDMLoaiXe, LoaiXe = _Chuyen.tblDMLoaiXe?.NameVI };
                var res = new
                {
                    result = "Lấy dữ liệu thành công !",
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
            Expression<Func<tblDieuPhoiVanChuyen, bool>> func = null;
            Expression<Func<tblDieuPhoiVanChuyen, bool>> funcTT = null;
           
           
            if (TrangThai == (int)EnumTrangThaiDieuPhoiFilterApp.DaNhan)
            {
                funcTT = x => x.EnumTrangThaiDieuPhoi == (int)EnumTrangThaiDieuPhoiVC.NhanLenh;
                if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) //  lái xe
                {
                    func = x => x.IDLaiXe == _user.IDNhanVien;
                }
            } 
            else if (TrangThai == (int)EnumTrangThaiDieuPhoiFilterApp.DuocGiao)
            {
                funcTT = x => x.EnumTrangThaiDieuPhoi == (int)EnumTrangThaiDieuPhoiVC.GuiLenh;
                if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) //  lái xe
                {
                    func = x => x.IDLaiXe == _user.IDNhanVien;
                }
            } 
            else if (TrangThai == (int)EnumTrangThaiDieuPhoiFilterApp.HoanThanh)
            {
                funcTT = x => x.EnumTrangThaiDieuPhoi == (int)EnumTrangThaiDieuPhoiVC.HoanThanh;
                if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) //  lái xe
                {
                    func = x => x.NgayDongHang >= dtS && x.NgayDongHang <= dtE && x.IDLaiXe == _user.IDNhanVien;
                }
                else
                {
                    func = x => x.NgayDongHang == null || (x.NgayDongHang >= dtS && x.NgayDongHang <= dtE);
                }
            }
            else
            {
                if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) //  lái xe
                {
                    func = x => x.NgayDongHang >= dtS && x.NgayDongHang <= dtE && x.IDLaiXe == _user.IDNhanVien;
                }
                else
                {
                    func = x => x.NgayDongHang == null || (x.NgayDongHang >= dtS && x.NgayDongHang <= dtE);
                }
            } 
                
            IQueryable<tblDieuPhoiVanChuyen> _dp = context.tblDieuPhoiVanChuyens;
            if (func != null) _dp = _dp.Where(func);
            if (funcTT != null) _dp = _dp.Where(funcTT);
            int _total = _dp.Count();
            if(_total > 0)
            {
                LstChuyenDto = _dp.ToList().OrderBy(x => x.STT_SapXep).Skip((Page - 1) * Limit).Take(Limit).Select(x =>
          new tblDieuPhoiVanChuyenDto
          {
              IDChuyen = x.ID,
              TrangThaiDieuPhoiIn = x.EnumTrangThaiDieuPhoi,
              TrangThaiVanChuyen = x.tblDMTrangThaiVanChuyen != null ? x.tblDMTrangThaiVanChuyen.NameVI : "",
              RGB = x.tblDMTrangThaiVanChuyen != null ? x.tblDMTrangThaiVanChuyen.RGB : "",
              NgayDongHangCal = x.NgayDongHang,
              KhachHang = x.tblDMCustomer != null ? x.tblDMCustomer.NameVI : "",
              DonViVanTai = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? "CMC" : (x.tblDMCustomer1 != null ? x.tblDMCustomer1.NameVI : ""),
              HangHoa = x.tblDMHangHoa != null ? x.tblDMHangHoa.NameVI : "",
              DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
              DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
              BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
              LaiXe = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? (x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "") : x.LaiXe,
              IDLaiXe = x.IDLaiXe,
              SoPL = x.SoPL != null ? x.SoPL.Value.ToString() : "",
              SoKhoi = x.SoKhoi != null ? x.SoKhoi.Value.ToString() : "",
              SoKG = x.SoKG != null ? x.SoKG.Value.ToString() : "",
              LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "",
              NgayTraHangCal = x.NgayTraHang,
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
                result = "Lấy dữ liệu thành công !",
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
        public IHttpActionResult GetChuyenDieuPhoi(string ProductKey,long IDChuyen , int EnumThueXeOrXeMinh )
        {
            try
            {
                var _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");
                if(_Chuyen.EnumTrangThaiDieuPhoi != null)
                {
                    return Content(HttpStatusCode.BadRequest, "Chuyển đã được điều phối !");
                }    
                if(EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company)
                {
                    var _data = new { IDXeOto = _Chuyen.IDDMXeOto, BienSoXe = _Chuyen.tblDMXeOto?.BienSoXE, IDLaiXe = _Chuyen.IDLaiXe, LaiXe = _Chuyen.tblNhanSu?.HoTenVI, SoGioCho = _Chuyen.SoGioCho, SoCaLuu = _Chuyen.SoCaLuu, VeBenBai = _Chuyen.VeBenBai, PhatSinhKhac = _Chuyen.PhatSinhKhac, GhiChu = _Chuyen.GhiChu };
                    var res = new
                    {
                        result = "Lấy dữ liệu thành công !",
                        data = _data,
                    };
                    return Ok(res);
                }    
                else if (EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Rental)
                {
                    var _data = new { BienSoXe = _Chuyen.BienSoXe, LaiXe = _Chuyen.LaiXe, DTLaiXe = _Chuyen.DTLaiXe, IDDonViVanTai = _Chuyen.IDDMCustomerTranport, SoGioCho = _Chuyen.SoGioCho, SoCaLuu = _Chuyen.SoCaLuu, VeBenBai = _Chuyen.VeBenBai, PhatSinhKhac = _Chuyen.PhatSinhKhac, GhiChu = _Chuyen.GhiChu };
                    var res = new
                    {
                        result = "Lấy dữ liệu thành công !",
                        data = _data,
                    };
                    return Ok(res);
                }
                else
                {
                    var _data = new { BienSoXe = _Chuyen.BienSoXe, LaiXe = _Chuyen.LaiXe, DTLaiXe = _Chuyen.DTLaiXe, IDDonViVanTai = _Chuyen.IDDMCustomerTranport, IDXeOto = _Chuyen.IDDMXeOto, IDLaiXe = _Chuyen.IDLaiXe, SoGioCho = _Chuyen.SoGioCho, SoCaLuu = _Chuyen.SoCaLuu, VeBenBai = _Chuyen.VeBenBai, PhatSinhKhac = _Chuyen.PhatSinhKhac, GhiChu = _Chuyen.GhiChu };
                    var res = new
                    {
                        result = "Lấy dữ liệu thành công !",
                        data = _data,
                    };
                    return Ok(res);
                } 
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
                if(lst.Count() == 0)
                {
                    return Content(HttpStatusCode.NotFound, "Không tìm thấy xe phù hợp !");
                }    
                var res = new
                {
                    result = "Lấy dữ liệu thành công !",
                    data = lst.Select(x => new { ID = x.ID, BienSoXe = x.BienSoXE, LaiXe = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "", LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "" }).ToList(),
                };
                return Ok(res);
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
                var res = new
                {
                    result = "Lấy dữ liệu thành công !",
                    data = new { IDLaiXe = _xe.IDTaiXe , TaiXe = _xe.tblNhanSu?.NameVI},
                };
                return Ok(res);
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

                if(_object.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company)
                {
                    _Chuyen.IDDMXeOto = _object.IDXeOto;
                    _Chuyen.IDLaiXe = _object.IDLaiXe;
                    _Chuyen.BienSoXe = null;
                    _Chuyen.LaiXe = null;
                    _Chuyen.DTLaiXe = null;
                    _Chuyen.IDDMCustomerTranport = null;
                }    
                else
                {
                    _Chuyen.IDDMXeOto = null;
                    _Chuyen.IDLaiXe = null;
                    _Chuyen.BienSoXe = _object.BienSoXe;
                    _Chuyen.LaiXe = _object.LaiXe;
                    _Chuyen.DTLaiXe = _object.DTLaiXe;
                    _Chuyen.IDDMCustomerTranport = _object.IDDonViVanTai;
                }    
               
                
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
                _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoiVC.GuiLenh;
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
                if (_Chuyen.FlagDaDieuPhoi != true || _Chuyen.IDLaiXe == null || _Chuyen.IDDMXeOto == null || _Chuyen.EnumTrangThaiDieuPhoi != (int)EnumTrangThaiDieuPhoiVC.GuiLenh)
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
                _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoiVC.ChuyenHuy;
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
        [Route("api/GetListSuaChuaXe")] // lọc ds chuyến theo ngày
        public IHttpActionResult GetListSuaChuaXe(string ProductKey, long IDUSer, DateTime dtS, DateTime dtE, int Page, int Limit)
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
                result = "Lấy dữ liệu thành công !",
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
                    ID = x.ID,
                    BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : "",
                    GhiChu = x.GhiChu,
                    SoLuong = x.SoLuong,
                    DonGia = x.DonGia,
                    ThanhTien = x.ThanhTien,
                    LaiXe = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",
                    NgayDoDauCal = x.NgayDoDau,
                    IDUser = x.IDCreateUser,
                    IDXeOto = x.IDDMXeOto,
                    IDLaiXe = x.IDLaiXe
                }).ToList();
            }    
            
            var res = new
            {
                result = "Lấy dữ liệu thành công !",
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
                if (_object.TrangThai == (int)EnumTrangThaiDieuPhoiVC.NhanLenh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoiVC.NhanLenh;
                else if (_object.TrangThai == -1)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoiVC.GuiLenh;
                else if (_object.TrangThai == (int)EnumTrangThaiDieuPhoiVC.KhongNhanLenh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoiVC.KhongNhanLenh;
                else if (_object.TrangThai == (int)EnumTrangThaiDieuPhoiVC.HoanThanh)
                    _Chuyen.EnumTrangThaiDieuPhoi = (int)EnumTrangThaiDieuPhoiVC.HoanThanh;
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

       
        [HttpGet]
        [Route("api/GetPathLocal")]
        public IHttpActionResult GetPathLocal(string ProductKey)
        {
            string root = SetValueExcel.PathExportView;
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            return Ok(new { PathLocal = root});
        }

        [HttpGet]
        [Route("api/GetPathServer")]
        public IHttpActionResult GetPathServer(string ProductKey)
        {
            
            string root1 = HttpContext.Current.Server.MapPath("~/uploads");
            if (!Directory.Exists(root1))
            {
                Directory.CreateDirectory(root1);
            }

            return Ok(new { PathServer = root1 });
        }

        [HttpGet]
        [Route("api/GetPathSFix")]
        public IHttpActionResult GetPathSFix(string ProductKey)
        {
            string _r = @"D:\ftpuser\dbdev.namanphu.vn\Model_CMCBacNinh\Document Transport\DP1";
            if (!Directory.Exists(_r))
            {
                Directory.CreateDirectory(_r);
            }
            return Ok(new { PathServer = _r });
        }

        [HttpPost]
        [Route("api/UpdateTrangThaiVanChuyen")] // sửa
        public async Task<IHttpActionResult> UpdateTrangThaiVanChuyen()
        {
            
            //string root = HttpContext.Current.Server.MapPath("~/uploads");
            //root = SetValueExcel.PathExportView;
            string root = @"D:\Temp";  // đường dẫn lưu temp trên server
            if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                var provider = new MultipartFormDataStreamProvider(root);
                //var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                //var provider = new MultipartFormDataStreamProvider(uploadsPath);
               

                ObjectCal metadata = null;
                var _ob = provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"') == "data");
                if(_ob == null)
                {
                    return Content(HttpStatusCode.NotFound, "Đối tượng rỗng !");
                }    
                var json = await _ob.ReadAsStringAsync();
                metadata = JsonConvert.DeserializeObject<ObjectCal>(json);


                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == metadata.IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến !");
                if (_Chuyen.FlagDaDieuPhoi != true || _Chuyen.IDLaiXe == null || _Chuyen.IDDMXeOto == null)
                {
                    return Content(HttpStatusCode.NotFound, "Chuyến thiếu thông tin.Không thể thực hiện trên chuyến này !");
                }

               // AppSettings.LicenseKey = "dbdev.namanphu.vn";
                AppSettings.DatabaseServerName = "dbdev.namanphu.vn";
                AppSettings.DatabaseName = "Model_CMCBacNinh";
                AppSettings.DatabaseUserName = "notification_user";
                AppSettings.DatabasePassword = "123456a$";

                AppSettings.ftpurl = "ftp://fs.namanphu.vn";
                AppSettings.ftpuser = "ftpuser";
                AppSettings.ftppass = "123456a$";

                var UserLogin = context.tblSysUsers.FirstOrDefault(x => x.ID == metadata.IDUser);

                var _newTrangThai = new tblDieuPhoiTrangThaiVC
                {
                    IDDieuPhoi = _Chuyen.ID,
                    DateCreate = DateTime.Now,
                    NgayGioThucHien = metadata.NgayGioThucHien ?? DateTime.Now,
                    IDUserCreate = UserLogin.ID,
                    tblSysUser = UserLogin,
                    IDDMTrangThaiVanChuyen = metadata.IDTrangThaiVanChuyen,
                    tblDMTrangThaiVanChuyen = context.tblDMTrangThaiVanChuyens.FirstOrDefault(x => x.ID == metadata.IDTrangThaiVanChuyen),
                };
                _Chuyen.ListTrangThaiVanChuyen.Add(_newTrangThai);
                context.tblDieuPhoiTrangThaiVCs.Add(_newTrangThai);
                _Chuyen.SaveData(context, UserLogin);


                var _doc = new tblJOBDocument
                {
                    IDUserUpload = UserLogin.ID,
                    tblSysUser = UserLogin,
                    CreateDate = DateTime.Now,
                   
                    IDTheoDoiVanChuyen = _newTrangThai?.ID,
                    tblDieuPhoiTrangThaiVC = _newTrangThai
                };
                _newTrangThai.ListDocument.Add(_doc);
                context.tblJOBDocuments.Add(_doc);
               

                // Lưu các file từ yêu cầu vào thư mục

                string _pathServer = "/db.namanphu.vn/CMCBacNinhDB/AppCMC" + $"/{AppSettings.DatabaseName}/DP{_Chuyen.ID}"; // sau này sẽ lấy theo DB chính

                _doc.Path = _pathServer;
                

                PublicCodeShare.ftpClientModel.createDirAndSub(_pathServer);
                
                foreach (var file in provider.FileData)
                {
                    var originalFileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var localFileName = file.LocalFileName;
                    var filePath = Path.Combine(root, originalFileName);

                    // Move the file to the new location
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.Move(localFileName, filePath);
                    List<string> _lstFileName = new List<string>();
                    _lstFileName.Add(filePath);


                    string _fnOnly = Path.GetFileName(filePath);
                    _doc.FileName = _fnOnly + ";" + _doc.FileName;


                    if (!PublicCodeShare.ftpClientModel.uploads(_pathServer, _lstFileName))
                    {
                         Content(HttpStatusCode.InternalServerError, "Chưa thể tải ảnh lên !");
                    }
                    File.Delete(filePath);
                }
                _Chuyen.SaveData(context);
                var _newDt = NewSelectDieuPhoi(_Chuyen);
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = _newDt
                };
                return Ok(res);
            //try
            //{
            //}
            //catch
            //{
            //    return Content(HttpStatusCode.NotFound, "Lỗi dữ liệu !");
            //}

        }

        [HttpGet]
        [Route("api/GetListTrangThaiVanChuyen")]
        public IHttpActionResult GetListTrangThaiVanChuyen(string ProductKey, long IDChuyen)
        {
            try
            {
                var _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");
                AppSettings.DatabaseName = "Model_CMCBacNinh";

                //foreach(var tt in _Chuyen.ListTrangThaiVanChuyen)
                //{
                //    int _count = tt.ListDocument.Where(x => x.FileName != null && x.FileName.Length > 0).SelectMany(x => x.FileNames).Count();
                //    string[] stringArray = new string[_count];
                //    if (_count == 0)
                //    { }   
                //    int sl = 0;
                //    foreach (var c in tt.ListDocument)
                //    {
                //        foreach (var _f in c.FileNames)
                //        {
                //            stringArray[sl] = $"{AppSettings.DatabaseName}/DP{tt.IDDieuPhoi}/{_f}";
                //            sl++;
                //        }
                //    }
                //}    


                var lst = _Chuyen.ListTrangThaiVanChuyen.Select(x => new {ID = x.ID , IDChuyen = x.IDDieuPhoi, IDTrangThaiVanChuyen = x.IDDMTrangThaiVanChuyen, TrangThaiVanChuyen = x.tblDMTrangThaiVanChuyen != null ? x.tblDMTrangThaiVanChuyen.NameVI : "", FileAttach = x.ListFileNameArrayAppText, NgayGioThucHien = x.NgayGioThucHien}  ).ToList();
                var res = new
                {
                    result = "Lấy dữ liệu thành công !",
                    data = lst
                };
                return Ok(res);
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, $"Lỗi dữ liệu: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("api/DeleteTrangThaiVanChuyen")]
        public IHttpActionResult DeleteTrangThaiVanChuyen([FromBody] ObjectCal _object) //
        {
            try
            {
                var _trangThai = context.tblDieuPhoiTrangThaiVCs.FirstOrDefault(x => x.ID == _object.ID);
                if (_trangThai == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu cần xóa !");

                var _chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _trangThai.IDDieuPhoi);

                context.tblJOBDocuments.RemoveRange(_trangThai.tblJOBDocuments);
                context.tblDieuPhoiTrangThaiVCs.Remove(_trangThai);
                _chuyen.ListTrangThaiVanChuyen.Remove(_trangThai);
                _chuyen.SaveData(context);
                var res = new
                {
                    result = "Xóa dữ liệu thành công !",
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Chưa thể xóa dữ liệu !");
            }
        }


        [HttpGet]
        [Route("api/GetXeVanChuyen")]
        public IHttpActionResult GetXeVanChuyen(string ProductKey, long IDUser)
        {
            try
            {
                var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUser);
                var _xe = context.tblDMXeOtoes.FirstOrDefault(x => x.IDTaiXe == _user.IDNhanVien);
                if (_xe == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy xe!");
                var res = new
                {
                    result = "Lấy dữ liệu thành công !",
                    data = new {IDXeOto = _xe.ID,BienSoXe = _xe.BienSoXE , IDLaiXe = _xe.IDTaiXe, TaiXe = _xe.tblNhanSu?.NameVI },
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
                _chuyen.IDDMXeOto = _object.IDXeOto;
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
                IDXeOto = _doDau.IDDMXeOto,
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
                var res = new
                {
                    result = "Lấy dữ liệu thành công !",
                    data = _ob
                };
                return Ok(res);
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

        #region Ảnh
        private string GetContentType(string filePath)
        {
            // Xác định loại nội dung dựa trên phần mở rộng tệp
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            switch (extension)
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                default:
                    return "application/octet-stream"; // Loại nội dung mặc định cho các tệp khác
            }
        }
        [HttpPost]
        [Route("api/UploadImage")]
        public async Task<IHttpActionResult> UploadImage()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Unsupported media type.");
            }

            var uploadsPath = HttpContext.Current.Server.MapPath("~/uploads");
            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            
            var provider = new MultipartFormDataStreamProvider(uploadsPath);

            // Lưu các file từ yêu cầu vào thư mục
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.FileData)
            {
                var originalFileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                var localFileName = file.LocalFileName;
                var filePath = Path.Combine(uploadsPath, originalFileName);

                // Move the file to the new location
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(localFileName, filePath);
            }

            
            return BadRequest("No file uploaded.");
        }
        #endregion
    }
}