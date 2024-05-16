using API_AppCMC;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace LOCYWebAPI.Controllers
{

    public enum EnumObjectType
    {
        Customer = 1,
        Shipper = 2,
        Consigne = 3,
        Ailine = 4,
        NhaCC = 5,
        Port = 6,
        NhanVien =7
    }
    public class DMDoiTuong
    {
        public string EnumType { get; set; } // 1 : Customer , 2: Shipper, 3:Consigne,4:Airline,5:NhaCC, 6: Cảng , 7:Nhân viên
        public string TaxCode { get; set; }
        public string Code { get; set; }
        public string NameVI { get; set; }
        public string NameEN { get; set; }
        public string AddressVI { get; set; }
        public string AddressEN { get; set; }
        public string SoCMT { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string IATACode { get; set; }
    }

    public class tblDMCustomersController : ApiController
    {
        enum ErrorCode
        {
            Insert_Successfully = 201,
            Update_Successfully = 204,
            Code_Exits = 409,
            MST_Exists= 400,
            Not_Found = 404,
            Bad_Request = 504,
            Object_Null = -100,
            Save_Error = -200,
            Type_Bad = -300,
            Code_Empty = -400,

        }

        private Model_CMCBacNinhEntities db = new Model_CMCBacNinhEntities(ConnectionTools.BuildConnectionString());

        private void UpdateDataCustomer(tblDieuPhoiVanChuyen _ob, DMDoiTuong _object, bool bNew)
        {
           // _ob.Code = _object.Code;
           // _ob.NameVI = _object.NameVI;
           // _ob.NameEN = _object.NameEN;
           // _ob.AddressEN = _object.AddressEN;
           // _ob.AddressVI = _object.AddressVI;
           // _ob.TaxCode = _object.TaxCode;
           // _ob.Phone = _object.SoDienThoai;
           // _ob.Email = _object.Email;
           // _ob.IATACode = _object.IATACode;
           //if(bNew == true)
           // {
           //     _ob.FlagCRM = true;
           //     _ob.FlagActive = true;
           //     _ob.DateCreate = DateTime.Now;
           // }    

        }
       
        //===================================================================API=====================================================
        // GET: api/tblDMCustomers
        [HttpGet]
        [Route("api/GetListDieuPhoiVanChuyen")]
        public List<tblDieuPhoiVanChuyen> GetListDieuPhoiVanChuyen(DateTime  dtS , DateTime dtE)
        {
            return db.tblDieuPhoiVanChuyens.Where(x=>x.NgayDongHang == null ).ToList();
        }


        // PUT: api/tblDMCustomers/5
        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("api/EditCustomer")]
        public IHttpActionResult EditCustomer([FromBody] DMDoiTuong _object)
        {
            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.PreconditionFailed, "Lỗi kiểu dữ liệu đầu vào");
            }
            if (_object == null) return Content(HttpStatusCode.LengthRequired, "Không được để trống ĐỐI TƯỢNG");


            string _code = _object.Code?.Trim();
            if ((_code + "").Length == 0) return Content(HttpStatusCode.LengthRequired, "Không được để trống MÃ ĐỐI TƯỢNG");


            if ((_object.EnumType + "").Length == 0) return Content(HttpStatusCode.LengthRequired, "Không được để trống LOẠI ĐỐI TƯỢNG");

            try
            {
                string[] _type = null;


                if (_object.EnumType.Contains(",")) _type = _object.EnumType.Split(',');
                else if (_object.EnumType.Contains(";")) _type = _object.EnumType.Split(';');
                else
                {
                    int _t = int.Parse(_object.EnumType);
                    if (_t != 1 && _t != 2 && _t != 3 && _t != 4 && _t != 5 && _t != 6 && _t != 7)
                    {
                        return Content(HttpStatusCode.BadRequest, "LOẠI ĐỐI TƯỢNG không hợp lệ !");
                    }
                    else
                    {
                        _type = new string[] { _t.ToString() };
                    }
                }

                if (_type == null || _type?.Count() == 0) return Content(HttpStatusCode.BadRequest, "LOẠI ĐỐI TƯỢNG không hợp lệ !");

                if (_type.Where(x => int.Parse(x) == 7).Count() > 0)  // nhân viên
                {
                    tblNhanSu _ob = null;
                    if (_code?.Trim()?.Length > 0)
                    {
                        //_ob = db.tblNhanSus.Where(x => x.MANHANSU == _code).FirstOrDefault();
                        if (_ob == null) return Content(HttpStatusCode.NotFound, "MÃ ĐỐI TƯỢNG không tồn tại");
                    }
                   // UpdateDataNhanSu(_ob, _object);
                }
               
                else
                {
                    return Content(HttpStatusCode.BadRequest, "LOẠI ĐỐI TƯỢNG không hợp lệ !");
                }
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "LOẠI ĐỐI TƯỢNG không hợp lệ !");
            }
            
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Content(HttpStatusCode.InternalServerError, "Lưu dữ liệu bị lỗi");
            }

            return Content(HttpStatusCode.OK, "Cập nhật dữ liệu thành công !");
        }

       

        //=============================================================================================================================


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}