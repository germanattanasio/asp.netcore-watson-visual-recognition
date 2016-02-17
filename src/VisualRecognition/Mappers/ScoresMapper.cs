using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualRecognition.Models;

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
