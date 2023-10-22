#nullable enable

namespace client_api
{
    // AnimalType types
    public class AnimalType
    {
        public string? Name { get; set; }
        public List<string>? Coats { get; set; }
        public List<string>? Genders { get; set; }
        public List<string>? Colors { get; set; }
        public List<string>? Breeds { get; set; }
        public TypeLinks? Links { get; set; }
    }

    public class TypeLinks
    {
        public Link? Self { get; set; }
        public Link? Breeds { get; set; }
    }

    // Animal types
    public class AnimalResponse
    {
        public List<Animal>? Animals { get; set; }
        public Pagination? Pagination { get; set; }
    }

    public class Animal
    {
        public int ID { get; set; }
        public string? OrganizationID { get; set; }
        public string? URL { get; set; }
        public string? Type { get; set; }
        public string? Species { get; set; }
        public Colors? Colors { get; set; }
        public string? Age { get; set; }
        public string? Gender { get; set; }
        public string? Size { get; set; }
        public string? Coat { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Photo>? Photos { get; set; }
        public string? Status { get; set; }
        public Attribute? Attributes { get; set; }
        public Environment? Environment { get; set; }
        public List<string>? Tags { get; set; }
        public Contact? Contact { get; set; }
    }

    public class Colors
    {
        public string? Primary { get; set; }
        public string? Secondary { get; set; }
        public string? Tertiary { get; set; }
    }

    public class Photo
    {
        public string? Small { get; set; }
        public string? Medium { get; set; }
        public string? Large { get; set; }
        public string? Full { get; set; }
    }

    public class Attribute
    {
        public bool? SpayedNeutered { get; set; }
        public bool? HouseTrained { get; set; }
        public bool? Declawed { get; set; }
        public bool? SpecialNeeds { get; set; }
        public bool? ShotsCurrent { get; set; }
    }

    public class Environmentpwd
    {
        public bool? Children { get; set; }
        public bool? Dogs { get; set; }
        public bool? Cats { get; set; }
    }

    // Organization types
    public class OrganizationResponse
    {
        public List<Organization>? Organizations { get; set; }
        public Pagination? Pagination { get; set; }
    }

    public class Organization
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Address? Address { get; set; }
        public Hours? Hours { get; set; }
        public string? URL { get; set; }
        public string? Website { get; set; }
        public string? MissionStatement { get; set; }
        public AdoptionPolicy? AdoptionPolicy { get; set; }
        public SocialMedia? SocialMedia { get; set; }
        public List<Photo>? Photos { get; set; }
        public OrganizationLinks? Links { get; set; }
    }

    public class Hours
    {
        public string? Monday { get; set; }
        public string? Tuesday { get; set; }
        public string? Wednesday { get; set; }
        public string? Thursday { get; set; }
        public string? Friday { get; set; }
        public string? Saturday { get; set; }
        public string? Sunday { get; set; }
    }

    public class SocialMedia
    {
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? Youtube { get; set; }
        public string? Instagram { get; set; }
        public string? Pinterest { get; set; }
    }

    public class OrganizationLinks
    {
        public Link? Self { get; set; }
        public Link? Animals { get; set; }
    }

    public class AdoptionPolicy
    {
        public string? Policy { get; set; }
        public string? URL { get; set; }
    }

    // Shared types
    public class Pagination
    {
        public int? CountPerPage { get; set; }
        public int? TotalCount { get; set; }
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
        public PaginationLinks? Links { get; set; }
    }

    public class PaginationLinks
    {
        public Link? Next { get; set; }
    }

    public class Contact
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Address? Address { get; set; }
    }

    public class Address
    {
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public string? Country { get; set; }
    }

    public class Link
    {
        public string? Href { get; set; }
    }

    public class Environment
    {
        public bool? Children { get; set; }
        public bool? Dogs { get; set; }
        public bool? Cats { get; set; }
    }

    public class SearchParams : Dictionary<string, string?>
    {
        public string CreateQueryString()
        {
            var queryString = "?";
            foreach (var param in this)
            {
                queryString += $"{param.Key}={param.Value}&";
            }
            return queryString.TrimEnd('&');
        }

        public void AddParam(string key, string? value)
        {
            this[key] = value;
        }

        public static SearchParams NewPetSearchParams()
        {
            return new SearchParams();
        }
    }
}


    public class SearchParams : Dictionary<string, string?>
    {
        public string CreateQueryString()
        {
            var queryString = "?";
            foreach (var param in this)
            {
                queryString += $"{param.Key}={param.Value}&";
            }
            return queryString.TrimEnd('&');
        }

        public void AddParam(string key, string? value)
        {
            this[key] = value;
        }

        public static SearchParams NewPetSearchParams()
        {
            return new SearchParams();
    }
}

