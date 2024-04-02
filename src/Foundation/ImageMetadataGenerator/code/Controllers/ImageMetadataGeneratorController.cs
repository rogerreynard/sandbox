using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Text;

namespace Foundation.ImageMetadataGenerator.Controllers
{
    using Models;
    using Repositories;
    using Helpers;

    public class ImageMetadataGeneratorController : ApiController
    {
        private readonly IImageMetadataGeneratorRepository _repo;
        
        public ImageMetadataGeneratorController(IImageMetadataGeneratorRepository repo)
        {
            _repo = repo;
        }

        /// POST api/imagemetadatagenerator/getmetadata
        [HttpPost]
        public async Task<HttpResponseMessage> GetMetadata([FromBody] string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var settings = _repo.GetSettings();
                    var uri = new Uri(settings.UrlField.Value);
                    var baseAddress = uri.Scheme + "://" + uri.Host;

                    client.PopulateHttpClient(baseAddress, settings.ApiKeyField.Value);

                    var payload = GetRequestBody(url, settings.ModelField.Value, int.Parse(settings.MaxTokensField.Value));
                    
                    var vResponse = await client.PostAsJsonAsync(uri.AbsolutePath, payload);

                    vResponse.EnsureSuccessStatusCode();

                    var response = await vResponse.Content.ReadAsAsync<VisionResponse>();

                    var content = response.Choices[0]?.Message?.Content;

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(content, Encoding.UTF8, "application/json")
                    };
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        private static VisionRequestBody GetRequestBody(string url, string model, int maxTokens)
        {
            var imageUrlProp = new VisionRequestImageUrlProperty { Url = url };
            var imageUrlType = new VisionRequestImageUrlType { ImageUrl = imageUrlProp };
            var textType = new VisionRequestTextType();
            var content = new ArrayList { textType, imageUrlType };
            var userRole = new VisionRequestUserRole { Content = content };
            return new VisionRequestBody
            {
                Model = model,
                Messages = new ArrayList { userRole },
                MaxTokens = maxTokens
            };
        }
    }
}