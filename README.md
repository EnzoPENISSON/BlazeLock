Liens du Repo : https://github.com/EnzoPENISSON/BlazeLock


# Analyse des Failles de Sécurité : Gestionnaire de Mots de Passe

Ce document répertorie les vulnérabilités identifiées dans le flux actuel de gestion des secrets et propose des pistes d'amélioration pour renforcer la sécurité globale du système.

## 1. Interception lors de la création

**Description :** Lorsqu'un utilisateur génère ou saisit un nouveau mot de passe, celui-ci peut être intercepté avant même d'être chiffré dans la base de données.

**Vecteurs d'attaque :**
* *Keyloggers* sur le poste client.
* Interception via le presse-papier (Copy-Paste).
* Écoute réseau si le flux n'est pas intégralement chiffré en TLS.

* **Risque :** Critique. Le secret est compromis à la source.

## 2. Fuite de données par les Logs

**Description :** Les systèmes de surveillance et de débogage (Application Logs, serveurs web) peuvent enregistrer par inadvertance des données sensibles en clair.

* **Scénario :** Une erreur survient lors d'une requête POST, et le serveur log l'intégralité du corps de la requête (contenant le mot de passe) dans un fichier.
* **Impact :** Élevé. Les administrateurs système ou toute personne ayant accès aux outils de monitoring (Elasticsearch, Splunk, etc.) peuvent lire les secrets.
* **Recommandation :** Implémenter un système de masquage (*data masking*) pour filtrer les champs secrets dans les logs.

## 3. Partage du mot de passe maître (Master Password)

**Description :** L'architecture actuelle impose de partager le mot de passe du coffre-fort pour permettre l'accès à un tiers.

**Le problème :** Cela contrevient au principe de
**non-répudiation**. Si le mot de passe est partagé, il est impossible de savoir qui a effectué quelle action.
* **Risques associés :**
* Nécessité de changer le mot de passe de tout le coffre dès qu'un membre quitte l'équipe.
* Exposition accrue de la "Clé Maître".
