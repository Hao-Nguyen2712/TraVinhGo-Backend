using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Features.Destination.Interface;
using TraVinhMaps.Application.Features.EventAndFestivalFeature.Interface;
using TraVinhMaps.Application.Features.OcopProduct.Interface;

namespace TraVinhMaps.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly ITouristDestinationService _touristDestinationService;
    private readonly IEventAndFestivalService _eventAndFestivalService;
    private readonly IOcopProductService _ocopProductService;

    public HomeController(
        ITouristDestinationService touristDestinationService,
        IEventAndFestivalService eventAndFestivalService,
        IOcopProductService ocopProductService
        )
    {
        _touristDestinationService = touristDestinationService;
        _eventAndFestivalService = eventAndFestivalService;
        _ocopProductService = ocopProductService;
    }

    [HttpGet]
    [Route("GetDataHomePage")]
    public async Task<IActionResult> GetDataHomePage()
    {
        // Gọi song song cho nhanh (tối ưu), hoặc tuần tự nếu cần
        var favoriteDestinationsTask = _touristDestinationService.GetTop10FavoriteDestination();
        var topEventsTask = _eventAndFestivalService.GetTopUpcomingEvents();
        var ocopProductsTask = _ocopProductService.GetCurrentOcopProduct();

        await Task.WhenAll(favoriteDestinationsTask, topEventsTask, ocopProductsTask);

        var result = new
        {
            FavoriteDestinations = favoriteDestinationsTask.Result,
            TopEvents = topEventsTask.Result,
            OcopProducts = ocopProductsTask.Result
        };

        return this.ApiOk(result);
    }
}
