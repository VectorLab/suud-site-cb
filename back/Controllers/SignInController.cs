using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using suud_site_cb.Utils;

namespace suud_site_cb.Controllers;

[ApiController]
[Route("/signin")]
public class SignInController(ILogger<SignInController> p1, SsoSuudClient p2) : ControllerBase
{

    private readonly ILogger<SignInController> log = p1;
    private readonly SsoSuudClient sso = p2;

    public async Task<IActionResult> onPost([FromBody] string req_data)
    {
        // client uploads one-time auth key 
        var v1 = await this.sso.HandleRequest(req_data);
        if (null == v1)
        {
            return BadRequest();
        }
        var v2 = new JsonObject
        {
            ["n"] = v1.UserName,
            ["a"] = v1.Avatar
        };
        return new JsonResult(v2) { ContentType = "application/json" };
    }

}
