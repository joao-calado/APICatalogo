﻿using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/teste")]
[ApiController]
[ApiVersion(3)]
[ApiVersion(4)]
public class TesteV3Controller : ControllerBase
{
    [MapToApiVersion(3)]
    [HttpGet]
    public string GetVersion3()
    {
        return "Version3 - GET - Api Versão 3.0";
    }

    [MapToApiVersion(4)]
    [HttpGet]
    public string GerVersion4()
    {
        return "Version4 - GET - Api Versão 4.0";
    }
}
