using BlazeLock.API.Extensions;
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
    [Route("api/dossier")]
    public class DossierController : ControllerBase
    {
        private readonly ICoffreService _coffreService;
        private readonly IDossierService _dossierService;
        private readonly IEncryptService _encryptService;
        private readonly IUtilisateurService _utilisateurService;

        public DossierController(ICoffreService coffreService, IUtilisateurService utilisateurService, IEncryptService encryptService)
        {
            _coffreService = coffreService;
            _utilisateurService = utilisateurService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var dossiers = await _dossierService.GetAllAsync();
                return Ok(dossiers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération de tous les dossiers.");
            }
        }

        [HttpGet("coffre/{id}")]
        public async Task<IActionResult> GetByCoffre(Guid IdCoffre)
        {
            try
            {
                var dossiers = await _dossierService.GetByCoffreAsync(IdCoffre);
                if (dossiers == null) return NotFound();

                CoffreDto coffre = await _coffreService.GetByIdAsync(IdCoffre);
                if (coffre == null) return NotFound();

                await _coffreService.VerifyUserAccess(coffre, User.GetCurrentUserId());

                return Ok(dossiers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des dossiers pour le coffre {IdCoffre}.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var dossier = await _dossierService.GetByIdAsync(id);

                if (dossier == null) return NotFound();

                await _dossierService.VerifyUserAccess(dossier, User.GetCurrentUserId());

                return Ok(dossier);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération du coffre {id}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(DossierDto dto) 
        {
            try
            {
                await _dossierService.VerifyUserAccess(dto, User.GetCurrentUserId());

                if (dto.IdDossier == Guid.Empty)
                {
                    dto.IdDossier = Guid.NewGuid();
                }

                await _dossierService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = dto.IdDossier }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création du dossier.");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DossierDto dto)
        {
            try
            {
                await _dossierService.VerifyUserAccess(dto, User.GetCurrentUserId());

                await _dossierService.Delete(dto);
                return Ok("Partage supprimé");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la suppression du dossier.");
            }
        }
    }
}