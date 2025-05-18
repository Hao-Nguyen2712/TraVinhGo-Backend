// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Features.OcopProduct.Interface;
using TraVinhMaps.Application.Features.OcopProduct.Mappers;
using TraVinhMaps.Application.Features.OcopProduct.Models;
using TraVinhMaps.Application.Features.OcopProduct;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;

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
        var getAllOcopProductActive = await _service.GetAllOcopProductActiveAsync();
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
    [Route("GetByIdOcopProduct/{id}", Name = "GetByIdOcopProduct")]
    public async Task<IActionResult> GetByIdOcopProduct(string id)
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
    [Route("CreateOcopProduct")]
    public async Task<IActionResult> CreateOcopProduct([FromForm] CreateOcopProductRequest createOcopProductRequest)
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
        return CreatedAtRoute("GetByIdOcopProduct", new { id = ocopProducts.Id }, ocopProducts);
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
        return this.ApiOk("Deleted ocop product successfully.");
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
        return this.ApiOk("Restore ocop product successfully.");
    }

}
