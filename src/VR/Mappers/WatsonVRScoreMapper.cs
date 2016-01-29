using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VR.Models;

namespace VR.Mappers
{
    public class WatsonVRScoreMapper
    {
        public static WatsonVRScoreViewModel Map(WatsonVRScore fromModel)
        {
            WatsonVRScoreViewModel toModel = new WatsonVRScoreViewModel();
            toModel.ClassifierId = fromModel.ClassifierId;
            toModel.ClassifierName = fromModel.ClassifierName;
            toModel.Score = fromModel.Score.ToString("P2");
            return toModel;
        }
    }
}
