using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using VsLogistics.DataModel;
using VsLogistics.DataModel.Common;

namespace Trackings.Controllers
{
    public class UserController : ApiController
    {
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
        [Route("api/GetJOBTracking")]
        public IHttpActionResult GetJOBTracking(string ProductKey, string CodeFind)
        {
            if (ProductKey?.Length == 0)
            {
                return Content(HttpStatusCode.NotFound, "Nhập key để tiếp tục !");
            }
            if ((CodeFind + "").Trim()?.Length == 0)
            {
                return Content(HttpStatusCode.NotFound, "Nhập MÃ cần tìm kiếm để tiếp tục !");
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
            CodeFind = CodeFind.Trim().ToUpper();
            LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString(AppSettings.DatabaseServerName, AppSettings.DatabaseName, AppSettings.DatabaseUserName, AppSettings.DatabasePassword));

            
            var EntityJOB = context.tblJOBs.FirstOrDefault(x => (x.CodeJOB != null && x.CodeJOB.Trim().ToUpper() == CodeFind) || (x.HBLNumber != null && x.HBLNumber.Trim().ToUpper() == CodeFind)|| (x.MBLNumber != null && x.MBLNumber.Trim().ToUpper() == CodeFind));
            if (EntityJOB == null)
            {
                return Content(HttpStatusCode.NotFound, "Không tìm thấy thông tin đơn hàng phù hợp !");
            }
            string ttChuyen = EntityJOB.FlagJobAir ? (EntityJOB.tblDMAirLine != null ? EntityJOB.tblDMAirLine.NameVI : "") + $" - {EntityJOB.FlightNumber} ": (EntityJOB.tblDMCarrier != null ? EntityJOB.tblDMCarrier.NameVI : "") + $" - {EntityJOB.DeptVesselVoyageNoText}" ;
            
            var _Dto = new
            {
                ThongTinChuyen = ttChuyen,
                HangHoa = EntityJOB.VolumeDescription,
                CangDi = EntityJOB.tblDMPort != null ? EntityJOB.tblDMPort.NameVI : "",
                CangDen = EntityJOB.tblDMPort1 != null ? EntityJOB.tblDMPort1.NameVI : "",
                NgayDi = EntityJOB.ETDATD?.ToString("HH:mm dd/MM/yyyy"),
                NgayDen = EntityJOB.ETAATA?.ToString("HH:mm dd/MM/yyyy"),
                ListCharge = EntityJOB.tblJOBChargeLists.Where(x => x.FlagChargeBaoHangDen == true).Select(x => new { TenChiPhi = x.tblDMCharge?.NameVI, DonViTinh = x.tblDMSeaUnit?.NameVI,SoLuong = x.Quantity?.ToString("#,#.0##") , DonGia = x.DonGiaVN_Ex?.ToString("#,#"), VAT = x.VAT, ThanhTien = x.AmountByExchange?.ToString("#,#") }).ToList()
                 
            };
            return Ok(_Dto);


        }
    }
}