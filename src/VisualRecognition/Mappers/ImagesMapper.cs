using System.Collections.Generic;
using System.Linq;
using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal class ImagesMapper
    {
        internal static VisualRecognitionViewModel Map(ClassifyResponse classifyResponse, ClassifyResponse facesResponse, ClassifyResponse wordsResponse)
        {
            VisualRecognitionViewModel toModel = new VisualRecognitionViewModel();

            toModel.Images = new List<ImageViewModel>();
            MapClassifyResponse(classifyResponse, toModel);
            MapWordsResponse(wordsResponse, toModel);
            MapFacesResponse(facesResponse, toModel);

            return toModel;
        }

        private static void MapClassifyResponse(ClassifyResponse classifyResponse, VisualRecognitionViewModel toModel)
        {
            if (classifyResponse == null)
                return;

            foreach (var image in classifyResponse.Images)
            {
                var currentImage = toModel.Images.FirstOrDefault(m => m.ImageName == image.ImageName);
                if (currentImage == null)
                {
                    toModel.Images.Add(new ImageViewModel()
                    {
                        ImageName = image.ImageName,
                        Scores = image?.Scores?.Select(ScoresMapper.Map).ToArray(),
                        ResolvedUrl = image.ResolvedUrl,
                        SourceUrl = image.SourceUrl,
                    });
                }
                else
                {
                    currentImage.Scores = image?.Scores?.Select(ScoresMapper.Map).ToArray();
                    currentImage.ResolvedUrl = currentImage.ResolvedUrl ?? image.ResolvedUrl;
                    currentImage.SourceUrl = currentImage.SourceUrl ?? image.SourceUrl;
                }
            }
        }

        private static void MapFacesResponse(ClassifyResponse facesResponse, VisualRecognitionViewModel toModel)
        {
            if (facesResponse == null)
                return;

            foreach (var image in facesResponse.Images)
            {
                var currentImage = toModel.Images.FirstOrDefault(m => m.ImageName == image.ImageName);
                if (currentImage == null)
                {
                    toModel.Images.Add(new ImageViewModel()
                    {
                        ImageName = image.ImageName,
                        Faces = image?.Faces?.Select(ImageFaceMapper.Map).ToArray(),
                        ResolvedUrl = image.ResolvedUrl,
                        SourceUrl = image.SourceUrl,
                    });
                }
                else
                {
                    currentImage.Faces = image?.Faces?.Select(ImageFaceMapper.Map).ToArray();
                    currentImage.ResolvedUrl = currentImage.ResolvedUrl ?? image.ResolvedUrl;
                    currentImage.SourceUrl = currentImage.SourceUrl ?? image.SourceUrl;
                }
            }
        }

        private static void MapWordsResponse(ClassifyResponse wordsResponse, VisualRecognitionViewModel toModel)
        {
            if (wordsResponse == null)
                return;

            foreach (var image in wordsResponse.Images)
            {
                var currentImage = toModel.Images.FirstOrDefault(m => m.ImageName == image.ImageName);
                if (currentImage == null)
                {
                    toModel.Images.Add(new ImageViewModel()
                    {
                        ImageName = image.ImageName,
                        Words = image?.Words?.Select(ImageWordsMapper.Map).ToArray(),
                        ResolvedUrl = image.ResolvedUrl,
                        SourceUrl = image.SourceUrl,
                        ImageText = image.ImageText
                    });
                }
                else
                {
                    currentImage.Words = image?.Words?.Select(ImageWordsMapper.Map).ToArray();
                    currentImage.ResolvedUrl = currentImage.ResolvedUrl ?? image.ResolvedUrl;
                    currentImage.SourceUrl = currentImage.SourceUrl ?? image.SourceUrl;
                    currentImage.ImageText = image?.ImageText;
                }
            }
        }
    }
}
