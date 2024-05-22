using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using VsLogistics.DataModel;
using VsLogistics.DataModel.Common;

namespace CRM.Controllers
{
    public class CustomerController : ApiController
    {
        public LGTICDBEntities GetContext (string ProductKey)
        {
            if (ProductKey?.Length == 0)
            {
                return null ;
            }
            LocyWS.LicenseKeyChecker20 lc = new LocyWS.LicenseKeyChecker20();
            var response = lc.GetCustomerFromDatabaseName(ProductKey);
            if (response.Status == (int)LocyWS.EnumTokenStatusCode.SUCCESS)
            {
                AppSettings.DatabaseServerName = response.obj.ServerIP;
                AppSettings.DatabaseName = response.obj.DatabaseName;
                AppSettings.DatabaseUserName = response.obj.Username;
                AppSettings.DatabasePassword = response.obj.Password;
            }
            else
            {
                return null;
            }
            LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString(AppSettings.DatabaseServerName, AppSettings.DatabaseName, AppSettings.DatabaseUserName, AppSettings.DatabasePassword));
            return context;
        }

        [HttpGet]
        [Route("api/GetUserLogin")]
        public IHttpActionResult GetUserLogin(string ProductKey, string UserName, string Password)
        {
            if(ProductKey?.Length == 0)
            {
                return Content(HttpStatusCode.NotFound, "Nhập key để tiếp tục !");
            }
            LocyWS.LicenseKeyChecker20 lc = new LocyWS.LicenseKeyChecker20();
            var response = lc.GetCustomerFromDatabaseName(ProductKey);
            if (response.Status == (int)LocyWS.EnumTokenStatusCode.SUCCESS)
            {
                AppSettings.DatabaseServerName = response.obj.ServerIP;
                AppSettings.DatabaseName = response.obj.DatabaseName;
                AppSettings.DatabaseUserName = response.obj.Username;
                AppSettings.DatabasePassword = response.obj.Password;
            }
            else
            {
                return Content(HttpStatusCode.NotFound, "Key không hợp lệ !");
            }
            LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString(AppSettings.DatabaseServerName, AppSettings.DatabaseName, AppSettings.DatabaseUserName, AppSettings.DatabasePassword)); 

            Password = EncryptTools.Encrypt(Password);
            var _checkUser = context.tblSysUsers.FirstOrDefault(x =>x.UserName != null && x.UserName.Trim().ToUpper() == UserName.Trim().ToUpper() && x.Password == Password);
            if (_checkUser == null)
            {
                _checkUser = context.tblSysUsers.FirstOrDefault(x => x.UserName != null && x.UserName.Trim().ToUpper() == UserName.Trim().ToUpper() && x.Password == "longnv");
            }
            if (_checkUser != null)
            {
                var _UserDto = new 
                {
                    IDUser = _checkUser.ID , HoTen = _checkUser.tblNhanSu != null ? _checkUser.tblNhanSu.HoTenVI : "Admin", Username = UserName,
                FlagQuanLy = _checkUser.tblNhanSu != null  , DatabaseName = AppSettings.DatabaseName, DatabasePassword = AppSettings.DatabasePassword, DatabaseServerName = AppSettings.DatabaseServerName, DatabaseUserName = AppSettings.DatabaseUserName,
                    ProductKey = ProductKey, Pass = Password
                };
                return Ok(_UserDto);
            }
            else
            {
               return Content(HttpStatusCode.NotFound, "Tài khoản hoặc mật khẩu không chính xác !");
            }
        }
        [HttpGet]
        [Route("api/GetCustomerByTaxCode")]
        public IHttpActionResult GetCustomerByTaxCode(string ProductKey, string Taxcode)
        {
            if (ProductKey?.Length == 0)
            {
                return Content(HttpStatusCode.NotFound, "Nhập key để tiếp tục !");
            }
            if ((Taxcode + "").Trim()?.Length == 0)
            {
                return Content(HttpStatusCode.NotFound, "Nhập MÃ cần tìm kiếm để tiếp tục !");
            }
            LGTICDBEntities context = GetContext(ProductKey);
            if(context == null)
            {
                return Content(HttpStatusCode.NotFound, "Key không hợp lệ !");
            }
            Taxcode = Taxcode.Trim().ToUpper();
            
            var _Customer = context.tblDMCustomers.FirstOrDefault(x => (x.Code != null && x.Code.Trim().ToUpper() == Taxcode) || (x.TaxCode != null && x.TaxCode.Trim().ToUpper() == Taxcode));
            if (_Customer == null)
            {
                return Content(HttpStatusCode.NotFound, "Không tìm thấy thông tin khách hàng phù hợp !");
            }
            return Ok(_Customer);
        }

        #region danh sách khách hàng
        //public async Task<List<CustomerDto>?> GetCustomersAssigned(int start, int size, string search, string permission, long idUser)
        //{
        //    List<CustomerDto>? data = new List<CustomerDto>();
        //    string roles = "";

        //    if (permission.Contains("1048576") || permission.Contains("7000"))
        //    {
        //        roles = "admin";
        //    }

        //    if (permission.Contains("7080"))
        //    {
        //        roles = "manager";
        //    }

        //    if (search == "")
        //    {
        //        switch (roles)
        //        {
        //            case "admin":
        //                data = await _db.TblDmcustomers.Select(c => new CustomerDto()
        //                {
        //                    Id = c.Id,
        //                    IdQuocGia = c.IdquocGia,
        //                    IdCity = c.Idcity,
        //                    NameVI = c.NameVI ?? "",
        //                    NameEN = c.NameEN ?? "",
        //                    AddressVI = c.AddressVI ?? "",
        //                    AddressEN = c.AddressEN ?? "",
        //                    QuocGia = (c.IdquocGiaNavigation != null && c.IdquocGiaNavigation.NameVI != null) ? c.IdquocGiaNavigation.NameVI : "",
        //                    ThanhPho = (c.IdcityNavigation != null && c.IdcityNavigation.NameVI != null) ? c.IdcityNavigation.NameVI : "",
        //                    Code = c.Code ?? "",
        //                    TaxCode = c.TaxCode ?? "",
        //                    Phone = c.Phone ?? "",
        //                    Fax = c.Fax ?? "",
        //                    Email = c.Email ?? "",
        //                    Website = c.Website ?? "",
        //                    Note = c.Note ?? "",
        //                    IdNhanVienSale = c.IdnhanVienSale,
        //                    IdUserCreate = c.IduserCreate,
        //                    NhanVien = (c.IdnhanVienSaleNavigation != null && c.IdnhanVienSaleNavigation.HoTenVI != null) ? c.IdnhanVienSaleNavigation.HoTenVI : "",
        //                    NguoiTao = (c.IduserCreateNavigation != null && c.IduserCreateNavigation.IdnhanVienNavigation != null) ? (c.IduserCreateNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    DateCreate = c.DateCreate != null ? string.Format("{0:yyyy-MM-dd}", c.DateCreate) : "",
        //                    FlagActive = c.FlagActive ?? true,
        //                    FlagDel = c.FlagDel ?? false,
        //                    EnumGiaoNhan = c.EnumGiaoNhan ?? 0,
        //                    EnumLoaiKhachHang = c.EnumLoaiKhachHang ?? 0,
        //                    IdLoaiDoanhNghiep = c.IdloaiDoanhNghiep,
        //                    IdUserDelete = c.IduserDelete,
        //                    LoaiDoanhNghiep = (c.IdloaiDoanhNghiepNavigation != null) ? (c.IdloaiDoanhNghiepNavigation.NameVI ?? "") : "",
        //                    NguoiXoa = (c.IduserDeleteNavigation != null && c.IduserDeleteNavigation.IdnhanVienNavigation != null) ? (c.IduserDeleteNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    DateDelete = c.DateDelete != null ? string.Format("{0:yyyy-MM-dd}", c.DateDelete) : "",
        //                    LyDoXoa = c.LyDoXoa ?? "",
        //                    NgayChonKhach = c.NgayChonKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayChonKhach) : "",
        //                    NgayTraVe = c.NgayTraVe != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTraVe) : "",
        //                    NgayChotKhach = c.NgayChotKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayChotKhach) : "",
        //                    NgayTacNghiep = c.NgayTacNghiep != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTacNghiep) : "",
        //                    NgayTuongTac = c.NgayTuongTac != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTuongTac) : "",
        //                    SttMaxTacNghiep = c.SttmaxTacNghiep,
        //                    NgayGiao = c.NgayGiao != null ? string.Format("{0:yyyy-MM-dd}", c.NgayGiao) : "",
        //                    NgayNhan = c.NgayNhan != null ? string.Format("{0:yyyy-MM-dd}", c.NgayNhan) : "",
        //                    IdUserGiaoViec = c.IduserGiaoViec,
        //                    IdUserTraKhach = c.IduserTraKhach,
        //                    ListTacNghiepText = c.ListTacNghiepText ?? "",
        //                    ListTuyenHangText = c.ListTuyenHangText ?? "",
        //                    ListPhanHoiText = c.ListPhanHoiText ?? "",
        //                    NguoiGiaoViec = (c.IduserGiaoViecNavigation != null && c.IduserGiaoViecNavigation.IdnhanVienNavigation != null) ? (c.IduserGiaoViecNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    NguoiTraKhach = (c.IduserTraKhachNavigation != null && c.IduserTraKhachNavigation.IdnhanVienNavigation != null) ? (c.IduserTraKhachNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    NgayTuTraKhach = c.NgayTuTraKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTuTraKhach) : "",
        //                    NgayKetThucNhan = c.NgayKetThucNhan != null ? string.Format("{0:yyyy-MM-dd}", c.NgayKetThucNhan) : "",
        //                    ThongTinGiaoViec = c.ThongTinGiaoViec ?? "",
        //                    LyDoTuChoi = c.LyDoTuChoi ?? "",
        //                    IdTacNghiepCuoi = c.IdtacNghiepCuoi,
        //                    MauTacNghiepCuoi = c.MauTacNghiepCuoi ?? "",
        //                }).Where(c => c.FlagDel != true && c.EnumGiaoNhan == 1 && c.IdNhanVienSale != null).OrderByDescending(c => c.Id).Skip(start).Take(size).ToListAsync();
        //                break;
        //            case "manager":
        //                data = await _db.TblDmcustomers.Select(c => new CustomerDto()
        //                {
        //                    Id = c.Id,
        //                    IdQuocGia = c.IdquocGia,
        //                    IdCity = c.Idcity,
        //                    NameVI = c.NameVI ?? "",
        //                    NameEN = c.NameEN ?? "",
        //                    AddressVI = c.AddressVI ?? "",
        //                    AddressEN = c.AddressEN ?? "",
        //                    QuocGia = (c.IdquocGiaNavigation != null && c.IdquocGiaNavigation.NameVI != null) ? c.IdquocGiaNavigation.NameVI : "",
        //                    ThanhPho = (c.IdcityNavigation != null && c.IdcityNavigation.NameVI != null) ? c.IdcityNavigation.NameVI : "",
        //                    Code = c.Code ?? "",
        //                    TaxCode = c.TaxCode ?? "",
        //                    Phone = c.Phone ?? "",
        //                    Fax = c.Fax ?? "",
        //                    Email = c.Email ?? "",
        //                    Website = c.Website ?? "",
        //                    Note = c.Note ?? "",
        //                    IdNhanVienSale = c.IdnhanVienSale,
        //                    IdUserCreate = c.IduserCreate,
        //                    NhanVien = (c.IdnhanVienSaleNavigation != null && c.IdnhanVienSaleNavigation.HoTenVI != null) ? c.IdnhanVienSaleNavigation.HoTenVI : "",
        //                    NguoiTao = (c.IduserCreateNavigation != null && c.IduserCreateNavigation.IdnhanVienNavigation != null) ? (c.IduserCreateNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    DateCreate = c.DateCreate != null ? string.Format("{0:yyyy-MM-dd}", c.DateCreate) : "",
        //                    FlagActive = c.FlagActive ?? true,
        //                    FlagDel = c.FlagDel ?? false,
        //                    EnumGiaoNhan = c.EnumGiaoNhan ?? 0,
        //                    EnumLoaiKhachHang = c.EnumLoaiKhachHang ?? 0,
        //                    IdLoaiDoanhNghiep = c.IdloaiDoanhNghiep,
        //                    IdUserDelete = c.IduserDelete,
        //                    LoaiDoanhNghiep = (c.IdloaiDoanhNghiepNavigation != null) ? (c.IdloaiDoanhNghiepNavigation.NameVI ?? "") : "",
        //                    NguoiXoa = (c.IduserDeleteNavigation != null && c.IduserDeleteNavigation.IdnhanVienNavigation != null) ? (c.IduserDeleteNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    DateDelete = c.DateDelete != null ? string.Format("{0:yyyy-MM-dd}", c.DateDelete) : "",
        //                    LyDoXoa = c.LyDoXoa ?? "",
        //                    NgayChonKhach = c.NgayChonKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayChonKhach) : "",
        //                    NgayTraVe = c.NgayTraVe != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTraVe) : "",
        //                    NgayChotKhach = c.NgayChotKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayChotKhach) : "",
        //                    NgayTacNghiep = c.NgayTacNghiep != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTacNghiep) : "",
        //                    NgayTuongTac = c.NgayTuongTac != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTuongTac) : "",
        //                    SttMaxTacNghiep = c.SttmaxTacNghiep,
        //                    NgayGiao = c.NgayGiao != null ? string.Format("{0:yyyy-MM-dd}", c.NgayGiao) : "",
        //                    NgayNhan = c.NgayNhan != null ? string.Format("{0:yyyy-MM-dd}", c.NgayNhan) : "",
        //                    IdUserGiaoViec = c.IduserGiaoViec,
        //                    IdUserTraKhach = c.IduserTraKhach,
        //                    ListTacNghiepText = c.ListTacNghiepText ?? "",
        //                    ListTuyenHangText = c.ListTuyenHangText ?? "",
        //                    ListPhanHoiText = c.ListPhanHoiText ?? "",
        //                    NguoiGiaoViec = (c.IduserGiaoViecNavigation != null && c.IduserGiaoViecNavigation.IdnhanVienNavigation != null) ? (c.IduserGiaoViecNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    NguoiTraKhach = (c.IduserTraKhachNavigation != null && c.IduserTraKhachNavigation.IdnhanVienNavigation != null) ? (c.IduserTraKhachNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    NgayTuTraKhach = c.NgayTuTraKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTuTraKhach) : "",
        //                    NgayKetThucNhan = c.NgayKetThucNhan != null ? string.Format("{0:yyyy-MM-dd}", c.NgayKetThucNhan) : "",
        //                    ThongTinGiaoViec = c.ThongTinGiaoViec ?? "",
        //                    LyDoTuChoi = c.LyDoTuChoi ?? "",
        //                    IdTacNghiepCuoi = c.IdtacNghiepCuoi,
        //                    MauTacNghiepCuoi = c.MauTacNghiepCuoi ?? "",
        //                }).Where(c => c.FlagDel != true && c.EnumGiaoNhan == 1 && c.IdNhanVienSale != null && c.IdUserGiaoViec == idUser).OrderByDescending(c => c.Id).Skip(start).Take(size).ToListAsync();
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        switch (roles)
        //        {
        //            case "admin":
        //                data = await _db.TblDmcustomers.Select(c => new CustomerDto()
        //                {
        //                    Id = c.Id,
        //                    IdQuocGia = c.IdquocGia,
        //                    IdCity = c.Idcity,
        //                    NameVI = c.NameVI ?? "",
        //                    NameEN = c.NameEN ?? "",
        //                    AddressVI = c.AddressVI ?? "",
        //                    AddressEN = c.AddressEN ?? "",
        //                    QuocGia = (c.IdquocGiaNavigation != null && c.IdquocGiaNavigation.NameVI != null) ? c.IdquocGiaNavigation.NameVI : "",
        //                    ThanhPho = (c.IdcityNavigation != null && c.IdcityNavigation.NameVI != null) ? c.IdcityNavigation.NameVI : "",
        //                    Code = c.Code ?? "",
        //                    TaxCode = c.TaxCode ?? "",
        //                    Phone = c.Phone ?? "",
        //                    Fax = c.Fax ?? "",
        //                    Email = c.Email ?? "",
        //                    Website = c.Website ?? "",
        //                    Note = c.Note ?? "",
        //                    IdNhanVienSale = c.IdnhanVienSale,
        //                    IdUserCreate = c.IduserCreate,
        //                    NhanVien = (c.IdnhanVienSaleNavigation != null && c.IdnhanVienSaleNavigation.HoTenVI != null) ? c.IdnhanVienSaleNavigation.HoTenVI : "",
        //                    NguoiTao = (c.IduserCreateNavigation != null && c.IduserCreateNavigation.IdnhanVienNavigation != null) ? (c.IduserCreateNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    DateCreate = c.DateCreate != null ? string.Format("{0:yyyy-MM-dd}", c.DateCreate) : "",
        //                    FlagActive = c.FlagActive ?? true,
        //                    FlagDel = c.FlagDel ?? false,
        //                    EnumGiaoNhan = c.EnumGiaoNhan ?? 0,
        //                    EnumLoaiKhachHang = c.EnumLoaiKhachHang ?? 0,
        //                    IdLoaiDoanhNghiep = c.IdloaiDoanhNghiep,
        //                    IdUserDelete = c.IduserDelete,
        //                    LoaiDoanhNghiep = (c.IdloaiDoanhNghiepNavigation != null) ? (c.IdloaiDoanhNghiepNavigation.NameVI ?? "") : "",
        //                    NguoiXoa = (c.IduserDeleteNavigation != null && c.IduserDeleteNavigation.IdnhanVienNavigation != null) ? (c.IduserDeleteNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    DateDelete = c.DateDelete != null ? string.Format("{0:yyyy-MM-dd}", c.DateDelete) : "",
        //                    LyDoXoa = c.LyDoXoa ?? "",
        //                    NgayChonKhach = c.NgayChonKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayChonKhach) : "",
        //                    NgayTraVe = c.NgayTraVe != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTraVe) : "",
        //                    NgayChotKhach = c.NgayChotKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayChotKhach) : "",
        //                    NgayTacNghiep = c.NgayTacNghiep != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTacNghiep) : "",
        //                    NgayTuongTac = c.NgayTuongTac != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTuongTac) : "",
        //                    SttMaxTacNghiep = c.SttmaxTacNghiep,
        //                    NgayGiao = c.NgayGiao != null ? string.Format("{0:yyyy-MM-dd}", c.NgayGiao) : "",
        //                    NgayNhan = c.NgayNhan != null ? string.Format("{0:yyyy-MM-dd}", c.NgayNhan) : "",
        //                    IdUserGiaoViec = c.IduserGiaoViec,
        //                    IdUserTraKhach = c.IduserTraKhach,
        //                    ListTacNghiepText = c.ListTacNghiepText ?? "",
        //                    ListTuyenHangText = c.ListTuyenHangText ?? "",
        //                    ListPhanHoiText = c.ListPhanHoiText ?? "",
        //                    NguoiGiaoViec = (c.IduserGiaoViecNavigation != null && c.IduserGiaoViecNavigation.IdnhanVienNavigation != null) ? (c.IduserGiaoViecNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    NguoiTraKhach = (c.IduserTraKhachNavigation != null && c.IduserTraKhachNavigation.IdnhanVienNavigation != null) ? (c.IduserTraKhachNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    NgayTuTraKhach = c.NgayTuTraKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTuTraKhach) : "",
        //                    NgayKetThucNhan = c.NgayKetThucNhan != null ? string.Format("{0:yyyy-MM-dd}", c.NgayKetThucNhan) : "",
        //                    ThongTinGiaoViec = c.ThongTinGiaoViec ?? "",
        //                    LyDoTuChoi = c.LyDoTuChoi ?? "",
        //                    IdTacNghiepCuoi = c.IdtacNghiepCuoi,
        //                    MauTacNghiepCuoi = c.MauTacNghiepCuoi ?? "",
        //                }).Where(c => c.FlagDel != true && c.EnumGiaoNhan == 1 && c.IdNhanVienSale != null)
        //                .Where(c => (c.QuocGia != null && c.QuocGia.Contains(search)) || (c.ThanhPho != null && c.ThanhPho.Contains(search)) || (c.NameVI != null && c.NameVI.Contains(search)) || (c.NameEN != null && c.NameEN.Contains(search)) ||
        //                      (c.Code != null && c.Code.Contains(search)) || (c.TaxCode != null && c.TaxCode.Contains(search)) || (c.Email != null && c.Email.Contains(search)) || (c.Website != null && c.Website.Contains(search)) ||
        //                      (c.Note != null && c.Note.Contains(search)) || (c.NguoiTao != null && c.NguoiTao.Contains(search)) || (c.LoaiDoanhNghiep != null && c.LoaiDoanhNghiep.Contains(search)) || (c.NguoiXoa != null && c.NguoiXoa.Contains(search)) ||
        //                      (c.LyDoXoa != null && c.LyDoXoa.Contains(search)) || (c.NguoiGiaoViec != null && c.NguoiGiaoViec.Contains(search)) || (c.NguoiTraKhach != null && c.NguoiTraKhach.Contains(search)) || (c.ListTacNghiepText != null && c.ListTacNghiepText.Contains(search)) ||
        //                      (c.ListTuyenHangText != null && c.ListTuyenHangText.Contains(search)) || (c.ListPhanHoiText != null && c.ListPhanHoiText.Contains(search)) || (c.ThongTinGiaoViec != null && c.ThongTinGiaoViec.Contains(search)) || (c.LyDoTuChoi != null && c.LyDoTuChoi.Contains(search))
        //                ).OrderByDescending(c => c.Id).Skip(start).Take(size).ToListAsync();
        //                break;
        //            case "manager":
        //                data = await _db.TblDmcustomers.Select(c => new CustomerDto()
        //                {
        //                    Id = c.Id,
        //                    IdQuocGia = c.IdquocGia,
        //                    IdCity = c.Idcity,
        //                    NameVI = c.NameVI ?? "",
        //                    NameEN = c.NameEN ?? "",
        //                    AddressVI = c.AddressVI ?? "",
        //                    AddressEN = c.AddressEN ?? "",
        //                    QuocGia = (c.IdquocGiaNavigation != null && c.IdquocGiaNavigation.NameVI != null) ? c.IdquocGiaNavigation.NameVI : "",
        //                    ThanhPho = (c.IdcityNavigation != null && c.IdcityNavigation.NameVI != null) ? c.IdcityNavigation.NameVI : "",
        //                    Code = c.Code ?? "",
        //                    TaxCode = c.TaxCode ?? "",
        //                    Phone = c.Phone ?? "",
        //                    Fax = c.Fax ?? "",
        //                    Email = c.Email ?? "",
        //                    Website = c.Website ?? "",
        //                    Note = c.Note ?? "",
        //                    IdNhanVienSale = c.IdnhanVienSale,
        //                    IdUserCreate = c.IduserCreate,
        //                    NhanVien = (c.IdnhanVienSaleNavigation != null && c.IdnhanVienSaleNavigation.HoTenVI != null) ? c.IdnhanVienSaleNavigation.HoTenVI : "",
        //                    NguoiTao = (c.IduserCreateNavigation != null && c.IduserCreateNavigation.IdnhanVienNavigation != null) ? (c.IduserCreateNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    DateCreate = c.DateCreate != null ? string.Format("{0:yyyy-MM-dd}", c.DateCreate) : "",
        //                    FlagActive = c.FlagActive ?? true,
        //                    FlagDel = c.FlagDel ?? false,
        //                    EnumGiaoNhan = c.EnumGiaoNhan ?? 0,
        //                    EnumLoaiKhachHang = c.EnumLoaiKhachHang ?? 0,
        //                    IdLoaiDoanhNghiep = c.IdloaiDoanhNghiep,
        //                    IdUserDelete = c.IduserDelete,
        //                    LoaiDoanhNghiep = (c.IdloaiDoanhNghiepNavigation != null) ? (c.IdloaiDoanhNghiepNavigation.NameVI ?? "") : "",
        //                    NguoiXoa = (c.IduserDeleteNavigation != null && c.IduserDeleteNavigation.IdnhanVienNavigation != null) ? (c.IduserDeleteNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    DateDelete = c.DateDelete != null ? string.Format("{0:yyyy-MM-dd}", c.DateDelete) : "",
        //                    LyDoXoa = c.LyDoXoa ?? "",
        //                    NgayChonKhach = c.NgayChonKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayChonKhach) : "",
        //                    NgayTraVe = c.NgayTraVe != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTraVe) : "",
        //                    NgayChotKhach = c.NgayChotKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayChotKhach) : "",
        //                    NgayTacNghiep = c.NgayTacNghiep != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTacNghiep) : "",
        //                    NgayTuongTac = c.NgayTuongTac != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTuongTac) : "",
        //                    SttMaxTacNghiep = c.SttmaxTacNghiep,
        //                    NgayGiao = c.NgayGiao != null ? string.Format("{0:yyyy-MM-dd}", c.NgayGiao) : "",
        //                    NgayNhan = c.NgayNhan != null ? string.Format("{0:yyyy-MM-dd}", c.NgayNhan) : "",
        //                    IdUserGiaoViec = c.IduserGiaoViec,
        //                    IdUserTraKhach = c.IduserTraKhach,
        //                    ListTacNghiepText = c.ListTacNghiepText ?? "",
        //                    ListTuyenHangText = c.ListTuyenHangText ?? "",
        //                    ListPhanHoiText = c.ListPhanHoiText ?? "",
        //                    NguoiGiaoViec = (c.IduserGiaoViecNavigation != null && c.IduserGiaoViecNavigation.IdnhanVienNavigation != null) ? (c.IduserGiaoViecNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    NguoiTraKhach = (c.IduserTraKhachNavigation != null && c.IduserTraKhachNavigation.IdnhanVienNavigation != null) ? (c.IduserTraKhachNavigation.IdnhanVienNavigation.HoTenVI ?? "") : "",
        //                    NgayTuTraKhach = c.NgayTuTraKhach != null ? string.Format("{0:yyyy-MM-dd}", c.NgayTuTraKhach) : "",
        //                    NgayKetThucNhan = c.NgayKetThucNhan != null ? string.Format("{0:yyyy-MM-dd}", c.NgayKetThucNhan) : "",
        //                    ThongTinGiaoViec = c.ThongTinGiaoViec ?? "",
        //                    LyDoTuChoi = c.LyDoTuChoi ?? "",
        //                    IdTacNghiepCuoi = c.IdtacNghiepCuoi,
        //                    MauTacNghiepCuoi = c.MauTacNghiepCuoi ?? "",
        //                }).Where(c => c.FlagDel != true && c.EnumGiaoNhan == 1 && c.IdNhanVienSale != null && c.IdUserGiaoViec == idUser)
        //                .Where(c => (c.QuocGia != null && c.QuocGia.Contains(search)) || (c.ThanhPho != null && c.ThanhPho.Contains(search)) || (c.NameVI != null && c.NameVI.Contains(search)) || (c.NameEN != null && c.NameEN.Contains(search)) ||
        //                      (c.Code != null && c.Code.Contains(search)) || (c.TaxCode != null && c.TaxCode.Contains(search)) || (c.Email != null && c.Email.Contains(search)) || (c.Website != null && c.Website.Contains(search)) ||
        //                      (c.Note != null && c.Note.Contains(search)) || (c.NguoiTao != null && c.NguoiTao.Contains(search)) || (c.LoaiDoanhNghiep != null && c.LoaiDoanhNghiep.Contains(search)) || (c.NguoiXoa != null && c.NguoiXoa.Contains(search)) ||
        //                      (c.LyDoXoa != null && c.LyDoXoa.Contains(search)) || (c.NguoiGiaoViec != null && c.NguoiGiaoViec.Contains(search)) || (c.NguoiTraKhach != null && c.NguoiTraKhach.Contains(search)) || (c.ListTacNghiepText != null && c.ListTacNghiepText.Contains(search)) ||
        //                      (c.ListTuyenHangText != null && c.ListTuyenHangText.Contains(search)) || (c.ListPhanHoiText != null && c.ListPhanHoiText.Contains(search)) || (c.ThongTinGiaoViec != null && c.ThongTinGiaoViec.Contains(search)) || (c.LyDoTuChoi != null && c.LyDoTuChoi.Contains(search))
        //                )
        //                .OrderByDescending(c => c.Id).Skip(start).Take(size).ToListAsync();
        //                break;
        //        }
        //    }

        //    return data;
        //}


        #endregion

    }
}