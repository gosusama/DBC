using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.AuNguoiDung;
using BTS.API.SERVICE.Helper;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Oracle.ManagedDataAccess.Client;

namespace BTS.SP.API.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            int level = 0;
            try
            {
                var user = new AU_NGUOIDUNG();
                using (var connection = new OracleConnection(ConfigurationManager.ConnectionStrings["KNQ.Connection"].ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            "SELECT * FROM AU_NGUOIDUNG WHERE USERNAME='" + context.UserName + "' AND PASSWORD='" + MD5Encrypt.MD5Hash(context.Password) + "' AND TRANGTHAI = 10 ";
                        using (var oracleDataReader = command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            if (!oracleDataReader.Result.HasRows)
                            {
                                user = null;
                            }
                            else
                            {
                                while (oracleDataReader.Result.Read())
                                {
                                    user.Username = oracleDataReader.Result["USERNAME"]?.ToString();
                                    user.TenNhanVien = oracleDataReader.Result["TENNHANVIEN"]?.ToString();
                                    user.SoDienThoai = oracleDataReader.Result["SODIENTHOAI"]?.ToString();
                                    user.ChungMinhThu = oracleDataReader.Result["SOCHUNGMINHTHU"]?.ToString();
                                    user.UnitCode = oracleDataReader.Result["UNITCODE"]?.ToString();
                                    user.ParentUnitcode = oracleDataReader.Result["PARENT_UNITCODE"]?.ToString();
                                    int.TryParse(oracleDataReader.Result["LEVEL"]?.ToString(), out level);
                                    user.Level = level;
                                }
                            }
                        }
                    }
                }
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                Action<ClaimsIdentity, string> addClaim = (ClaimsIdentity obj, string username) => { return; };
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                addClaim.Invoke(identity, user.Username);
                identity.AddClaim(new Claim(ClaimTypes.Role, "MEMBER"));
                identity.AddClaim(new Claim("unitCode", user.UnitCode));
                identity.AddClaim(new Claim("parentUnitCode", user.ParentUnitcode));
                AuthenticationProperties properties = new AuthenticationProperties(new Dictionary<string, string>
                    {
                    {
                        "userName", string.IsNullOrEmpty(user.Username)?string.Empty:user.Username
                    },
                    {
                        "fullName", string.IsNullOrEmpty(user.TenNhanVien)?string.Empty:user.TenNhanVien
                    },
                    {
                        "code", string.IsNullOrEmpty(user.MaNhanVien)?string.Empty:user.MaNhanVien
                    },
                    {
                        "phone", string.IsNullOrEmpty(user.SoDienThoai)?string.Empty:user.SoDienThoai
                    },
                    {
                        "chungMinhThu", string.IsNullOrEmpty(user.ChungMinhThu)?string.Empty:user.ChungMinhThu
                    },
                    {
                        "unitCode", string.IsNullOrEmpty(user.UnitCode)?string.Empty:user.UnitCode
                    },
                    {
                        "parentUnitCode", string.IsNullOrEmpty(user.ParentUnitcode)?string.Empty:user.ParentUnitcode
                    },
                    {
                        "level", level.ToString()
                    }
                    });

                AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(identity);
            }
            catch (Exception e)
            {
                context.SetError("invalid_grant", e.Message);
                return;
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

    }
}