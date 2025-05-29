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
        createOcopProduct.Status = true;
        var ocopProducts = await _service.AddAsync(createOcopProduct);
        foreach (var item in imageFile)
        {
            await this._service.AddImageOcopProduct(ocopProducts.Id, item);
        }
        return CreatedAtRoute("GetOcopProductById", new { id = ocopProducts.Id }, this.ApiOk(ocopProducts));
    }
    [HttpPost]
    [Route("AddImageOcopProduct")]
    public async Task<IActionResult> AddImageOcopProduct([FromForm] AddImageRequest addImageRequest)
    {
        if(addImageRequest.imageFile == null && !addImageRequest.imageFile.Any())
        {
            return this.ApiError("Please upload at least one image.");
        }
        var linkImage = await _imageManagementOcopProductServices.AddImageOcopProduct(addImageRequest.imageFile);
        if (linkImage == null || !linkImage.Any())
        {
            return this.ApiError("Image upload failed.");
        }
        var ocopProduct = await _service.GetByIdAsync(addImageRequest.id);
        if (ocopProduct == null)
        {
            throw new NotFoundException("Ocop product not found.");
        }
        if(ocopProduct.ProductImage == null)
        {
            ocopProduct.ProductImage = new List<string>();
        }
        ocopProduct.ProductImage.AddRange(linkImage);
        await _service.UpdateAsync(ocopProduct);
        return this.ApiOk(ocopProduct.ProductImage);
    }
    [HttpDelete]
    [Route("DeleteImageOcopProduct/{id}/{imageUrl}")]
    public async Task<IActionResult> DeleteImageOcopProduct(string id, string imageUrl)
    {
        var decodedImageUrl = Uri.UnescapeDataString(imageUrl);
        var ocopProduct = await _service.GetByIdAsync(id);
        if (ocopProduct == null)
        {
            throw new NotFoundException("Ocop product not found.");
        }
        if(ocopProduct.ProductImage == null || !ocopProduct.ProductImage.Contains(decodedImageUrl)) {
            return this.ApiError("Image URL not found in product.");
        }
        ocopProduct.ProductImage.Remove(decodedImageUrl);
        await _service.UpdateAsync(ocopProduct);
        return this.ApiOk("Image of ocop product deleted successfully.");
    }
    [HttpPut]
    [Route("UpdateOcopProduct")]
    public async Task<IActionResult> UpdateOcopProduct([FromBody] UpdateOcopProductRequest updateOcopProductRequest)
    {
        var existingProduct = await _service.GetByIdAsync(updateOcopProductRequest.Id);
        if (existingProduct == null)
        {
            throw new NotFoundException("Ocop product not found.");
        }

        existingProduct.ProductName = updateOcopProductRequest.ProductName;
        existingProduct.ProductDescription = updateOcopProductRequest.ProductDescription;
        existingProduct.ProductPrice = updateOcopProductRequest.ProductPrice;
        existingProduct.OcopTypeId = updateOcopProductRequest.OcopTypeId;
        existingProduct.CompanyId = updateOcopProductRequest.CompanyId;
        existingProduct.OcopPoint = updateOcopProductRequest.OcopPoint;
        existingProduct.OcopYearRelease = updateOcopProductRequest.OcopYearRelease;
        existingProduct.TagId = updateOcopProductRequest.TagId;
        existingProduct.SellingLinkId = updateOcopProductRequest.SellingLinkId;

        if (updateOcopProductRequest.UpdateAt.HasValue)
        {
            existingProduct.UpdateAt = updateOcopProductRequest.UpdateAt.Value;
        }
        await _service.UpdateAsync(existingProduct);
        return this.ApiOk("Updated ocop product successfully.");
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
    public async Task<IActionResult> AddSellLocation([FromBody] CreateSellLocationRequest sellLocation)
    {
        var ocopProduct = await _service.GetByIdAsync(sellLocation.Id);
        if (ocopProduct == null)
        {
            throw new NotFoundException("Ocop product not found.");
        }
        var mapSellLocation = OcopProductMapper.Mapper.Map<SellLocation>(sellLocation);
        var addSellLocation = await _service.AddSellLocation(sellLocation.Id, mapSellLocation);
        return this.ApiOk(addSellLocation);
    }
    [HttpPut]
    [Route("UpdateSellLocation")]
    public async Task<IActionResult> UpdateSellLocation([FromBody] CreateSellLocationRequest sellLocation)
    {
        var ocopProduct = await _service.GetByIdAsync(sellLocation.Id);
        if (ocopProduct == null)
        {
            throw new NotFoundException("Ocop product not found.");
        }
        var mapSellLocation = OcopProductMapper.Mapper.Map<SellLocation>(sellLocation);
        var addSellLocation = await _service.UpdateSellLocation(sellLocation.Id, mapSellLocation);
        return this.ApiOk(addSellLocation);
    }
    [HttpDelete]
    [Route("DeleteSellLocation/{id}/{name}")]
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
