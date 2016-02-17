using System.Collections.Generic;
using System.Threading.Tasks;
using VisualRecognition.Models;

namespace VisualRecognition.Services
{
    public interface IVisualRecognitionService
    {
        bool ShareData { get; set; }

        Task<VisualRecognitionViewModel> ClassifyAsync(string filePath, int? maxScores = null, params string[] classifierIds);
        Task<ClassifierViewModel> CreateClassifierAsync(string positiveExamplesPath, string negativeExamplesPath, string classifierName);
        Task<bool> DeleteClassifierAsync(string classifierId);
        Task<bool> DeleteClassifierAsync(Classifier classifier);
        Task<ClassifierViewModel> GetClassifierAsync(string classifierId);
        Task<ClassifiersResponse> GetClassifiersAsync();
    }
}
