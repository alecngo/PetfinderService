using client_api;

namespace PetFinderService
{
    public class Program
    {
        /// The entry point for the program. Gets information from Dog's API and prints it to the console.
        public static async Task Main(string[] args)
        {
            try
            {
                var client = Client.GetClient();

                // Retrieve all animal types and print details
                List<AnimalType> types = await client.GetAllTypesAsync();
                if (types != null)
                {
                    foreach (var t in types)
                    {
                        Logger.Info($"Name: {t?.Name ?? "N/A"}");
                        Logger.Info($"Colors: {string.Join(", ", t?.Colors ?? new List<string>())}");
                        Logger.Info($"Self Link: {t?.Links?.Self?.Href ?? "N/A"}");
                    }
                }

                // Get a specific type
                AnimalType myType = await client.GetTypeAsync("dog");
                if (myType != null)
                {
                    Logger.Info(myType.Name);
                }

                // Get a particular animal by ID
                Animal myAnimal = await client.GetAnimalByIdAsync("68670528");
                if (myAnimal != null)
                {
                    Logger.Info($"{myAnimal.ID}, {myAnimal.Species}");
                }
                // Removed the Breeds property, add it back if Animal class actually has it

                // Search for animals with specific parameters
                client_api.SearchParams myParams = new client_api.SearchParams(); // ensure correct SearchParams
                myParams.AddParam("type", "Dog");
                myParams.AddParam("coat", "Medium");

                AnimalResponse myAnimals = await client.GetAnimalsAsync(myParams);
                if (myAnimals?.Animals != null)
                {
                    foreach (var a in myAnimals.Animals)
                    {
                        Logger.Info(a?.Name ?? "N/A");
                        // Check if Photos is not null and then iterate through each photo.
                        if (a?.Photos != null)
                        {
                            foreach (var photo in a.Photos)
                            {
                                Logger.Info(photo?.Medium ?? "No medium photo available");
                            }
                        }
                    }
                }
                Logger.Info(myAnimals?.Pagination?.TotalCount ?? 0);

                // Retrieve all organizations and print details
                OrganizationResponse myOrgs = await client.GetOrganizationsAsync();
                if (myOrgs?.Organizations != null)
                {
                    foreach (var a in myOrgs.Organizations)
                    {
                        Logger.Info($"Org Name: {a?.ID ?? "N/A"}");
                    }
                }
                Logger.Info(myOrgs?.Pagination?.TotalCount ?? 0);

                // Retrieve a specific organization by ID
                Organization myOrg = await client.GetOrganizationByIdAsync("KY422");
                if (myOrg != null)
                {
                    Logger.Info("Org filter:");
                    Logger.Info(myOrg); // Adjust this as needed based on the structure of Organization object
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Error: {ex.Message}");
            }
        }
    }
}
