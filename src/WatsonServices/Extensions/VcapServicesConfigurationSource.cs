using Microsoft.Extensions.Configuration;

namespace WatsonServices.Extensions
{
    public class VcapServicesConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new VcapServicesConfigurationProvider(this);
        }
    }
}
