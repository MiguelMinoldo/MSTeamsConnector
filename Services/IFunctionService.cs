using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSTeamsConnector.Models;

namespace MSTeamsConnector.Services
{
    public interface IFunctionService
    {
        Task<IActionResult> DoPostAsync(TeamsMessage teamsMessage);
    }
}