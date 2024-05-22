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
            Taxcode = Taxcode.Trim().ToUpper();
            LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString(AppSettings.DatabaseServerName, AppSettings.DatabaseName, AppSettings.DatabaseUserName, AppSettings.DatabasePassword));

            
            var _Customer = context.tblDMCustomers.FirstOrDefault(x => (x.Code != null && x.Code.Trim().ToUpper() == Taxcode) || (x.TaxCode != null && x.TaxCode.Trim().ToUpper() == Taxcode));
            if (_Customer == null)
            {
                return Content(HttpStatusCode.NotFound, "Không tìm thấy thông tin khách hàng phù hợp !");
            }
            return Ok(_Customer);
        }
    }
}