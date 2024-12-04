using System.Threading.Tasks;
using DynamicPixels.GameService.Models.outputs;
using DynamicPixels.GameService.Services.Storage.Repositories;

namespace DynamicPixels.GameService.Services.Storage
{
    public class StorageService : IStorage
    {
        private StorageRepository _repository;

        public StorageService()
        {
            _repository = new StorageRepository();
        }


        public void Download()
        {
            throw new System.NotImplementedException();
        }

        public async Task<FileMetaForUpload> UploadAsync<TInput>(TInput param) where TInput : FileMetaForUpload
        {
            var result = await _repository.UploadFile(param);

            return result;
        }



        public Task<FileMetadata> GetFileInfo()
        {
            throw new System.NotImplementedException();
        }
    }
}