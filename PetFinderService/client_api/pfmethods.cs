using Newtonsoft.Json.Linq;

namespace client_api
{
    public partial class Client 
    {
        private const string DeserializationErrorMessage = "Failed to deserialize the response.";
        private const string KeyNotFoundErrorTemplate = "Expected '{0}' key not found in the response or failed to deserialize.";

        public async Task<List<AnimalType>> GetAllTypesAsync() 
        {
            byte[] body = await SendGetRequestAysnc("/types");
            var response = JObject.Parse(System.Text.Encoding.UTF8.GetString(body));
            if (response["types"] is JToken typesToken) 
            {
                return typesToken.ToObject<List<AnimalType>>() ?? new List<AnimalType>();
            }
            throw new Exception(string.Format(KeyNotFoundErrorTemplate, "types"));
        }

        public async Task<AnimalType> GetTypeAsync(string reqType) 
        {
            byte[] body = await SendGetRequestAysnc("/types/" + reqType);
            var response = JObject.Parse(System.Text.Encoding.UTF8.GetString(body));

            if (response?["type"] is JToken typeToken && typeToken.ToObject<AnimalType>() is AnimalType animalType) 
            {
                return animalType;
            }
            throw new Exception(string.Format(KeyNotFoundErrorTemplate, "type"));
        }

        public async Task<Animal> GetAnimalByIdAsync(string animalID) 
        {
            byte[] body = await SendGetRequestAysnc("/animals/" + animalID);
            var response = JObject.Parse(System.Text.Encoding.UTF8.GetString(body));
            if (response?["animal"] is JToken animalToken && animalToken.ToObject<Animal>() is Animal animal) 
            {
                return animal;
            }
            throw new Exception(string.Format(KeyNotFoundErrorTemplate, "animal"));
        }

        public async Task<AnimalResponse> GetAnimalsAsync(SearchParams searchParams) 
        {
            string paramString = searchParams.CreateQueryString();
            byte[] body = await SendGetRequestAysnc("/animals" + paramString);
            var response = JObject.Parse(System.Text.Encoding.UTF8.GetString(body));
            if (response?.ToObject<AnimalResponse>() is AnimalResponse animalResponse) 
            {
                return animalResponse;
            }
            throw new Exception(DeserializationErrorMessage);
        }

        public async Task<OrganizationResponse> GetOrganizationsAsync() 
        {
            byte[] body = await SendGetRequestAysnc("/organizations");
            var response = JObject.Parse(System.Text.Encoding.UTF8.GetString(body));
            if (response?.ToObject<OrganizationResponse>() is OrganizationResponse organizationResponse) 
            {
                return organizationResponse;
            }
            throw new Exception(DeserializationErrorMessage);
        }

        public async Task<Organization> GetOrganizationByIdAsync(string organizationID) 
        {
            byte[] body = await SendGetRequestAysnc("/organizations/" + organizationID);
            var response = JObject.Parse(System.Text.Encoding.UTF8.GetString(body));
            if (response?["organization"] is JToken organizationToken && organizationToken.ToObject<Organization>() is Organization organization) 
            {
                return organization;
            }
            throw new Exception(string.Format(KeyNotFoundErrorTemplate, "organization"));
        }
    }
}
