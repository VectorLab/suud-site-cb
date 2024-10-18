using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace suud_site_cb.Utils;

public class SsoSuudClient
{

    public readonly HttpClient upstream_server;

    public readonly RSA KeyPri;// base64url
    public readonly string KeyPwd;// base64url treat as raw
    public readonly string Id;// hex treat as ObjectId

    public SsoSuudClient(IOptions<SsoSuudClientSettings> p1)
    {
        {
            var v1 = RSA.Create();
            v1.ImportPkcs8PrivateKey(WebEncoders.Base64UrlDecode(p1.Value.KeyPri), out int v2);
            KeyPri = v1;
        }

        upstream_server = new()
        {
            BaseAddress = new Uri("https://suud.net"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        KeyPwd = p1.Value.KeyPwd;
        Id = p1.Value.Id;
    }

    public async Task<SsoSuudResponse?> HandleRequest(string p1)
    {
        if (p1.Length < 0)
        {
            return null;
        }

        string? v1 = null;
        try
        {
            var v3 = new JsonObject
            {
                ["t"] = p1,// token
                ["k"] = KeyPwd,// password
                ["p"] = new JsonArray([1, 2, 3])// permission
            };// for more detailed permission info, access suud.net documents for more details

            var v2 = await upstream_server.PostAsync("/api/public/sso/loginverify",
            new StringContent(
            JsonSerializer.Serialize(v3),
            Encoding.UTF8,
            "application/json"
            )
            );

            v2.EnsureSuccessStatusCode();
            v1 = await v2.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("SsoSuud Request error: " + e.Message);
            return null;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("SsoSuud Request timed out.");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("SsoSuud Unexpected error: " + e.Message);
            return null;
        }
        if (null == v1)
        {
            return null;
        }

        JsonObject? v4 = null;
        try
        {
            v4 = JsonSerializer.Deserialize<JsonObject>(v1);
        }
        catch (Exception e)
        {
            Console.WriteLine("SsoSuud response parsing error: " + e.Message);
            return null;
        }

        if (null == v4)
        {
            return null;
        }

        SsoSuudResponse v5 = new();

        {
            long? v6 = JsonUtil.GetPropertyTVal<long>(v4, "t");
            if (!v6.HasValue)
            {
                return null;
            }
            v5.ResponseTime = DateTimeOffset.FromUnixTimeMilliseconds(v6.Value).UtcDateTime;
        }

        var ve = JsonUtil.GetPropertyTRef<JsonObject>(v4, "v");
        if (null == ve)
        {
            return null;
        }

        {
            ve.TryGetPropertyValue("p", out JsonNode? v6);
            if (v6 is JsonArray v7)
            {// parse permission access result
                foreach (JsonNode? v8 in v7)
                {
                    if (v8 is JsonObject v9)
                    {
                        int? va = JsonUtil.GetPropertyTVal<int>(v9, "p");
                        if (!va.HasValue)
                        {
                            continue;
                        }
                        var vb = JsonUtil.GetPropertyTRef<string>(v9, "r");
                        if (null == vb)
                        {
                            continue;
                        }
                        switch (va.Value)
                        {
                            case 1:
                                if (ObjectId.TryParse(vb, out ObjectId vc))
                                {
                                    v5.UserId = vc;
                                }
                                break;
                            case 2:
                                v5.UserName = vb;
                                break;
                            case 3:
                                v5.Avatar = vb;
                                break;
                        }
                    }
                }
            }
        }

        if (ObjectId.Empty == v5.UserId || null == v5.UserName || "" == v5.UserName)
        {
            return null;
        }
        return v5;
    }

};

public class SsoSuudClientSettings
{
    public virtual string Id { get; set; } = null!;
    public virtual string KeyPri { get; set; } = null!;
    public virtual string KeyPwd { get; set; } = null!;
};

public class SsoSuudResponse
{
    public DateTime ResponseTime;

    public ObjectId UserId;
    public string UserName = "";
    public string Avatar = "";
};


