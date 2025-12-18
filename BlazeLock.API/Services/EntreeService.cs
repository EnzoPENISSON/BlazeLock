using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public class EntreeService : IEntreeService
    {

        private readonly IEntreeRepository _entreeRepository;
        private readonly IHistoriqueEntreeRepository _historiqueEntreeRepository;
        private readonly IDossierRepository _dossierRepository;
        private readonly ICoffreRepository _coffreRepository;
        private readonly ILogRepository _logRepository;

        public EntreeService(IEntreeRepository EntreeRepository, IHistoriqueEntreeRepository HistoriqueEntreeRepository, IDossierRepository dossierRepository, ICoffreRepository coffreRepository, ILogRepository logRepository)
        {
            _entreeRepository = EntreeRepository;
            _historiqueEntreeRepository = HistoriqueEntreeRepository;
            _dossierRepository = dossierRepository;
            _coffreRepository = coffreRepository;
            _logRepository = logRepository;
        }

        public async Task<HashSet<EntreeDto>> GetAllAsync()
        {
            var entree = await _entreeRepository.GetAllAsync();
            var result = entree
                .Select(c => new EntreeDto
                {
                    IdEntree = c.IdEntree,
                    DateCreation = c.DateCreation,
                    IdDossier = c.IdDossier
                })
                .ToHashSet();

            return result;
        }

        public async Task<HashSet<EntreeDto>> GetAllByDossierAsync(Guid idCoffre, Guid IdDossier)
        {
            var entree = await _entreeRepository.GetAllByDossierAsync(IdDossier);
            var result = entree.Select(e =>
            {
                var latest = e.HistoriqueEntrees?
                    .OrderByDescending(h => h.DateUpdate)
                    .FirstOrDefault();

                return new EntreeDto
                {
                    IdEntree = e.IdEntree,
                    IdDossier = IdDossier,
                    DateCreation = e.DateCreation,

                    Libelle = latest?.Libelle,
                    DateUpdate = latest?.DateUpdate ?? DateTime.MinValue,

                    idCoffre = idCoffre,

                    Username = latest?.Username,
                    UsernameTag = latest?.UsernameTag,
                    UsernameVi = latest?.UsernameVi,

                    Password = latest?.Password,
                    PasswordTag = latest?.PasswordTag,
                    PasswordVi = latest?.PasswordVi,

                    Url = latest?.Url,
                    UrlTag = latest?.UrlTag,
                    UrlVi = latest?.UrlVi,

                    Commentaire = latest?.Commentaire,
                    CommentaireTag = latest?.CommentaireTag,
                    CommentaireVi = latest?.CommentaireVi
                };
            })
            .ToHashSet();

            return result;
        }

        public async Task<EntreeDto?> GetByIdAsync(Guid idEntree)
        {
            var entree = await _entreeRepository.GetByIdAsync(idEntree);
            if (entree == null)
            {
                return null;
            }
            var historique = await _historiqueEntreeRepository.GetLastByIdAsync(idEntree);

            var result = new EntreeDto
            {
                IdEntree = idEntree,
                DateCreation = entree.DateCreation,
                DateUpdate = historique.DateUpdate,
                Libelle = historique.Libelle,
                Username = historique.Username,
                UsernameTag = historique.UsernameTag,
                UsernameVi = historique.UsernameVi,
                Url = historique.Url,
                UrlTag = historique.UrlTag,
                UrlVi = historique.UrlVi,
                Password = historique.Password,
                PasswordTag = historique.PasswordTag,
                PasswordVi = historique.PasswordVi,
                Commentaire = historique.Commentaire,
                CommentaireTag = historique.CommentaireTag,
                CommentaireVi = historique.CommentaireVi,
            };

            return result;
        }

        public async Task<EntreeHistoriqueDto?> GetByIdWithHistoriaqueAsync(Guid idEntree)
        {
            var entree = await _entreeRepository.GetByIdAsync(idEntree);
            if (entree == null)
            {
                return null;
            }
            var historique = await _historiqueEntreeRepository.GetAllByEntreeAsync(idEntree);

            var result = new EntreeHistoriqueDto
            {
                IdEntree = idEntree,
                DateCreation = entree.DateCreation,
                Historique = historique.Select(h => new HistoriqueDto
                {
                    DateUpdate = h.DateUpdate,
                    Libelle = h.Libelle,
                    Username = h.Username,
                    UsernameTag = h.UsernameTag,
                    UsernameVi = h.UsernameVi,
                    Url = h.Url,
                    UrlTag = h.UrlTag,
                    UrlVi = h.UrlVi,
                    Password = h.Password,
                    PasswordTag = h.PasswordTag,
                    PasswordVi = h.PasswordVi,
                    Commentaire = h.Commentaire,
                    CommentaireTag = h.CommentaireTag,
                    CommentaireVi = h.CommentaireVi
                }).ToHashSet()
            };
            return result;
        }

        public async Task AddAsync(EntreeDto dto)
        {
            var existingEntree = await _entreeRepository.GetByIdAsync(dto.IdEntree);

            Guid finalId;

            if (existingEntree == null)
            {
                var existingDefaultFolder = await _dossierRepository.GetByLibelleAndCoffreIdAsync("Default", dto.idCoffre);
                var newEntree = new Entree
                {
                    IdEntree = dto.IdEntree == Guid.Empty ? Guid.NewGuid() : dto.IdEntree,
                    DateCreation = dto.DateCreation == default ? DateTime.UtcNow : dto.DateCreation,
                    IdDossier = existingDefaultFolder.IdDossier
                };

                await _entreeRepository.AddAsync(newEntree);

                finalId = newEntree.IdEntree;
            }
            else
            {
                finalId = existingEntree.IdEntree;
            }

            var newHistorique = new HistoriqueEntree
            {
                IdEntree = finalId,
                DateUpdate = DateTime.UtcNow,
                Libelle = dto.Libelle,
                Username = dto.Username,
                UsernameTag = dto.UsernameTag,
                UsernameVi = dto.UsernameVi,
                Url = dto.Url,
                UrlTag = dto.UrlTag,
                UrlVi = dto.UrlVi,
                Password = dto.Password,
                PasswordTag = dto.PasswordTag,
                PasswordVi = dto.PasswordVi,
                Commentaire = dto.Commentaire,
                CommentaireTag = dto.CommentaireTag,
                CommentaireVi = dto.CommentaireVi
            };

            await _historiqueEntreeRepository.AddAsync(newHistorique);
        }

        public async Task<HashSet<EntreeDto>> GetAllByCoffreAsync(Guid idCoffre)
        {
            var entities = await _entreeRepository.GetAllByCoffreAsync(idCoffre);

            var result = entities.Select(e =>
            {
                var latest = e.HistoriqueEntrees?
                    .OrderByDescending(h => h.DateUpdate)
                    .FirstOrDefault();

                return new EntreeDto
                {
                    IdEntree = e.IdEntree,
                    IdDossier = e.IdDossier,
                    DateCreation = e.DateCreation,

                    Libelle = latest?.Libelle,
                    DateUpdate = latest?.DateUpdate ?? DateTime.MinValue,

                    idCoffre = idCoffre,

                    Username = latest?.Username,
                    UsernameTag = latest?.UsernameTag,
                    UsernameVi = latest?.UsernameVi,

                    Password = latest?.Password,
                    PasswordTag = latest?.PasswordTag,
                    PasswordVi = latest?.PasswordVi,

                    Url = latest?.Url,
                    UrlTag = latest?.UrlTag,
                    UrlVi = latest?.UrlVi,

                    Commentaire = latest?.Commentaire,
                    CommentaireTag = latest?.CommentaireTag,
                    CommentaireVi = latest?.CommentaireVi
                };
            })
            .ToHashSet();

            return result;
        }

        public async Task updateAsync(Guid idEntree, Guid IdDossier)
        {
            var existingEntree = await _entreeRepository.GetByIdAsync(idEntree);
            if (existingEntree != null)
            {
                var newEntree = new Entree
                {
                    IdEntree = existingEntree.IdEntree,
                    DateCreation = existingEntree.DateCreation,
                    IdDossier = IdDossier
                };
                Console.WriteLine(newEntree);
                await _entreeRepository.updateAsync(newEntree);
            }
        }

        public async Task Delete(Guid idEntree)
        {
            var existingEntree = await _entreeRepository.GetByIdAsync(idEntree);
            if (existingEntree != null)
            {
                await _entreeRepository.DeleteEntree(existingEntree);
            }
        }
        public async Task<IActionResult?> VerifyUserAccess(EntreeDto entreDto, (Guid, IActionResult?) utilisateur)
        {

            var (userId, errorResult) = utilisateur;
            if (errorResult != null) return errorResult;

            var dossier = await _dossierRepository.GetByIdAsync(entreDto.IdDossier);
            if (dossier == null) return new BadRequestObjectResult("Le dossier associé à cette entrée n'a pas été trouvé.");

            var coffre = await _coffreRepository.GetByIdAsync(dossier.IdCoffre);
            if (coffre == null) return new BadRequestObjectResult("Le coffre associé à cette entrée n'a pas été trouvé.");

            if (coffre.IdUtilisateur != userId) return new UnauthorizedObjectResult("Utilisateur non autorisé");

            return null;
        }


        public  async Task AddLog(EntreeDto entree, Guid idUtilisateur, string message)
        {
            var dossier = await _dossierRepository.GetByIdAsync(entree.IdDossier);
            var entity = new Log
            {
                IdCoffre = dossier.IdCoffre,
                IdUtilisateur = idUtilisateur,
                Texte = message,
                Timestamp = DateTime.UtcNow
            };
            await _logRepository.AddAsync(entity);
        }

        //public async Task Delete(EntreeDto dto)
        //{
        //    var entity = new Entree
        //    {
        //        IdEntree = dto.IdEntree,
        //        IdUtilisateur = dto.IdUtilisateur,
        //        Libelle = dto.Libelle,
        //        HashMasterkey = dto.HashMasterkey,
        //        Salt = dto.Salt
        //    };
        //    await _repository.DeleteEntree(entity);
        //}
    }
}
