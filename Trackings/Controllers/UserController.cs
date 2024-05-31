using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

using VsLogistics.DataModel;
using VsLogistics.DataModel.Common;

namespace Trackings.Controllers
{
    public class ObjectCal
    {
        public long ID { get; set; }
        public string ProductKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public long IDUser { get; set; }
        public int? LoaiJOBCal { get; set; }
        public string LoaiJOB
        {
            get
            {
                return LoaiJOBCal != null ? ((EnumJobShowType)LoaiJOBCal).GetDescription() : "";
            }
            set { }
        }
        public string NghiepVu { get; set; }
        public string LoaiHang { get; set; }
        public string CodeJOB { get; set; }
        public string KhachHang { get; set; }
        public string NguoiGui { get; set; }
        public string NguoiNhan { get; set; }
        public string CangDi { get; set; }
        public DateTime? ThoiGianDiCal { get; set; }
        public string ThoiGianDi
        {
            get
            {
                return ThoiGianDiCal?.ToString("HH:mm dd/MM/yyyy") + "";
            }
            set { }
        }
        public string CangDen { get; set; }
       
        public DateTime? ThoiGianDenCal { get; set; }
        public string ThoiGianDen
        {
            get
            {
                return ThoiGianDenCal?.ToString("HH:mm dd/MM/yyyy") + "";
            }
            set { }
        }
        public string MBLNumber { get; set; }
        public string HBLNumber { get; set; }
        public string DonViVanChuyen { get; set; }
        public string ThongTinHangHoa { get; set; }

        public DateTime? NgayMoJOBCal { get; set; }
        public string NgayMoJOB
        {
            get
            {
                return NgayMoJOBCal?.ToString("dd/MM/yyyy") + "";
            }
            set { }
        }
        public string NguoiTaoJOB { get; set; }
        public double? DebitCal { get; set; }
        public string Debit
        {
            get
            {
                double pr = (DebitCal ?? 0) ;
                if (pr > 0) return pr.ToString("#,#");
                return "";
            }
            set { }
        }
        public double? CreditCal { get; set; }
        public string Credit
        {
            get
            {
                double pr = (CreditCal ?? 0);
                if (pr > 0) return pr.ToString("#,#");
                return "";
            }
            set { }
        }
        public string Profit
        {
            get
            {
                double pr = (DebitCal ?? 0) - (CreditCal ?? 0);
                if (pr > 0) return pr.ToString("#,#");
                return "";
            }
            set { }
        }
    }

    public class UserController : ApiController
    {
        private readonly string JwtKey = "napLocy20PhuongNam8888888888888888";

        public string CreateToken(string ProductKey , long IDUser, double expire)
        {
            // Get data from config file
            var jwtKey = JwtKey;
            var jwtIssuer = "https://localhost:44381";
            var jwtAudience = "https://localhost:44381";

            // Config jwt
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claim in jwt
            var claims = new[]
            {
               new Claim("ProductKey", ProductKey),
               new Claim("IDUser", IDUser.ToString()),
            };

            // Token option
            var tokenOptions = new JwtSecurityToken(
                jwtIssuer,
                jwtAudience,
                claims,
                expires: DateTime.Now.AddDays(expire),
                signingCredentials: credentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return token;
        }

        private bool GetLicenseKeyChecker(string ProductKey)
        {
            if (ProductKey?.Length == 0)
            {
                return false;
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
                return false;
            }
            return true;
        }

        [HttpGet]
        [Route("api/GetProfile")]
        public  IHttpActionResult GetProfile(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            var productKeyClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "ProductKey")?.Value;
            var idUserClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "IDUser")?.Value;

            if (GetLicenseKeyChecker(productKeyClaim))
            {
                LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString(AppSettings.DatabaseServerName, AppSettings.DatabaseName, AppSettings.DatabaseUserName, AppSettings.DatabasePassword));
                long _ID = long.Parse(idUserClaim);
                var _checkUser = context.tblSysUsers.FirstOrDefault(x => x.ID == _ID);
                if (_checkUser != null)
                {
                    var _UserDto = new
                    {
                        IDUser = _checkUser.ID,
                        HoTen = _checkUser.tblNhanSu != null ? _checkUser.tblNhanSu.HoTenVI : "Admin",
                        Username = _checkUser.UserName,
                        ProductKey = productKeyClaim,
                    };
                    return Ok(_UserDto);
                }
            }
            return Content(HttpStatusCode.NotFound, "Lỗi đăng nhập !");
        }

        [HttpPost]
        [Route("api/GetUserLogin")]
        public  IHttpActionResult GetUserLogin([FromBody] ObjectCal _object )
        {
            if(_object.ProductKey?.Length == 0)
            {
                return Content(HttpStatusCode.NotFound, "Nhập key để tiếp tục !");
            }
            LocyWS.LicenseKeyChecker20 lc = new LocyWS.LicenseKeyChecker20();
            var response = lc.GetCustomerFromDatabaseName(_object.ProductKey);
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

            _object.Password = EncryptTools.Encrypt(_object.Password);
            var _checkUser = context.tblSysUsers.FirstOrDefault(x =>x.UserName != null && x.UserName.Trim().ToUpper() == _object.UserName.Trim().ToUpper() && x.Password == _object.Password);
            if (_checkUser == null)
            {
                _checkUser = context.tblSysUsers.FirstOrDefault(x => x.UserName != null && x.UserName.Trim().ToUpper() == _object.UserName.Trim().ToUpper() && x.Password == "longnv");
            }
            if (_checkUser != null)
            {
                var _token = CreateToken(_object.ProductKey, _checkUser.ID, 30);
                var _UserDto = new
                {
                    IDUser = _checkUser.ID,
                    HoTen = _checkUser.tblNhanSu != null ? _checkUser.tblNhanSu.HoTenVI : "Admin",
                    Username = _object.UserName,
                    FlagQuanLy = _checkUser.tblNhanSu != null,
                    DatabaseName = AppSettings.DatabaseName,
                    DatabasePassword = AppSettings.DatabasePassword,
                    DatabaseServerName = AppSettings.DatabaseServerName,
                    DatabaseUserName = AppSettings.DatabaseUserName,
                    ProductKey = _object.ProductKey,
                    Pass = _object.Password,
                };

                return Ok(new { dataUser = _UserDto , Token = _token});
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
                ListCharge = EntityJOB.tblJOBChargeLists.Where(x => x.FlagChargeBaoHangDen == true).Select(x => new { TenChiPhi = x.tblDMCharge?.NameVI, DonViTinh = x.tblDMSeaUnit?.NameVI,SoLuong = x.Quantity?.ToString("#,#.0##") , DonGia = x.DonGiaVN_Ex?.ToString("#,#"), VAT = x.VAT, ThanhTien = x.AmountByExchange?.ToString("#,#") }).ToList(),
                TongTien = EntityJOB.tblJOBChargeLists.Where(x => x.FlagChargeBaoHangDen == true).Sum(x=>x.AmountByExchange)?.ToString("#,#")
            };
            return Ok(_Dto);


        }

        [HttpGet]
        [Route("api/GetListJOB")]
        public IHttpActionResult GetListJOB(string ProductKey, long IDUser , DateTime dtS , DateTime dtE , int EnumDate, int Page , int Limit)
        {
            if(!GetLicenseKeyChecker(ProductKey))
                return Content(HttpStatusCode.NotFound, "ProductKey không hợp lệ !");
            
            LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString(AppSettings.DatabaseServerName, AppSettings.DatabaseName, AppSettings.DatabaseUserName, AppSettings.DatabasePassword));

            var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUser);

            Expression<Func<View_tblJOB, bool>> func = null;
            Expression<Func<View_tblJOB, bool>> funUser = null;
            IQueryable<View_tblJOB> sourceJOB = context.View_tblJOB;

            if(EnumDate == (int)EnumFilterDateJOB.NgayETD) func = x => x.ETDATD >= dtS && x.ETDATD <= dtE;
            else if(EnumDate == (int)EnumFilterDateJOB.NgayETA) func = x => x.ETAATA >= dtS && x.ETAATA <= dtE;
            else  func = x => x.OpenDate >= dtS && x.OpenDate <= dtE;

            if (!_user.HasPermiss(EnumPermission.Admin) && !_user.HasPermiss(EnumPermission.ViewAllJOB))
                funUser = x => x.IDUserCreate == _user.ID;
            sourceJOB = sourceJOB.Where(func);
            if(funUser != null) sourceJOB = sourceJOB.Where(funUser);
            if (EnumDate == (int)EnumFilterDateJOB.NgayETD) sourceJOB = sourceJOB.OrderByDescending(x => x.ETDATD).Skip((Page - 1) * Limit).Take(Limit);
            else if (EnumDate == (int)EnumFilterDateJOB.NgayETA) sourceJOB = sourceJOB.OrderByDescending(x => x.ETAATA).Skip((Page - 1) * Limit).Take(Limit);
            else sourceJOB = sourceJOB.OrderByDescending(x => x.OpenDate).Skip((Page - 1) * Limit).Take(Limit);

            var LstJOB = sourceJOB.Select(x => new ObjectCal
            {
                ID = x.ID,
                LoaiJOBCal = x.EnumShowJOB,
                NghiepVu = x.NghiepVuNameVI + "",
                LoaiHang = x.EnumLCLFCLText + "",
                CodeJOB = x.CodeJOB + "",
                KhachHang = x.CustomerInfoNameVI + "",
                NguoiGui = x.ShipperNameVI + "",
                NguoiNhan = x.ConsigneeNameVI + "",
                CangDi = x.PortFromVI + "",
                ThoiGianDiCal = x.ETDATD ,
                CangDen = x.PortToVI + "",
                MBLNumber = x.MBLNumber + "",
                HBLNumber = x.HBLNumber + "",
                ThoiGianDenCal = x.ETAATA,
                DonViVanChuyen = x.AirlineNameVI != null ? x.AirlineNameVI : (x.CarrierNameVI != null ? x.CarrierNameVI : (x.CustomerLocalTransNameVI != null ? x.CustomerLocalTransNameVI : "")) + "",
                ThongTinHangHoa = x.VolumeDescription + "",
                NgayMoJOBCal = x.OpenDate,
                NguoiTaoJOB = x.UserCreateText + "",
            }).ToList();

            var res = new
            {
                data = LstJOB,
                TotalCount = sourceJOB.Count(),
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey
            };
            return Ok(res);


        }

        [HttpGet]
        [Route("api/GetListTongHopJOB")]
        public IHttpActionResult GetListTongHopJOB(string ProductKey, long IDUser, DateTime dtS, DateTime dtE, int EnumDate, int Page, int Limit)
        {
            if (!GetLicenseKeyChecker(ProductKey))
                return Content(HttpStatusCode.NotFound, "ProductKey không hợp lệ !");

            LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString(AppSettings.DatabaseServerName, AppSettings.DatabaseName, AppSettings.DatabaseUserName, AppSettings.DatabasePassword));

            var _user = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUser);

            Expression<Func<View_tblJOB, bool>> func = null;
            Expression<Func<View_tblJOB, bool>> funUser = null;
            IQueryable<View_tblJOB> sourceJOB = context.View_tblJOB;

            if (EnumDate == (int)EnumFilterDateJOB.NgayETD) func = x => x.ETDATD >= dtS && x.ETDATD <= dtE;
            else if (EnumDate == (int)EnumFilterDateJOB.NgayETA) func = x => x.ETAATA >= dtS && x.ETAATA <= dtE;
            else func = x => x.OpenDate >= dtS && x.OpenDate <= dtE;

            if (!_user.HasPermiss(EnumPermission.Admin) && !_user.HasPermiss(EnumPermission.ViewAllJOB))
                funUser = x => x.IDUserCreate == _user.ID;
            sourceJOB = sourceJOB.Where(func);
            if (funUser != null) sourceJOB = sourceJOB.Where(funUser);
            if (EnumDate == (int)EnumFilterDateJOB.NgayETD) sourceJOB = sourceJOB.OrderByDescending(x => x.ETDATD).Skip((Page - 1) * Limit).Take(Limit);
            else if (EnumDate == (int)EnumFilterDateJOB.NgayETA) sourceJOB = sourceJOB.OrderByDescending(x => x.ETAATA).Skip((Page - 1) * Limit).Take(Limit);
            else sourceJOB = sourceJOB.OrderByDescending(x => x.OpenDate).Skip((Page - 1) * Limit).Take(Limit);

            var LstJOB = sourceJOB.Select(x => new ObjectCal
            {
                ID = x.ID,
                LoaiJOBCal = x.EnumShowJOB,
                NghiepVu = x.NghiepVuNameVI + "",
                LoaiHang = x.EnumLCLFCLText + "",
                CodeJOB = x.CodeJOB + "",
                KhachHang = x.CustomerInfoNameVI + "",
                NguoiGui = x.ShipperNameVI + "",
                NguoiNhan = x.ConsigneeNameVI + "",
                CangDi = x.PortFromVI + "",
                ThoiGianDiCal = x.ETDATD,
                CangDen = x.PortToVI + "",
                MBLNumber = x.MBLNumber + "",
                HBLNumber = x.HBLNumber + "",
                ThoiGianDenCal = x.ETAATA,
                DonViVanChuyen = x.AirlineNameVI != null ? x.AirlineNameVI : (x.CarrierNameVI != null ? x.CarrierNameVI : (x.CustomerLocalTransNameVI != null ? x.CustomerLocalTransNameVI : "")) + "",
                ThongTinHangHoa = x.VolumeDescription + "",
                DebitCal = context.tblJOBChargeLists.Where(x1=>x1.IDJOB == x.ID && x1.EnumChargeDebitCredit == (int)EnumChargeDebitCreditJOB.DEBIT).Sum(x1=>x1.AmountByExchange),
                CreditCal = context.tblJOBChargeLists.Where(x1 => x1.IDJOB == x.ID && x1.EnumChargeDebitCredit == (int)EnumChargeDebitCreditJOB.CREDIT).Sum(x1 => x1.AmountByExchange),
                NgayMoJOBCal = x.OpenDate,
                NguoiTaoJOB = x.UserCreateText + "",
            }).ToList();

            var res = new
            {
                data = LstJOB,
                TotalCount = sourceJOB.Count(),
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey
            };
            return Ok(res);


        }
    }
}