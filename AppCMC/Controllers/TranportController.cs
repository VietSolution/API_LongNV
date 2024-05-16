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
            _Chuyen.IDDMCustomer = LongNVExport.Getlong(_object.IDKhachHang?.ToString());
            _Chuyen.IDDiemDi = LongNVExport.Getlong(_object.IDDiemDi?.ToString());
            _Chuyen.IDDiemDen = LongNVExport.Getlong(_object.IDDiemDen?.ToString());
            _Chuyen.IDDMHangHoa = LongNVExport.Getlong(_object.IDHangHoa?.ToString());
            _Chuyen.SoKG = LongNVExport.GetDouble(_object.SoKG?.ToString());
            _Chuyen.SoKhoi = LongNVExport.GetDouble(_object.SoKhoi?.ToString());
            _Chuyen.SoPL = LongNVExport.GetDouble(_object.SoPL?.ToString());
            _Chuyen.FlagHangVe = _object.FlagHangVe == "1" ? true : false;
            _Chuyen.NgayDongHang = LongNVExport.GetDate(_object.ThoiGianVe);
        }
        #endregion

        #region Api Admin
        [HttpGet]
        [Route("api/GetListXe")]
        public IHttpActionResult GetTrangThaiXeTrongNgay() // danh sách xe trên màn hình chính Admin
        {
            List<tblDMXeDto> LstAllXeSelect = new List<tblDMXeDto>();
            var LstAllXe = context.tblDMXeOtoes.ToList();

            foreach(var _xe in LstAllXe)
            {
                tblDMXeDto _New = new tblDMXeDto()
                {
                    ID = _xe.ID,
                    BienSoXe = _xe.BienSoXE,
                    SoLuongChuyen = context.tblDieuPhoiVanChuyens.Where(x => x.NgayDongHang != null && x.NgayDongHang.Value.Year == DateTime.Now.Year && x.NgayDongHang.Value.Month == DateTime.Now.Month && x.IDDMXeOto == _xe.ID).Count().ToString(),
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
        [Route("api/GetTrangThaiXeTrongNgay")]
        public IHttpActionResult GetListChuyenXe(long IDXe) // Lấy ds chuyến của 1 xe
        {
            List<tblDieuPhoiVanChuyenDto> LstChuyenDto = new List<tblDieuPhoiVanChuyenDto>();
            
            Expression<Func<tblDieuPhoiVanChuyen, bool>> func = null;

            func = x =>x.IDDMXeOto == IDXe && x.NgayDongHang.Value.Year == DateTime.Now.Year && x.NgayDongHang.Value.Month == DateTime.Now.Month && x.NgayDongHang.Value.Day == DateTime.Now.Day;
            LstChuyenDto = context.tblDieuPhoiVanChuyens.Where(func).Select(x =>
            new tblDieuPhoiVanChuyenDto
            {
                ID = x.ID,
                BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
                DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
                DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
                NgayDongHang = x.NgayDongHang,
                NgayTraHang = x.NgayTraHang,
            }).ToList();
            if(LstChuyenDto.Count() > 0) return Ok(LstChuyenDto);
            else
            {
                return Content(HttpStatusCode.NotFound, "Xe không có chuyến trong ngày !");
            }
        }

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
                ID = x.ID,
                BienSoXe = x.tblDMXeOto != null ? x.tblDMXeOto.BienSoXE : x.BienSoXe,
                DiemDi = x.tblDMDoor != null ? x.tblDMDoor.AddressVI : "",
                DiemDen = x.tblDMDoor1 != null ? x.tblDMDoor1.AddressVI : "",
                DonViVanTai = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? "CMC" : (x.tblDMCustomer1 != null ? x.tblDMCustomer1.NameVI : ""),
                HangHoa = x.tblDMHangHoa != null ? x.tblDMHangHoa.NameVI : "",
                HangVe = x.FlagHangVe == true ? "1" : "0",
                KhachHang = x.tblDMCustomer != null ? x.tblDMCustomer.NameVI : "",
                LaiXe = x.EnumThueXeOrXeMinh == (int)EnumThueXeOrXeMinhJOB.Company ? (x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "") : x.LaiXe,
                NgayDongHang = x.NgayDongHang,
                NgayTraHang = x.NgayTraHang,
                SoKhoi = x.SoKhoi != null ? x.SoKhoi.Value.ToString() : "",
                SoKG = x.SoKG != null ? x.SoKG.Value.ToString() : "",
                SoPL = x.SoPL != null ? x.SoPL.Value.ToString() : "",
                ThoiGianVe = x.ThoiGianVe != null ? x.ThoiGianVe.Value.ToString() : "",

            }).ToList();
            return Ok(LstChuyenDto);
        }

        [HttpPut]
        //[ResponseType(typeof(void))]
        [Route("api/PutChuyenVanChuyen")]
        public IHttpActionResult PutChuyenVanChuyen([FromBody] tblDieuPhoiVanChuyenNewDto _object)
        {
            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.PreconditionFailed, "Lỗi kiểu dữ liệu đầu vào");
            }
            if (_object == null) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu !");
           
            if (_object.NgayDongHang == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống Ngày đóng hàng");
            if (_object.IDKhachHang == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống khách hàng");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = new tblDieuPhoiVanChuyen()
                {
                    CreateDate = DateTime.Now,
                    IDCreateUser = _object.IDUser,
                    tblSysUser = context.tblSysUsers.FirstOrDefault(x => x.ID == _object.ID)
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

        [HttpPost]
        //[ResponseType(typeof(void))]
        [Route("api/PostChuyenVanChuyen")]
        public IHttpActionResult PostChuyenVanChuyen([FromBody] tblDieuPhoiVanChuyenNewDto _object)
        {
            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.PreconditionFailed, "Lỗi kiểu dữ liệu đầu vào");
            }
            if (_object == null) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu !");
            if (_object.ID == 0) return Content(HttpStatusCode.LengthRequired, "Lỗi dữ liệu truyền vào !");

            if (_object.NgayDongHang == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống Ngày đóng hàng");
            if (_object.IDKhachHang == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống khách hàng");

            try
            {
                tblDieuPhoiVanChuyen _Chuyen = null;
                _Chuyen = context.tblDieuPhoiVanChuyens.FirstOrDefault(x => x.ID == _object.ID);
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

        #endregion
    }
}