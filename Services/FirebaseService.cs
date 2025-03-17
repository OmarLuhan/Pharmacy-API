using Farma_api.Configuration;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
namespace Farma_api.Services;

public interface IFirebaseService
{
    Task<string> UploadToFirebaseAsync(Stream fileStream, string objectName, string contentType);
    Task DeleteStorage(string directory, string fileName);
}

public class FirebaseService : IFirebaseService
{
    private readonly string _bucketName;
    private readonly StorageClient _storageClient;

    public FirebaseService(IConfiguration config)
    {
        var settings = config.GetSection("FireBaseSettings").Get<FirebaseSettings>();
        if (settings== null) return;

        _bucketName = settings.StorageBucket;
        var credential = GoogleCredential.FromFile(settings.ServiceAccountPath);
        _storageClient = StorageClient.Create(credential);
    }

    public async Task DeleteStorage(string directory, string fileName)
    {
        try
        {
            var objectName = $"{directory}/{fileName}";
            await _storageClient.DeleteObjectAsync(
                _bucketName,
                objectName);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar archivo de Firebase Storage: {ex.Message}");
        }
    }

    public async Task<string> UploadToFirebaseAsync(Stream fileStream, string objectName, string contentType)
    {
        var metadata = new Dictionary<string, string>{
            { "firebaseStorageDownloadTokens", Guid.NewGuid().ToString() }
         };
        try
        {
            var obj = new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = objectName,
                ContentType = contentType,
                Metadata = metadata
            };
            await _storageClient.UploadObjectAsync(obj, source: fileStream);
            var downloadToken = metadata["firebaseStorageDownloadTokens"];
            var fileUrl = GenerateFirebaseUrl(_bucketName, objectName, downloadToken);
            return fileUrl;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al subir archivo a Firebase Storage: {ex.Message}");
        }
    }
    private static string GenerateFirebaseUrl(string bucketName, string objectName, string accessToken)
    {
        var escapedObjectName = Uri.EscapeDataString(objectName);
        return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{escapedObjectName}?alt=media&token={accessToken}";
    }

}