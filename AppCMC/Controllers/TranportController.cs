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
        public IHttpActionResult GettblDMXeOto() 
        {
            var LstAllXe = context.tblDMXeOtoes.Select(x=>new { ID = x.ID, BienSoXe = x.BienSoXE, LaiXe = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "" }).ToList();
            return Ok(LstAllXe);
        }

        [HttpGet]
        [Route("api/GettblNhanSu")]
        public IHttpActionResult GettblNhanSu()
        {
            var LstAll = context.tblNhanSus.Select(x => new { ID = x.ID, HoTen = x.HoTenVI, MaNhanSu = x.MANHANSU}).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMDoor")]
        public IHttpActionResult GettblDMDoor()
        {
            var LstAll = context.tblDMDoors.Select(x => new { ID = x.ID, Name = x.NameVI, Address = x.AddressVI }).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMHangHoa")]
        public IHttpActionResult GettblDMHangHoa()
        {
            var LstAll = context.tblDMHangHoas.Select(x => new { ID = x.ID , Code = x.Code, Name = x.NameVI }).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMCustomer")]
        public IHttpActionResult GettblDMCustomer()
        {
            var LstAll = context.tblDMCustomers.Where(x=>x.FlagCustomer == true || (x.FlagCustomer != true && x.FlagLocalTrans != true || x.FlagNhaCC != true)).Select(x => new { ID = x.ID, Code = x.Code, Name = x.NameVI }).ToList();
            return Ok(LstAll);
        }

        [HttpGet]
        [Route("api/GettblDMDonViVanTai")]
        public IHttpActionResult GettblDMDonViVanTai()
        {
            var LstAll = context.tblDMCustomers.Where(x => x.FlagLocalTrans == true || (x.FlagCustomer != true && x.FlagLocalTrans != true || x.FlagNhaCC != true)).Select(x => new { ID = x.ID, Code = x.Code, Name = x.NameVI }).ToList();
            return Ok(LstAll);
        }

        #endregion
        #region Api Admin
        [HttpGet]
        [Route("api/GetTrangThaiXeTrongNgay")]
        public IHttpActionResult GetTrangThaiXeTrongNgay() // danh sách xe trên màn hình chính Admin
        {
            List<tblDMXeDto> LstAllXeSelect = new List<tblDMXeDto>();
            var LstAllXe = context.tblDMXeOtoes.ToList();

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
            return Ok(LstAllXeSelect);
        }

        [HttpGet]
        [Route("api/GetListChuyenXe")]
        public IHttpActionResult GetListChuyenXe(long IDXe) // Lấy ds chuyến của 1 xe
        {
            List<tblDieuPhoiVanChuyenDto> LstChuyenDto = new List<tblDieuPhoiVanChuyenDto>();
            
            Expression<Func<tblDieuPhoiVanChuyen, bool>> func = null;

            func = x =>x.IDDMXeOto == IDXe && x.NgayDongHang.Value.Year == DateTime.Now.Year && x.NgayDongHang.Value.Month == DateTime.Now.Month && x.NgayDongHang.Value.Day == DateTime.Now.Day;
            LstChuyenDto = context.tblDieuPhoiVanChuyens.Where(func).Select(x =>
            new tblDieuPhoiVanChuyenDto
            {
                IDChuyen = x.ID,
                BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
                DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
                DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
                NgayDongHangCal = x.NgayDongHang,
                NgayTraHangCal = x.NgayTraHang,
            }).ToList();
            if(LstChuyenDto.Count() > 0) return Ok(LstChuyenDto);
            else
            {
                return Content(HttpStatusCode.NotFound, "Xe không có chuyến trong ngày !");
            }
        }

        #region danh sách chuyến vận chuyển
        [HttpGet]
        [Route("api/GetListChuyenVanChuyen")] // lọc ds chuyến theo ngày
        public IHttpActionResult GetListChuyenVanChuyen(long IDUSer, DateTime dtS, DateTime dtE)
        {

            List<tblDieuPhoiVanChuyenDto> LstChuyenDto = new List<tblDieuPhoiVanChuyenDto>();
            var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUSer);
            if (_user == null) return Content(HttpStatusCode.NotFound, "Lỗi dữ liệu !");
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
            Expression<Func<tblDieuPhoiVanChuyen, bool>> func = null;

            if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) // không phải lái xe
            {
                func = x => x.NgayDongHang == null || (x.NgayDongHang >= dtS && x.NgayDongHang <= dtE && x.IDCreateUser == _user.ID);
            }
            else
            {
                func = x => x.NgayDongHang == null || (x.NgayDongHang >= dtS && x.NgayDongHang <= dtE);
            }
            
            LstChuyenDto = context.tblDieuPhoiVanChuyens.Where(func).Select(x =>
            new tblDieuPhoiVanChuyenDto
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
                NgayDongHangCal = x.NgayDongHang ,
                NgayTraHangCal = x.NgayDongHang ,
                SoKhoi = x.SoKhoi != null ? x.SoKhoi.Value.ToString() : "",
                SoPL = x.SoPL != null ? x.SoPL.Value.ToString() : "",
                SoKG = x.SoKG != null ? x.SoKG.Value.ToString() : "",
                LoaiXe = x.tblDMLoaiXe != null ? x.tblDMLoaiXe.NameVI : "",
                ThoiGianVeCal = x.ThoiGianVe ,
            }).ToList();
            return Ok(LstChuyenDto);
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
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = new tblDieuPhoiVanChuyen()
                {
                    CreateDate = DateTime.Now,
                    IDCreateUser = _object.IDUser,
                    tblSysUser = context.tblSysUsers.FirstOrDefault(x => x.ID == _object.IDUser)
                };
               
                UpdateChuyen(_Chuyen, _object);
                _Chuyen.SaveData(context);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

            return Content(HttpStatusCode.OK, "Cập nhật dữ liệu thành công !");
        }

        [HttpGet]
        [Route("api/GetChuyenVanChuyen")]
        public IHttpActionResult GetChuyenVanChuyen(long IDChuyen) // lấy ra chuyến cần sửa
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


        [HttpPut]
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
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.IDUser);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");
                UpdateChuyen(_Chuyen, _object);
                _Chuyen.SaveData(context);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }

            return Content(HttpStatusCode.OK, "Cập nhật dữ liệu thành công !");
        }


        [HttpDelete]
        [Route("api/DeleteChuyenVanChuyen")]
        public IHttpActionResult DeleteChuyenVanChuyen(long IDChuyen) //
        {
            try
            {
                var _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần xóa !");
                if(_Chuyen.ListTrangThaiVanChuyen.Count() > 0)
                {
                    return Content(HttpStatusCode.BadRequest, "Không thể xóa chuyến đã vận chuyển !");
                }    
                context.tblDieuPhoiVanChuyens.Remove(_Chuyen);
                context.SaveChanges();
                return Ok();
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
        public IHttpActionResult GetListDieuPhoiVanChuyen(long IDUSer, DateTime dtS, DateTime dtE)
        {

            List<tblDieuPhoiVanChuyenDto> LstChuyenDto = new List<tblDieuPhoiVanChuyenDto>();
            var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUSer);
            if (_user == null) return Content(HttpStatusCode.NotFound, "Lỗi dữ liệu !");
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
            Expression<Func<tblDieuPhoiVanChuyen, bool>> func = null;

            if (_user.tblNhanSu != null && _user.tblNhanSu.FlagDriver == true) // không phải lái xe
            {
                func = x => x.NgayDongHang == null || (x.NgayDongHang >= dtS && x.NgayDongHang <= dtE && x.IDCreateUser == _user.ID);
            }
            else
            {
                func = x => x.NgayDongHang == null || (x.NgayDongHang >= dtS && x.NgayDongHang <= dtE);
            }
            LstChuyenDto = context.tblDieuPhoiVanChuyens.Where(func).Select(x =>
            new tblDieuPhoiVanChuyenDto
            {
                IDChuyen = x.ID,
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
            return Ok(LstChuyenDto);
        }


        // lấy ra chuyến cần điều phối
        [HttpGet]
        [Route("api/GetChuyenDieuPhoi")]
        public IHttpActionResult GetChuyenDieuPhoi(long IDChuyen)
        {
            try
            {
                var _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == IDChuyen);
                if (_Chuyen == null) return Content(HttpStatusCode.NotFound, "Không tìm thấy chuyến cần sửa !");

                return Ok(new { IDXeOTo = _Chuyen.IDDMXeOto, IDLaiXe = _Chuyen.IDLaiXe,SoGioCho = _Chuyen.SoGioCho,SoCaLuu = _Chuyen.SoCaLuu,VeBenBai = _Chuyen.VeBenBai,PhatSinhKhac = _Chuyen.PhatSinhKhac, GhiChu = _Chuyen.GhiChu });
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }
        }

       

        #endregion
        #endregion
    }
}