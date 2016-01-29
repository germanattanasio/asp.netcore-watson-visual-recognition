using System.Collections.Generic;
using System.Threading.Tasks;
using VR.Models;

namespace VR.Services
{
    public interface IWatsonVRService
    {
        bool ShareData { get; set; }

        Task<WatsonVRViewModel> ClassifyAsync(string filePath, int? maxScores = null, params string[] classifierIds);
        Task<WatsonVRClassifierViewModel> CreateClassifierAsync(string positiveExamplesPath, string negativeExamplesPath, string classifierName);
        Task<bool> DeleteClassifierAsync(string classifierId);
        Task<bool> DeleteClassifierAsync(WatsonVRClassifier classifier);
        Task<WatsonVRClassifierViewModel> GetClassifierAsync(string classifierId);
        Task<WatsonVRClassifiersResponse> GetClassifiersAsync();
    }
}
