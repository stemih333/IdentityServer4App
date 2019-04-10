using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace CleanTasks.CommonWeb.Classes
{
    public class JsonKeyArrayClaimAction : ClaimAction
    {
        /// <summary>
        /// Creates a new JsonKeyArrayClaimAction.
        /// </summary>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        /// <param name="jsonKey">The top level key to look for in the json user data.</param>
        public JsonKeyArrayClaimAction(string claimType, string valueType, string jsonKey) : base(claimType, valueType)
        {
            JsonKey = jsonKey;
        }

        /// <summary>
        /// The top level key to look for in the json user data.
        /// </summary>
        public string JsonKey { get; }

        public override void Run(JObject userData, ClaimsIdentity identity, string issuer)
        {
            var values = userData?[JsonKey];
            if (values == null) return;
                 
            if (!(values is JArray)) {
                identity.AddClaim(new Claim(ClaimType, values.ToString(), ValueType, issuer));
                return;
            }

            foreach (var value in values)
            {
                identity.AddClaim(new Claim(ClaimType, value.ToString(), ValueType, issuer));
            }
        }
    }
}
