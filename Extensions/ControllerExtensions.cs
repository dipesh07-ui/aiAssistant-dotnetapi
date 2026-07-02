using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace aiAssistant.api.Extensions
{
    public static class ControllerExtensions
    {
        public static Guid GetUserId(this ControllerBase controller) =>
        Guid.Parse(controller.User
            .FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
