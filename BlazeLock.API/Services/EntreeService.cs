using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.EntityFrameworkCore;

namespace BlazeLock.API.Services
{
    public class EntreeService : IEntreeService
    {

        private readonly IEntreeRepository _entreeRepository;
        private readonly IHistoriqueEntreeRepository _historiqueEntreeRepository;
        private readonly IDossierRepository _dossierRepository;

        public EntreeService(IEntreeRepository EntreeRepository, IHistoriqueEntreeRepository HistoriqueEntreeRepository, IDossierRepository dossierRepository)
        {
            _entreeRepository = EntreeRepository;
            _historiqueEntreeRepository = HistoriqueEntreeRepository;
            _dossierRepository = dossierRepository;
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

        public async Task<HashSet<EntreeDto>> GetAllByDossierAsync(Guid IdDossier)
        {
            var entree = await _entreeRepository.GetAllByDossierAsync(IdDossier);
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
            var entree = await _entreeRepository.GetAllByDossierAsync(idCoffre);

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
