using BlazeLock.API.Extensions;
using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BlazeLock.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/coffre")]
    public class CoffreController : ControllerBase
    {
        private readonly ICoffreService _coffreService;
        private readonly IEncryptService _encryptService;
        private readonly ILogService _logService;
        private readonly IUtilisateurService _utilisateurService;

        private readonly IMemoryCache _cache;

        public CoffreController(ICoffreService coffreService, ILogService logService, IEncryptService encryptService, IUtilisateurService utilisateurService, IMemoryCache cache)
        {
            _coffreService = coffreService;
            _logService = logService;
            _encryptService = encryptService;
            _utilisateurService = utilisateurService;
            _cache = cache;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            try
            {
                var (userId, errorResult) = User.GetCurrentUserId();
                if (errorResult != null) return errorResult;

                var coffres = await _coffreService.GetByUtilisateurAsync(userId);

                if (coffres == null || !coffres.Any())
                    return NoContent();

                return Ok(coffres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération de vos coffres.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var coffres = await _coffreService.GetAllAsync();
                return Ok(coffres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération de tous les coffres.");
            }
        }

        [HttpGet("utilisateur/{id}")]
        public async Task<IActionResult> GetByUtilisateur(Guid id)
        {
            try
            {
                var coffres = await _coffreService.GetByUtilisateurAsync(id);
                if (coffres == null) return NotFound();

                return Ok(coffres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des coffres pour l'utilisateur {id}.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var coffre = await _coffreService.GetByIdAsync(id);

                if (coffre == null) return NotFound();

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
                await _encryptService.HashMasterKey(dto); // Hash the master key before and creating the coffre
                await _coffreService.AddLog(dto.IdCoffre, userId, "Création du coffre");
                return CreatedAtAction(nameof(GetById), new { id = dto.IdCoffre }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création du coffre." + ex);
            }
        }

        [HttpDelete("{idCoffre}")]
        public async Task<IActionResult> Delete(Guid idCoffre)
        {
            try 
            {
                var (userId, errorResult) = User.GetCurrentUserId();
                if (errorResult != null) return errorResult;
                var existingCoffre = await _coffreService.GetByIdAsync(idCoffre);
                if (existingCoffre == null) return NotFound();
                if (existingCoffre.IdUtilisateur != userId)
                    return Forbid();
                await _coffreService.Delete(idCoffre);
                await _coffreService.AddLog(null, userId, $"Suppression du coffre '{idCoffre}'");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la suppression du coffre.");
            }
        }

        [HttpPost("verify-password")]
        public async Task<IActionResult> VerifyMasterKeyPassword([FromBody] CoffreDto dto)
        {
            if (dto == null) return BadRequest("Invalid client request");

            var (userId, errorResult) = User.GetCurrentUserId();
            if (errorResult != null) return Forbid();

            var existingCoffre = await _coffreService.GetByIdAsync(dto.IdCoffre);

            if (existingCoffre == null) return NotFound("Coffre not found");
            if (existingCoffre.IdUtilisateur != userId) return Forbid();

            bool isValid = await _encryptService.VerifyMasterKey(
                dto.ClearPassword,
                existingCoffre.Salt,
                existingCoffre.HashMasterkey
            );

            if (isValid)
            {
                byte[] derivedKey = await _encryptService.GetDerivedKey(
                    dto.ClearPassword,
                    existingCoffre.Salt
                );

                string sessionKey = $"Session_{userId}_{dto.IdCoffre}";

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetPriority(CacheItemPriority.High);

                _cache.Set(sessionKey, derivedKey, cacheOptions);

                await _coffreService.AddLog(dto.IdCoffre, userId, "Ouverture du coffre");

                return Ok(new { IsValid = true, Message = "Session active for 5 minutes" });
            }

            await _coffreService.AddLog(dto.IdCoffre, userId, "Erreur lors de l'ouverture du coffre");
            return Unauthorized(new { IsValid = false });
        }
    }
}