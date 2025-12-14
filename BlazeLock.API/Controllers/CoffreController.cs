using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazeLock.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/coffre")]
    public class CoffreController : ControllerBase
    {
        private readonly ICoffreService _coffreService;
        private readonly IUtilisateurService _utilisateurService; // <--- Nouveau

        public CoffreController(ICoffreService coffreService, IUtilisateurService utilisateurService)
        {
            _coffreService = coffreService;
            _utilisateurService = utilisateurService;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var (userId, errorResult) = GetCurrentUserId();
            if (errorResult != null) return errorResult;

            var coffres = await _coffreService.GetByUtilisateurAsync(userId);

            if (coffres == null || !coffres.Any())
                return NoContent();

            return Ok(coffres);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var coffres = await _coffreService.GetAllAsync();
            return Ok(coffres);
        }

        [HttpGet("utilisateur/{id}")]
        public async Task<IActionResult> GetByUtilisateur(Guid id)
        {
            var coffres = await _coffreService.GetByUtilisateurAsync(id);
            if (coffres == null) return NotFound();
            return Ok(coffres);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var coffre = await _coffreService.GetByIdAsync(id);

            if (coffre == null) return NotFound();

            var (userId, _) = GetCurrentUserId();
            if (coffre.IdUtilisateur != userId)
                return NotFound(); // Pour ne pas révéler l'existence du coffre

            return Ok(coffre);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CoffreDto dto)
        {
            var (userId, errorResult) = GetCurrentUserId();
            if (errorResult != null) return errorResult;

            var userExists = await _utilisateurService.ExistsAsync(userId);
            if (!userExists)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // 3. Créer le coffre
            dto.IdUtilisateur = userId;
            await _coffreService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.IdCoffre }, dto);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(CoffreDto dto)
        {
            await _coffreService.Delete(dto);
            return Ok("Coffre supprimé");
        }

        private (Guid userId, IActionResult? error) GetCurrentUserId()
        {
            
            var userIdClaim = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = User.FindFirstValue("oid");
            }

            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return (Guid.Empty, Unauthorized("Impossible de récupérer l'ID utilisateur (Claims 'oid' ou 'NameIdentifier' manquants)."));
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                Console.WriteLine($"[AUTH ERROR] Valeur reçue non-GUID : {userIdClaim}");
                return (Guid.Empty, BadRequest($"L'ID utilisateur reçu n'est pas un GUID valide. Valeur reçue : {userIdClaim}"));
            }

            return (userId, null);
        }
    }
}