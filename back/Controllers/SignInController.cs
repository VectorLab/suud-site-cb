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
        var req_json = await this.sso.HandleRequest(req_data);
        if (null == req_json)
        {
            return BadRequest();
        }
        var res_json = new JsonObject
        {
            ["n"] = req_json.UserName,
            ["a"] = req_json.Avatar
        };
        return new JsonResult(res_json) { ContentType = "application/json" };
    }

}
