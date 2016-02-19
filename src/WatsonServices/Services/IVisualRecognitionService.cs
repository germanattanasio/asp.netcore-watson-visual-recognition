using System.Threading.Tasks;
using WatsonServices.Models;

namespace WatsonServices.Services
{
    public interface IVisualRecognitionService
    {
        bool ShareData { get; set; }

        Task<ClassifyResponse> ClassifyAsync(string filePath, params string[] classifierIds);
        Task<Classifier> CreateClassifierAsync(string positiveExamplesPath, string negativeExamplesPath, string classifierName);
        Task<bool> DeleteClassifierAsync(string classifierId);
        Task<bool> DeleteClassifierAsync(Classifier classifier);
        Task<Classifier> GetClassifierAsync(string classifierId);
        Task<ClassifiersResponse> GetClassifiersAsync();
    }
}
