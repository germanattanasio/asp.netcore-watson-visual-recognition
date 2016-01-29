using System;
using VR.Models;

namespace VR.Mappers
{
    public class WatsonVRClassifierMapper
    {
        public static WatsonVRClassifierViewModel Map(WatsonVRClassifier fromModel)
        {
            WatsonVRClassifierViewModel toModel = new WatsonVRClassifierViewModel();
            toModel.ClassifierId = fromModel.ClassifierId;
            DateTime createdTime;
            if (DateTime.TryParse(fromModel.CreatedTime, out createdTime))
            {
                toModel.CreatedTime = createdTime;
            }
            toModel.Name = fromModel.Name;
            toModel.Owner = fromModel.Owner;

            return toModel;
        }
    }
}
