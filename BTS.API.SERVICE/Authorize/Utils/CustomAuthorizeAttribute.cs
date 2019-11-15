using System;
using System.Data;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Security.Claims;
using System.Linq;
using BTS.API.SERVICE.Helper;
using System.Web.Http;

namespace BTS.API.SERVICE.Authorize.Utils
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string State { get; set; }
        public string Method { get; set; }
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }
        }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string _unitCode = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var unit = currentUser.Claims.FirstOrDefault(x => x.Type == "unitCode");
                if (unit != null)
                    _unitCode = unit.Value;
            }
            string username = actionContext.RequestContext.Principal.Identity.Name;
            var authorize = base.IsAuthorized(actionContext);
            if (username.Equals("admin") && authorize) return true;
            bool check = false;
            RoleState roleState = Get(_unitCode, username, State);
            if (!string.IsNullOrEmpty(roleState?.STATE))
            {
                switch (Method)
                {
                    case "XEM":
                        if (roleState.View) check = true;
                        break;
                    case "SUA":
                        if (roleState.Edit) check = true;
                        break;
                    case "THEM":
                        if (roleState.Add) check = true;
                        break;
                    case "XOA":
                        if (roleState.Delete) check = true;
                        break;
                    case "DUYET":
                        if (roleState.Approve) check = true;
                        break;
                }
            }
            if (!authorize || !check) return false;
            return true;

        }

        private RoleState Get(string unitCode, string username, string machucnang)
        {
            RoleState roleState = new RoleState();
            if (username.Equals("admin"))
            {
                roleState = new RoleState()
                {
                    Approve = true,
                    Delete = true,
                    Add = true,
                    STATE = "all",
                    Edit = true,
                    View = true
                };
            }
            else
            {
                var cacheData = MemoryCacheHelper.GetValue(unitCode + "|" + machucnang + "|" + username);
                if (cacheData == null)
                {
                    using (var connection = new OracleConnection(ConfigurationManager.ConnectionStrings["KNQ.Connection"].ConnectionString))
                    {
                        connection.Open();
                        using (OracleCommand command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText =
                                @"SELECT XEM,THEM,SUA,XOA,DUYET FROM AU_NHOMQUYEN_CHUCNANG WHERE UNITCODE='" + unitCode + "' AND MACHUCNANG='" + machucnang +
                                "' AND MANHOMQUYEN IN (SELECT MANHOMQUYEN FROM AU_NGUOIDUNG_NHOMQUYEN WHERE UNITCODE='" + unitCode + "' AND USERNAME='" +
                                username + "') UNION SELECT AU_NGUOIDUNG_QUYEN.XEM,AU_NGUOIDUNG_QUYEN.THEM,AU_NGUOIDUNG_QUYEN.SUA,AU_NGUOIDUNG_QUYEN.XOA,AU_NGUOIDUNG_QUYEN.DUYET " +
                                "FROM AU_NGUOIDUNG_QUYEN WHERE AU_NGUOIDUNG_QUYEN.UNITCODE='" + unitCode + "' AND AU_NGUOIDUNG_QUYEN.MACHUCNANG='" + machucnang + "' AND AU_NGUOIDUNG_QUYEN.USERNAME='" + username + "'";
                            using (OracleDataReader oracleDataReader = command.ExecuteReader())
                            {
                                if (!oracleDataReader.HasRows)
                                {
                                    roleState = new RoleState()
                                    {
                                        STATE = string.Empty,
                                        View = false,
                                        Approve = false,
                                        Delete = false,
                                        Add = false,
                                        Edit = false
                                    };
                                }
                                else
                                {
                                    roleState.STATE = machucnang;
                                    while (oracleDataReader.Read())
                                    {
                                        int objXem = Int32.Parse(oracleDataReader["XEM"].ToString());
                                        if (objXem == 1)
                                        {
                                            roleState.View = true;
                                        }
                                        int objThem = Int32.Parse(oracleDataReader["THEM"].ToString());
                                        if (objThem == 1)
                                        {
                                            roleState.Add = true;
                                        }
                                        int objSua = Int32.Parse(oracleDataReader["SUA"].ToString());
                                        if (objSua == 1)
                                        {
                                            roleState.Edit = true;
                                        }
                                        int objXoa = Int32.Parse(oracleDataReader["XOA"].ToString());
                                        if (objXoa == 1)
                                        {
                                            roleState.Delete = true;
                                        }
                                        int objDuyet = Int32.Parse(oracleDataReader["DUYET"].ToString());
                                        if (objDuyet == 1)
                                        {
                                            roleState.Approve = true;
                                        }
                                    }
                                    MemoryCacheHelper.Add(unitCode + "|" + machucnang + "|" + username, roleState,
                                        DateTimeOffset.Now.AddHours(6));
                                }
                            }
                        }
                    }
                }
                else
                {
                    roleState = (RoleState)cacheData;

                }
            }
            return roleState;
        }
    }
}