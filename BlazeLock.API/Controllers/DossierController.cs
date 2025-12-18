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
    [Route("api/dossier")]
    [RequireVaultSession]
    public class DossierController : ControllerBase
    {
        private readonly ICoffreService _coffreService;
        private readonly IDossierService _dossierService;
        private readonly IUtilisateurService _utilisateurService;

        public DossierController(ICoffreService coffreService, IUtilisateurService utilisateurService, IDossierService dossierService)
        {
            _coffreService = coffreService;
            _utilisateurService = utilisateurService;
            _dossierService = dossierService;
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

        [HttpGet("coffre/{idCoffre}")]
        public async Task<IActionResult> GetByCoffre(Guid idCoffre)
        {
            try
            {
                var dossiers = await _dossierService.GetByCoffreAsync(idCoffre);
                if (dossiers == null) return NotFound();

                CoffreDto? coffre = await _coffreService.GetByIdAsync(idCoffre);
                if (coffre == null) return NotFound();

                await _coffreService.VerifyUserAccess(coffre, User.GetCurrentUserId());

                await _dossierService.AddLog(dossiers.First(), User.GetCurrentUserId().userId, "Affichage des dossier du coffre");

                return Ok(dossiers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des dossiers pour le coffre {idCoffre}.");
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

                await _dossierService.AddLog(dossier, User.GetCurrentUserId().userId, "Affichage du dossier" + dossier.Libelle);

                return Ok(dossier);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération du dossier {id}.");
            }
        }

        [HttpPost("{idCoffre}")]
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
                await _dossierService.AddLog(dto, User.GetCurrentUserId().userId, "Création du dossier" + dto.Libelle);
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
                await _dossierService.AddLog(dto, User.GetCurrentUserId().userId, "Suppression du dossier" + dto.Libelle);
                return Ok("Partage supprimé");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la suppression du dossier.");
            }
        }
    }
}