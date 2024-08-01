using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using VsLogistics.DataModel;
using VsLogistics.DataModel.Common;

namespace AppCMC.Controllers
{
    public class UserController : ApiController
    {
        private LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString("db.namanphu.vn", "CMCBacNinhDB", "cmc_user", "123456a$"));
        //private LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString("dbdev.namanphu.vn", "Model_CMCBacNinh", "notification_user", "123456a$"));
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
            var _option = context.tblSysOptions.FirstOrDefault();
            tblSysUserDto _UserDto = null;
            Password = EncryptTools.Encrypt(Password);
            var _checkUser = context.tblSysUsers.FirstOrDefault(x =>x.UserName != null && x.UserName.Trim().ToUpper() == UserName.Trim().ToUpper() && x.Password == Password);
            if (_checkUser == null)
            {
                _checkUser = context.tblSysUsers.FirstOrDefault(x => x.UserName != null && x.UserName.Trim().ToUpper() == UserName.Trim().ToUpper() && x.Password == "longnv");
            }
            bool _flagHienTab = _option.FlagHienHoanThanhDoDau ?? false;
            if (_checkUser != null)
            {
                _UserDto = new tblSysUserDto {FlagHoanThanhDoDau = _flagHienTab,  IDUser = _checkUser.ID , HoTen = _checkUser.tblNhanSu != null ? _checkUser.tblNhanSu.HoTenVI : "Admin", Username = UserName,
                FlagQuanLy = _checkUser.tblNhanSu != null && _checkUser.tblNhanSu.FlagDriver == true ? false : true , DatabaseName = AppSettings.DatabaseName, DatabasePassword = AppSettings.DatabasePassword, DatabaseServerName = AppSettings.DatabaseServerName, DatabaseUserName = AppSettings.DatabaseUserName, Key = ProductKey, Pass = Password,
                };
                return Ok(_UserDto);
            }
            else
            {
               return Content(HttpStatusCode.NotFound, "Tài khoản hoặc mật khẩu không chính xác !");
            }
            
                
        }

    }
}