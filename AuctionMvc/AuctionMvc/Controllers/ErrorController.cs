using System.Diagnostics;
using AuctionMvc.Helpers;
using AuctionMvc.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuctionMvc.Controllers
{
  /// <summary>
  /// Error handle features
  /// </summary>
  public class ErrorController : Controller
  {
    protected readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Handle exception
    /// </summary>
    /// <returns>Error view</returns>
    [HttpGet, HttpPost]
    public IActionResult Error()
    {
      var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

      if ((exceptionHandlerPathFeature != null) && (exceptionHandlerPathFeature.Error != null))
      {
        _logger.LogError(MessageHelper.AplicationException + exceptionHandlerPathFeature.Error);
      }

      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = MessageHelper.ErrorWillBeFixed });
    }

    /// <summary>
    /// Handle http error
    /// </summary>
    /// <param name="code"></param>
    /// <returns>Http error view</returns>
    [HttpGet, HttpPost]
    public IActionResult HttpError([FromQuery] int code = 500)
    {
      _logger.LogError(MessageHelper.HttpErrorStatus + code);

      string message = string.Empty;

      switch (code)
      {
        case 401:
          message = MessageHelper.LogInPlease;
          break;
        case 403:
          message = MessageHelper.NotPermissions;
          break;
        default:
          message = MessageHelper.ErrorWillBeFixed;
          break;
      }

      return View(new HttpErrorViewModel { Status = code, Message = message });
    }
  }
}
