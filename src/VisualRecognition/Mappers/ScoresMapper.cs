using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;
using System.Linq;
namespace VisualRecognition.Mappers
{
    internal static class ScoresMapper
    {
        internal static ClassificationScoreViewModel Map(ClassificationScore fromModel)
        {
            ClassificationScoreViewModel toModel = new ClassificationScoreViewModel();
            toModel.ClassifierId = fromModel?.ClassifierId;
            toModel.ClassifierName = fromModel?.ClassifierName;
            toModel.ClassResults = fromModel?.ClassResults?.Select(ClassResultMapper.Map).ToArray();
            return toModel;
        }
    }
}
