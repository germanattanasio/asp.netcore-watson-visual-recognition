using System.Collections.Generic;

namespace VisualRecognition.ViewModels
{
    public static class SampleImageBundles
    {
        internal const string DefaultBundleId = "default";
        internal static readonly Dictionary<string, string[]> ImageBundles = new Dictionary<string, string[]>()
        {
            {
                DefaultBundleId, new string[]
                {
                    "images/samples/1.jpg",
                    "images/samples/2.jpg",
                    "images/samples/3.jpg",
                    "images/samples/4.jpg",
                    "images/samples/5.jpg",
                    "images/samples/6.jpg",
                    "images/samples/7.jpg"
                }
            },
            {
                "dogs", new string[]
                {
                    "images/bundles/dogs/test/0.jpg",
                    "images/bundles/dogs/test/1.jpg",
                    "images/bundles/dogs/test/2.jpg",
                    "images/bundles/dogs/test/3.jpg",
                    "images/bundles/dogs/test/4.jpg",
                    "images/bundles/dogs/test/5.jpg"
                }
            },
            {
                "fruits", new string[]
                {
                    "images/bundles/fruits/test/0.jpg",
                    "images/bundles/fruits/test/1.jpg",
                    "images/bundles/fruits/test/2.jpg",
                    "images/bundles/fruits/test/3.jpg",
                    "images/bundles/fruits/test/4.jpg",
                    "images/bundles/fruits/test/5.jpg"
                }
            },
            {
                "insurance", new string[]
                {
                    "images/bundles/insurance/test/0.jpg",
                    "images/bundles/insurance/test/1.jpg",
                    "images/bundles/insurance/test/2.jpg",
                    "images/bundles/insurance/test/3.jpg",
                    "images/bundles/insurance/test/4.jpg",
                    "images/bundles/insurance/test/5.jpg"
                }
            },
            {
                "moleskine", new string[]
                {
                    "images/bundles/moleskine/test/0.jpg",
                    "images/bundles/moleskine/test/1.jpg",
                    "images/bundles/moleskine/test/2.jpg",
                    "images/bundles/moleskine/test/3.jpg",
                    "images/bundles/moleskine/test/4.jpg",
                    "images/bundles/moleskine/test/5.jpg"
                }
            },
            {
                "omniearth", new string[]
                {
                    "images/bundles/omniearth/test/0.jpg",
                    "images/bundles/omniearth/test/1.jpg",
                    "images/bundles/omniearth/test/2.jpg",
                    "images/bundles/omniearth/test/3.jpg",
                    "images/bundles/omniearth/test/4.jpg",
                    "images/bundles/omniearth/test/5.jpg"
                }
            }
        };
    }
}
