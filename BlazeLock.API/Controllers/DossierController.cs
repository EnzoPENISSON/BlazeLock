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
    [Route("api/dossier/{idCoffre}")]
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

        [HttpGet("{idDossier}")]
        public async Task<IActionResult> GetById(Guid idCoffre, Guid idDossier)
        { 
            try
            {
                var dossier = await _dossierService.GetByIdAsync(idDossier);

                if (dossier == null) return NotFound();

                await _dossierService.VerifyUserAccess(dossier, User.GetCurrentUserId());

                await _dossierService.AddLog(dossier, User.GetCurrentUserId().userId, "Affichage du dossier" + dossier.Libelle);

                return Ok(dossier);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération du dossier {idDossier}.");
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
                await _dossierService.AddLog(dto, User.GetCurrentUserId().userId, "Création du dossier" + dto.Libelle);
                return CreatedAtAction(nameof(GetById), new { id = dto.IdDossier }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création du dossier.");
            }
        }

        [HttpPut("{idDossier}")]
        public async Task<IActionResult> Update(Guid idCoffre, Guid idDossier, [FromBody] DossierDto dto)
        {
            try
            {
                if (idDossier != dto.IdDossier)
                {
                    return BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans le corps de la requête.");
                }

                var existingDossier = await _dossierService.GetByIdAsync(idDossier);
                if (existingDossier == null) return NotFound();

                if (existingDossier.IdCoffre != idCoffre)
                {
                    return BadRequest("Ce dossier n'appartient pas au coffre spécifié.");
                }

                await _dossierService.VerifyUserAccess(existingDossier, User.GetCurrentUserId());

                existingDossier.Libelle = dto.Libelle;
                await _dossierService.UpdateAsync(existingDossier);

                await _dossierService.AddLog(existingDossier, User.GetCurrentUserId().userId, "Renommage du dossier en " + dto.Libelle);

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Une erreur est survenue lors de la mise à jour du dossier.");
            }
        }

        [HttpDelete("{idDossier}")]
        public async Task<IActionResult> Delete(Guid idCoffre, Guid idDossier)
        {
            try
            {
                var dto = await _dossierService.GetByIdAsync(idDossier);
                if (dto == null) return NotFound();

                await _dossierService.VerifyUserAccess(dto, User.GetCurrentUserId());
                await _dossierService.Delete(dto);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur suppression.");
            }
        }
    }
}