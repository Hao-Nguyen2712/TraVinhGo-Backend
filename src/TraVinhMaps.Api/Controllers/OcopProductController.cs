// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.OcopProduct;
using TraVinhMaps.Application.Features.OcopProduct.Interface;
using TraVinhMaps.Application.Features.OcopProduct.Mappers;
using TraVinhMaps.Application.Features.OcopProduct.Models;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OcopProductController : ControllerBase
{
    private readonly IOcopProductService _service;
    private readonly ImageManagementOcopProductServices _imageManagementOcopProductServices;
    public OcopProductController(IOcopProductService service, ImageManagementOcopProductServices imageManagementOcopProductServices)
    {
        _service = service;
        _imageManagementOcopProductServices = imageManagementOcopProductServices;
    }
    [HttpGet]
    [Route("GetAllOcopProductPagination")]
    public async Task<Pagination<OcopProduct>> GetAllOcopProductPagination([FromQuery] OcopProductSpecParams OcopProductSpecParams)
    {
        var getAllOcopProductPagination = await _service.GetAllOcopProductAsync(OcopProductSpecParams);
        return getAllOcopProductPagination;
    }
    [HttpGet]
    [Route("GetAllOcopProductActive")]
    public async Task<IActionResult> GetAllOcopProductActive()
    {
        var getAllOcopProductActive = await _service.ListAsync(ocop => ocop.Status == true);
        return this.ApiOk(getAllOcopProductActive);
    }

    [HttpGet]
    [Route("GetAllOcopProduct")]
    public async Task<IActionResult> GetAllOcopProduct()
    {
        var listOcopProduct = await _service.ListAllAsync();
        return this.ApiOk(listOcopProduct);
    }
    [HttpGet]
    [Route("GetOcopProductById/{id}", Name = "GetOcopProductById")]
    public async Task<IActionResult> GetOcopProductById(string id)
    {
        var ocopProduct = await _service.GetByIdAsync(id);
        return this.ApiOk(ocopProduct);
    }
    [HttpGet]
    [Route("GetOcopProductByOcopTypeId/{id}")]
    public async Task<IActionResult> GetOcopProductByOcopTypeId(string id)
    {
        var listOcopProduct = await _service.GetOcopProductByOcopTypeId(id);
        return this.ApiOk(listOcopProduct);
    }
    [HttpGet]
    [Route("GetOcopProductByCompanyId/{id}")]
    public async Task<IActionResult> GetOcopProductByCompanyId(string id)
    {
        var listOcopProduct = await _service.GetOcopProductByCompanyId(id);
        return this.ApiOk(listOcopProduct);
    }
    [HttpGet]
    [Route("CountOcopProducts")]
    public async Task<IActionResult> CountOcopProducts()
    {
        var countOcopProducts = await _service.CountAsync();
        return this.ApiOk(countOcopProducts);
    }
    [HttpPost]
    [Route("AddOcopProduct")]
    public async Task<IActionResult> AddOcopProduct([FromForm] CreateOcopProductRequest createOcopProductRequest)
    {
        if (createOcopProductRequest.ProductImageFile == null || createOcopProductRequest.ProductImageFile.Count == 0)
        {
            return this.ApiError("Ocop product attractions must have at least 1 photo");
        }
        var imageFile = await _imageManagementOcopProductServices.AddImageOcopProduct(createOcopProductRequest.ProductImageFile);
        if (imageFile == null) { throw new NotFoundException("No valid image uploaded."); }
        var createOcopProduct = OcopProductMapper.Mapper.Map<OcopProduct>(createOcopProductRequest);
        var ocopProducts = await _service.AddAsync(createOcopProduct);
        ocopProducts.Status = true;
        foreach (var item in imageFile)
        {
            await this._service.AddImageOcopProduct(ocopProducts.Id, item);
        }
        return CreatedAtRoute("GetOcopProductById", new { id = ocopProducts.Id }, this.ApiOk(ocopProducts));
    }
    [HttpPost]
    [Route("AddImageOcopProduct")]
    public async Task<IActionResult> AddImageOcopProduct([FromForm] Application.Features.OcopProduct.Models.AddImageRequest addImageRequest)
    {
        var linkImage = await _imageManagementOcopProductServices.AddImageOcopProduct(addImageRequest.imageFile);
        if (linkImage == null)
        {
            return this.ApiError("Ocop product attractions must have at least 1 photo.");
        }
        foreach (var item in linkImage)
        {
            await this._service.AddImageOcopProduct(addImageRequest.id, item);
        }
        return this.ApiOk(linkImage);
    }
    [HttpPut]
    [Route("UpdateOcopProduct")]
    public async Task<IActionResult> UpdateOcopProduct([FromBody] UpdateOcopProductRequest updateOcopProductRequest)
    {
        var updateOcopProduct = OcopProductMapper.Mapper.Map<OcopProduct>(updateOcopProductRequest);
        await _service.UpdateAsync(updateOcopProduct);
        return this.ApiOk("Updated ocop product Successfully.");
    }
    [HttpDelete]
    [Route("DeleteOcopProduct/{id}")]
    public async Task<IActionResult> DeleteOcopProduct(string id)
    {
        var ocopProduct = await _service.GetByIdAsync(id);
        if (ocopProduct == null) { 
            throw new NotFoundException("Ocop product not found.");
            }
        await _service.DeleteOcopProductAsync(id);
        return this.ApiOk("Ocop product deleted successfully.");
    }

    [HttpPut]
    [Route("RestoreOcopProduct/{id}")]
    public async Task<IActionResult> RestoreOcopProduct(string id)
    {
        var restoreOcopProduct = await _service.GetByIdAsync(id);
        if (restoreOcopProduct == null)
        {
            throw new NotFoundException("Ocop product not found.");
        }
        await _service.RestoreOcopProductAsync(id);
        return this.ApiOk("Ocop product restored successfully.");
    }
    [HttpPost]
    [Route("AddSellLocation")]
    public async Task<IActionResult> AddSellLocation(string id, [FromBody] SellLocation sellLocation)
    {
        var ocopProduct = await _service.GetByIdAsync(id);
        if (ocopProduct == null)
        {
            throw new NotFoundException("Ocop product not found.");
        }
        var addSellLocation = await _service.AddSellLocation(id, sellLocation);
        return this.ApiOk(addSellLocation);
    }
    [HttpDelete]
    [Route("DeleteSellLocation")]
    public async Task<IActionResult> DeleteSellLocation(string id, string name)
    {
        var ocopProduct = await _service.GetByIdAsync(id);
        if (ocopProduct == null)
        {
            throw new NotFoundException("Ocop product not found.");
        }
        if(!ocopProduct.Sellocations.Any(n => n.LocationName == name))
        {
            throw new NotFoundException("Sell location not found.");
        }

        var deleteSellLocation = await _service.DeleteSellLocation(id, name);
        return this.ApiOk("Sell location deleted successfully.");
    }
}
