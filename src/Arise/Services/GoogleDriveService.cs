namespace Arise.Core.Services
{
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v3;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Drive = Google.Apis.Drive.v3.Data;

    public class GoogleDriveService
    {
        #region Fields

        private static readonly string CredentialPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credentials/drive-dotnet-quickstart.json"); 
        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "Arise";
        private UserCredential _userCredential;

        #endregion

        #region Constructors

        public GoogleDriveService()
        {
        }

        #endregion

        #region Methods

        public async Task<DriveService> AuthorizeAsync()
        {
            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                this._userCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(CredentialPath, true));
            }

            return this.CreateDriveService();
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        public async Task<Drive.File> CreateFolder(string folderName)
        {
            return await this.CreateFolder(folderName, string.Empty);
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="parentFolderId">The parent folder identifier.</param>
        public async Task<Drive.File> CreateFolder(string folderName, string parentFolderId)
        {
            var fileMetadata = new Drive.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };

            if (!string.IsNullOrEmpty(parentFolderId))
            {
                fileMetadata.Parents = new List<string> { parentFolderId };
            }

            var service = await this.AuthorizeAsync();
            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            return request.Execute();
        }

        /// <summary>
        /// Creates the drive service.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Authorize the service first!</exception>
        private DriveService CreateDriveService()
        {
            if (this._userCredential == null)
            {
                throw new InvalidOperationException("Authorize the service first!");
            }

            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = this._userCredential,
                ApplicationName = ApplicationName,
            });
        }

        #endregion
    }
}
