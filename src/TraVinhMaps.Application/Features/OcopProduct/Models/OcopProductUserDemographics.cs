// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.OcopProduct.Models;
public class OcopProductUserDemographics
{
    public string Id { get; set; } // ID sản phẩm OCOP
    public string ProductName { get; set; } // Tên sản phẩm
    public string AgeGroup { get; set; } // Nhóm tuổi (<18, 18-25, 26-35, 36-50, >50)
    public string Hometown { get; set; } // Quê quán (chuẩn hóa, ví dụ: "TP. Cần Thơ")
    public long UserCount { get; set; } // Số lượng người dùng
}
