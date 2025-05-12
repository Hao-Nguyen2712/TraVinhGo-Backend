using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TraVinhMaps.Application.Features.Users.Models;

public class UploadImageRequest
{
    public string id { get; set; }
    public IFormFile imageFile { get; set; }
}
