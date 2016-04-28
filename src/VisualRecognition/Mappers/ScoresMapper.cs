using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal class ScoresMapper
    {
        internal static ClassificationScoreViewModel Map(ClassificationScore fromModel)
        {
            ClassificationScoreViewModel toModel = new ClassificationScoreViewModel();
            toModel.ClassifierId = fromModel.ClassifierId;
            toModel.ClassifierName = fromModel.ClassifierName;
            toModel.Score = fromModel.Score.ToString();
            return toModel;
        }
    }
}
