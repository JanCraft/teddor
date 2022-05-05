using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace jDevES.FirebaseIDToken {
    public class IDToken {
        public readonly string idToken, projectId;
        public readonly JwtSecurityToken jwt;

        public IDToken(string idToken, string projectId) {
            this.idToken = idToken;
            this.projectId = projectId;

            jwt = new JwtSecurityToken(idToken);
        }

        public bool IsExpired {
            get {
                return DateTime.UtcNow >= jwt.Payload.ValidTo;
            }
        }

        public string UserID {
            get {
                return jwt.Payload.Sub;
            }
        }

        public int? AuthTime {
            get {
                return jwt.Payload.AuthTime;
            }
        }

        public async Task<bool> ValidateIDToken() {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = await GooglePublicKey.GetTokenValidationParameters(jwt.Header.Kid, projectId);
            if (validationParameters == null) return false;

            try {
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(idToken, validationParameters, out validatedToken);
            } catch (Exception e) {
                UnityEngine.Debug.LogException(e);
                return false;
            }
            return true;
        }
    }

    public class GooglePublicKey {
        public static string URL = "https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com";
        private static readonly HttpClient client = new HttpClient();
        private static TokenValidationParameters tokparams = null;
        private static string tokparamsid = null, tokparamsprojid = null;

        public static async Task<TokenValidationParameters> GetTokenValidationParameters(string id, string projectId) {
            if (tokparams != null && id.Equals(tokparamsid) && projectId.Equals(tokparamsprojid)) {
                return tokparams;
            }

            HttpResponseMessage response = await client.GetAsync(URL);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
            if (!dict.ContainsKey(id)) return null;
            string certkey = dict[id];

            certkey = certkey.Replace("\\n", "\n");
            certkey = certkey.Replace("-----BEGIN CERTIFICATE-----\n", null);
            certkey = certkey.Replace("-----END CERTIFICATE-----", null);

            var seccert = new X509Certificate2(Convert.FromBase64String(certkey));
            var seckey = new X509SecurityKey(seccert);
            seckey.KeyId = id;

            tokparamsid = id;
            tokparamsprojid = projectId;
            tokparams = new TokenValidationParameters() {
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidAudience = projectId,
                ValidIssuer = "https://securetoken.google.com/" + projectId,
                IssuerSigningKey = seckey
            };

            return tokparams;
        }
    }
}