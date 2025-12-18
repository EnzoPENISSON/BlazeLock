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
    [Route("api/log")]
    [RequireVaultSession]
    public class LogController : ControllerBase
    {
        private readonly ICoffreService _coffreService;
        private readonly IEncryptService _encryptService;
        private readonly ILogService _logService;
        private readonly IUtilisateurService _utilisateurService;

        public LogController(ICoffreService coffreService, ILogService logService, IEncryptService encryptService, IUtilisateurService utilisateurService)
        {
            _coffreService = coffreService;
            _logService = logService;
            _encryptService = encryptService;
            _utilisateurService = utilisateurService;


        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var coffres = await _logService.GetAllAsync();
                return Ok(coffres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération de tous les logs.");
            }
        }


        [HttpGet("coffre/{idCoffre}")]
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

        [HttpPost]
        public async Task<IActionResult> Create(CoffreDto dto)
        {
            try
            {
                var (userId, errorResult) = User.GetCurrentUserId();
                if (errorResult != null) return errorResult;

                var userExists = await _utilisateurService.ExistsAsync(userId);
                if (!userExists)
                {
                    return NotFound("Utilisateur non trouvé.");
                }

                dto.IdUtilisateur = userId;
                await _encryptService.HashMasterKey(dto);
                return CreatedAtAction(nameof(GetById), new { id = dto.IdCoffre }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création du coffre." + ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(CoffreDto dto)
        {
            try
            {
                await _coffreService.VerifyUserAccess(dto, User.GetCurrentUserId());

                await _coffreService.Delete(dto);
                return Ok("Partage supprimé");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la suppression du dossier.");
            }
        }

        [HttpPost("verify-password")]
        public async Task<IActionResult> VerifyMasterKeyPassword([FromBody] CoffreDto dto)
        {
            if (dto == null) return BadRequest("Invalid client request");

            var (userId, errorResult) = User.GetCurrentUserId();
            if (errorResult != null) return Forbid();

            var existingCoffre = await _coffreService.GetByIdAsync(dto.IdCoffre);

            if (existingCoffre == null)
            {
                return NotFound("Coffre not found");
            }

            if (existingCoffre.IdUtilisateur != userId) return Forbid();

            bool isValid = await _encryptService.VerifyMasterKey(
                dto.ClearPassword,
                existingCoffre.Salt,
                existingCoffre.HashMasterkey
            );

            if (isValid)
            {
                return Ok(isValid);
            }
            return Unauthorized(isValid);
        }

    }
}