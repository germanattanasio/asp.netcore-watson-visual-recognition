using VisualRecognition.ViewModels;
using WatsonServices.Models;

namespace VisualRecognition.Mappers
{
    public class ScoresMapper
    {
        public static ClassificationScoreViewModel Map(ClassificationScore fromModel)
        {
            ClassificationScoreViewModel toModel = new ClassificationScoreViewModel();
            toModel.ClassifierId = fromModel.ClassifierId;
            toModel.ClassifierName = fromModel.ClassifierName;
            toModel.Score = fromModel.Score.ToString("P2");
            return toModel;
        }
    }
}
