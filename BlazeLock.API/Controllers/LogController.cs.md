using BlazeLock.API.Extensions;
using BlazeLock.API.Helpers;
using BlazeLock.API.Models;
using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazeLock.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/log/{idCoffre}")]
    [RequireVaultSession]
    public class LogController : ControllerBase
    {
        private readonly ICoffreService _coffreService;
        private readonly ILogService _logService;
        private readonly IUtilisateurService _utilisateurService;

        public LogController(ICoffreService coffreService, ILogService logService, IUtilisateurService utilisateurService)
        {
            _coffreService = coffreService;
            _logService = logService;
            _utilisateurService = utilisateurService;


        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<LogDto>>> GetLogsByCoffre(Guid idCoffre, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                await _logService.VerifyUserAccess(idCoffre, User.GetCurrentUserId());
                var logs = await _logService.GetByCoffrePagedAsync(idCoffre, pageNumber, pageSize);
                if (logs == null) return NotFound();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des logs pour le coffre {idCoffre}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLog(Guid idCoffre, [FromBody] string message)
        {
            try
            {
                var (userId, error) = User.GetCurrentUserId();
                if (error != null) return error;

                await _logService.VerifyUserAccess(idCoffre, (userId, null));

                await _logService.Add(idCoffre, userId, message);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la création du log.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                CoffreDto? coffre = await _coffreService.GetByIdAsync(id);
                if (coffre == null) return NotFound();

                await _logService.VerifyUserAccess(coffre.IdCoffre, User.GetCurrentUserId());

                var (userId, _) = User.GetCurrentUserId();
                if (coffre.IdUtilisateur != userId)
                    return Forbid(); 

                return Ok(coffre);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération du coffre {id}.");
            }
        }

    }
}