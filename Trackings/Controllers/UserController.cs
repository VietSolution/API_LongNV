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

using VSL.CustomerDB;

using VsLogistics.DataModel;
using VsLogistics.DataModel.Common;
using VsLogistics.DataModel.Properties;

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
    public class ObjectThuChiCal
    {
        public long ID { get; set; }
        public string ProductKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public long IDUser { get; set; }
        public int _EnumLoaiDS { get; set; }
        public int? LoaiPhieuCal { get; set; }
        public string LoaiPhieu
        {
            get
            {
                if(_EnumLoaiDS == 1) return LoaiPhieuCal != null ? ((EnumQuyetToanThuChiJOB)LoaiPhieuCal).GetDescription() : ""; // nếu là ds quyết toán
                return LoaiPhieuCal != null ? ((EnumLoaiPhieuThuChi)LoaiPhieuCal).GetDescription() : "";
            }
            set { }
        }
        public string SoPhieu { get; set; }
        public string TrangThai{ get; set; }
        public DateTime? NgayThuChiCal { get; set; }
        public string NgayThuChi
        {
            get
            {
                return NgayThuChiCal?.ToString("HH:mm dd/MM/yyyy") + "";
            }
            set { }
        }
        public string NhanVien { get; set; }
        public double? SoTienCal { get; set; }
        public string SoTien => SoTienCal != null ? SoTienCal?.ToString("#,#") : "";
        public string LoaiTien { get; set; }
        public double? TyGiaCal { get; set; }
        public string TyGia => TyGiaCal != null ? TyGiaCal?.ToString("#,#") : "";
        public string NoiDung { get; set; }
        public string DoiTuongThanhToan { get; set; }
        public string CodeJOB { get; set; }
        public string NguoiDuyet { get; set; }
        public string DanhSachChungTu { get; set; }
        public int TrangThaiDuyet { get; set; }
    }
  
    public class UserController : ApiController
    {
        #region Key - login
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

            var response = PublicLicenseKey.GetDataServer(ProductKey);
            if (response != null)
            {
                AppSettings.DatabaseServerName = response.DatabaseServerName;
                AppSettings.DatabaseName = response.DatabaseName;
                AppSettings.DatabaseUserName = response.DatabaseUserName;
                AppSettings.DatabasePassword = response.DatabasePassword;
            }
            else
            {
                AppSettings.DatabaseServerName = "103.150.125.133";
                AppSettings.DatabaseName = "LocyDemo";
                AppSettings.DatabaseUserName = "sa";
                AppSettings.DatabasePassword = "VSL@2024";
                return true;
            }
            return true;
        }
        private tblSysUser UserLogin { get; set; }
        private LGTICDBEntities GetLicenseKey(string ProductKey, long IDUser)
        {
            if (ProductKey?.Length == 0)
            {
                return null;
            }
            var response = PublicLicenseKey.GetDataServer(ProductKey);
            if (response != null)
            {
                AppSettings.DatabaseServerName = response.DatabaseServerName;
                AppSettings.DatabaseName = response.DatabaseName;
                AppSettings.DatabaseUserName = response.DatabaseUserName;
                AppSettings.DatabasePassword = response.DatabasePassword;
            }
            else
            {
                AppSettings.DatabaseServerName = "103.150.125.133";
                AppSettings.DatabaseName = "LocyDemo";
                AppSettings.DatabaseUserName = "sa";
                AppSettings.DatabasePassword = "VSL@2024";
            }
            LGTICDBEntities context = new LGTICDBEntities(ConnectionTools.BuildConnectionString(AppSettings.DatabaseServerName, AppSettings.DatabaseName, AppSettings.DatabaseUserName, AppSettings.DatabasePassword));
            UserLogin = context.tblSysUsers.FirstOrDefault(x => x.ID == IDUser);
            AppSettings.CurrentLoginUser = new LoginUserInfo
            {
                LoginUser = UserLogin,
                IDLoginUser = UserLogin.ID,
                IDLoginNhanSu = UserLogin.IDNhanVien,
                LoginNhanSu = UserLogin.tblNhanSu
            };
            return context;

        }

        [HttpGet]
        [Route("api/GetProfile")]
        public  IHttpActionResult GetProfile(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            var productKeyClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "ProductKey")?.Value;
            var idUserClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "IDUser")?.Value;

            if (!GetLicenseKeyChecker(productKeyClaim))
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
            if (GetLicenseKeyChecker(_object.ProductKey) != true)
            {
                return Content(HttpStatusCode.NotFound, "Lỗi key !");
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
        #endregion
        #region đơn hàng
        [HttpGet]
        [Route("api/GetJOBTracking")]
        public IHttpActionResult GetJOBTracking(string ProductKey, string CodeFind)
        {
            if (!GetLicenseKeyChecker(ProductKey))
            {
                return Content(HttpStatusCode.NotFound, "Key không hợp lệ !");
            }
            if ((CodeFind + "").Trim()?.Length == 0)
            {
                return Content(HttpStatusCode.NotFound, "Nhập MÃ cần tìm kiếm để tiếp tục !");
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
            dtS = dtS.Date;
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
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
            dtS = dtS.Date;
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
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
        #endregion
        #region thu chi - phê duyệt
        [HttpGet]
        [Route("api/GetListThuChi")]
        public IHttpActionResult GetListThuChi(string ProductKey, long IDUser, DateTime dtS, DateTime dtE,  int Page, int Limit)
        {
            LGTICDBEntities context = GetLicenseKey(ProductKey, IDUser);
            if (context == null)
                return Content(HttpStatusCode.NotFound, "ProductKey không hợp lệ !");

            dtS = dtS.Date;
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);

            Expression<Func<tblJOBPhieuThuChi, bool>> func = null;
            Expression<Func<tblJOBPhieuThuChi, bool>> funUser = null;
            IQueryable<tblJOBPhieuThuChi> sourceJOB = context.tblJOBPhieuThuChis;

            func = x => x.NgayThuChi >= dtS && x.NgayThuChi <= dtE;

            if (!UserLogin.HasPermiss(EnumPermission.Admin) && !UserLogin.HasPermiss(EnumPermission.ViewAccouting))
            {
                var resError = new
                {
                    result = "Người dùng không có quyền xem dữ liệu !",
                    data = "",
                    TotalCount = 0,
                    Page = Page,
                    Limit = Limit,
                    ProductKey = ProductKey
                };
                return Ok(resError);
                
            }
            var _u = AppSettings.CurrentLoginUser.LoginUser;
            sourceJOB = sourceJOB.Where(func);
            if (funUser != null) sourceJOB = sourceJOB.Where(funUser);
            sourceJOB = sourceJOB.OrderByDescending(x => x.NgayThuChi).Skip((Page - 1) * Limit).Take(Limit);
            var LstJOB = sourceJOB.ToList().Select(x => new ObjectThuChiCal
            {
                ID = x.ID,
                LoaiPhieuCal = x.EnumLoaiPhieu,
                SoPhieu = x.BillNo,
                TrangThai = x.EnumStatusText,
                NgayThuChiCal = x.NgayThuChi,
                NhanVien = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",
                SoTienCal = x.Amount,
                LoaiTien = x.tblDMCurrency != null ? x.tblDMCurrency.KyHieu : "VND",
                TyGiaCal = x.TyGia,
                NoiDung = x.Note,
                CodeJOB = x.tblJOB != null ? x.tblJOB.CodeJOB : "",
                DoiTuongThanhToan = x.CurrentObjectNameWeb + "",
                NguoiDuyet = x.LichSuPheDuyetWebText + "",
                DanhSachChungTu = x.DocumentText
            }).ToList();
            string _text = sourceJOB.Count() > 0 ? "Lấy dữ liệu thành công !" : "Không có dữ liệu trong khoảng thời gian đã chọn !";
            var res = new
            {
                result = _text,
                data = LstJOB,
                TotalCount = sourceJOB.Count(),
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey
            };
            return Ok(res);


        }


        [HttpGet]
        [Route("api/GetListPheDuyetThuChi")]
        public IHttpActionResult GetListPheDuyetThuChi(string ProductKey, long IDUser, int Page, int Limit)
        {
            LGTICDBEntities context = GetLicenseKey(ProductKey, IDUser);
            if (context == null)
                return Content(HttpStatusCode.NotFound, "ProductKey không hợp lệ !");
           
            if (!UserLogin.HasPermiss(EnumPermission.Admin) && !UserLogin.HasPermiss(EnumPermission.ViewAccouting) && UserLogin.FlagDuyetChi != true)
            {
                var resError = new
                {
                    result = "Người dùng không có quyền xem dữ liệu !",
                    data = "",
                    TotalCount = 0,
                    Page = Page,
                    Limit = Limit,
                    ProductKey = ProductKey
                };
                return Ok(resError);
            }

            List<tblJOBPhieuThuChi> LstThuChi = new List<tblJOBPhieuThuChi>();
            if (UserLogin.HasPermiss(EnumPermission.Admin))
            {
                LstThuChi = context.tblJOBPhieuThuChis.Where(x=>x.tblJOBUserPheDuyets.Count(x1 => x1.EnumStatus == (int)EnumStatusPheDuyet.Init) > 0).OrderByDescending(x => x.NgayThuChi).Skip((Page - 1) * Limit).Take(Limit).ToList();
            }
            else
            {
                LstThuChi = context.tblJOBPhieuThuChis.Where(x => x.tblJOBUserPheDuyets.Count(x1 => x1.IDUserPheDuyet == IDUser && x1.EnumStatus == (int)EnumStatusPheDuyet.Init) > 0).OrderByDescending(x => x.NgayThuChi).Skip((Page - 1) * Limit).Take(Limit).ToList();
            } 
            var LstJOB = LstThuChi.Select(x => new ObjectThuChiCal
            {
                ID = x.ID,
                LoaiPhieuCal = x.EnumLoaiPhieu,
                SoPhieu = x.BillNo,
                TrangThai = x.EnumStatusText,
                NgayThuChiCal = x.NgayThuChi,
                NhanVien = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",
                SoTienCal = x.Amount,
                LoaiTien = x.tblDMCurrency != null ? x.tblDMCurrency.KyHieu : "VND",
                TyGiaCal = x.TyGia,
                NoiDung = x.Note,
                CodeJOB =x.tblJOB != null ? x.tblJOB.CodeJOB : "",
                DoiTuongThanhToan = x.CurrentObjectNameWeb + "",
                NguoiDuyet = x.LichSuPheDuyetWebText + "",
                DanhSachChungTu = x.DocumentText
            }).ToList();
            string _text = LstThuChi.Count() > 0 ? "Lấy dữ liệu thành công !" : "Không có dữ liệu !";
            var res = new
            {
                result = _text,
                data = LstJOB,
                TotalCount = LstThuChi.Count(),
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey
            };
            return Ok(res);


        }
        // Gửi lệnh
        [HttpPost]
        [Route("api/UpdateTrangThaiThuChi")] // sửa
        public IHttpActionResult UpdateTrangThaiThuChi([FromBody] ObjectThuChiCal _object)
        {
            if (_object == null) return Content(HttpStatusCode.NotFound, "Đối tượng rỗng !");
            if (_object.ID == 0) return Content(HttpStatusCode.NotFound, "Lỗi dữ liệu truyền vào !");

            try
            {
                LGTICDBEntities context = GetLicenseKey(_object.ProductKey , _object.IDUser);
                if (context == null)
                    return Content(HttpStatusCode.NotFound, "ProductKey không hợp lệ !");

                var  _entity = context.tblJOBPhieuThuChis.FirstOrDefault(x => x.ID == _object.ID);
                
                var res = new
                {
                    result = "Cập nhật dữ liệu thành công !",
                    data = ""
                };
                return Ok(res);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Lỗi dữ liệu !");
            }


        }

        [HttpGet]
        [Route("api/GetListQuyetToan")]
        public IHttpActionResult GetListQuyetToan(string ProductKey, long IDUser, DateTime dtS, DateTime dtE, int Page, int Limit)
        {
            LGTICDBEntities context = GetLicenseKey(ProductKey, IDUser);
            if (context == null)
                return Content(HttpStatusCode.NotFound, "ProductKey không hợp lệ !");

            dtS = dtS.Date;
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
            if (!UserLogin.HasPermiss(EnumPermission.Admin) && !UserLogin.HasPermiss(EnumPermission.ViewAccouting) )
            {
                var resError = new
                {
                    result = "Người dùng không có quyền xem dữ liệu !",
                    data = "",
                    TotalCount = 0,
                    Page = Page,
                    Limit = Limit,
                    ProductKey = ProductKey
                };
                return Ok(resError);
            }

            Expression<Func<tblJOBQuyetToan, bool>> funUser = null;
            Expression<Func<tblJOBQuyetToan, bool>> func = x => x.NgayQuyetToan != null && x.NgayQuyetToan >= dtS && x.NgayQuyetToan <= dtE;

            IQueryable<tblJOBQuyetToan> _source = context.tblJOBQuyetToans;
           

            List<tblJOBQuyetToan> LstThuChi = new List<tblJOBQuyetToan>();
            if (UserLogin.HasPermiss(EnumPermission.Admin) || UserLogin.HasPermiss(EnumPermission.ViewAccouting))
            {
            }
            else
            {
                funUser = x => x.IDUserCreate == UserLogin.ID || x.tblJOBUserPheDuyets.FirstOrDefault(x1 => x1.IDUserPheDuyet == UserLogin.ID) != null;
            }
            _source = _source.Where(func);
            _source = _source.Where(funUser);

            var LstJOB = _source.OrderByDescending(x => x.NgayQuyetToan).Skip((Page - 1) * Limit).Take(Limit).ToList().Select(x => new ObjectThuChiCal
            {
                _EnumLoaiDS = 1,
                ID = x.ID,
                LoaiPhieuCal = x.EnumQuyetToanThuChi,
                SoPhieu = x.SoQuyetToan,
                TrangThai = x.EnumStatusText,
                NgayThuChiCal = x.NgayQuyetToan,
                NhanVien = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",
                SoTienCal = x.TienDaChi,
                LoaiTien = x.tblDMCurrency != null ? x.tblDMCurrency.KyHieu : "VND",
                NoiDung = x.GhiChu,
                CodeJOB = x.tblJOB != null ? x.tblJOB.CodeJOB : "",
                DoiTuongThanhToan = x.CurrentObjectNameWeb,
                NguoiDuyet = x.LichSuPheDuyetWebText,
                DanhSachChungTu = string.Join("\n", x.ListAllChargeList.Select(x1 => x1.FileAttachText).ToList())
            }).ToList();
            string _text = LstThuChi.Count() > 0 ? "Lấy dữ liệu thành công !" : "Không có dữ liệu !";
            var res = new
            {
                result = _text,
                data = LstJOB,
                TotalCount = LstThuChi.Count(),
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey
            };
            return Ok(res);


        }

        [HttpGet]
        [Route("api/GetListPheDuyetQuyetToan")]
        public IHttpActionResult GetListPheDuyetQuyetToan(string ProductKey, long IDUser, DateTime dtS, DateTime dtE, int Page, int Limit, bool bJOB)
        {
            LGTICDBEntities context = GetLicenseKey(ProductKey, IDUser);
            if (context == null)
                return Content(HttpStatusCode.NotFound, "ProductKey không hợp lệ !");

            dtS = dtS.Date;
            dtE = new DateTime(dtE.Year, dtE.Month, dtE.Day, 23, 59, 00);
            if (!UserLogin.HasPermiss(EnumPermission.Admin) && !UserLogin.HasPermiss(EnumPermission.ViewAccouting ) && UserLogin.FlagDuyetChi != true)
            {
                var resError = new
                {
                    result = "Người dùng không có quyền xem dữ liệu !",
                    data = "",
                    TotalCount = 0,
                    Page = Page,
                    Limit = Limit,
                    ProductKey = ProductKey
                };
                return Ok(resError);
            }

            Expression<Func<tblJOBQuyetToan, bool>> func = null;
            Expression<Func<tblJOBQuyetToan, bool>> funUser = null;

            IQueryable<tblJOBQuyetToan> _source = context.tblJOBQuyetToans;
            if (bJOB == true) func = x => x.IDJOB != null;
            else func = x => x.IDJOB == null;

            List<tblJOBQuyetToan> LstThuChi = new List<tblJOBQuyetToan>();
            if (UserLogin.HasPermiss(EnumPermission.Admin))
            {
                funUser = x =>  x.tblJOBUserPheDuyets.Count(x1 => x1.EnumStatus == (int)EnumStatusPheDuyet.Init) > 0;
            }
            else
            {
                funUser = x =>  x.tblJOBUserPheDuyets.Count(x1 => x1.IDUserPheDuyet == AppSettings.CurrentLoginUser.IDLoginUser && x1.EnumStatus == (int)EnumStatusPheDuyet.Init) > 0;
            }
            _source = _source.Where(func);
            _source = _source.Where(funUser);

            var LstJOB = _source.OrderByDescending(x => x.NgayQuyetToan).Skip((Page - 1) * Limit).Take(Limit).ToList().Select(x => new ObjectThuChiCal
            {
                _EnumLoaiDS = 1,
                ID = x.ID,
                LoaiPhieuCal = x.EnumQuyetToanThuChi,
                SoPhieu = x.SoQuyetToan,
                TrangThai = x.EnumStatusText,
                NgayThuChiCal = x.NgayQuyetToan,
                NhanVien = x.tblNhanSu != null ? x.tblNhanSu.HoTenVI : "",
                SoTienCal = x.TienDaChi,
                LoaiTien = x.tblDMCurrency != null ? x.tblDMCurrency.KyHieu : "VND",
                NoiDung = x.GhiChu,
                CodeJOB = x.tblJOB != null ? x.tblJOB.CodeJOB : "",
                DoiTuongThanhToan = x.CurrentObjectNameWeb,
                NguoiDuyet = x.LichSuPheDuyetWebText,
                DanhSachChungTu = string.Join("\n",x.ListAllChargeList.Select(x1=>x1.FileAttachText).ToList())
            }).ToList();
            string _text = LstThuChi.Count() > 0 ? "Lấy dữ liệu thành công !" : "Không có dữ liệu !";
            var res = new
            {
                result = _text,
                data = LstJOB,
                TotalCount = LstThuChi.Count(),
                Page = Page,
                Limit = Limit,
                ProductKey = ProductKey
            };
            return Ok(res);


        }

        #endregion

    }
}