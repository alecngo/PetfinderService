package petfinderapi

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"os"
	"sync"

	"github.com/mitchellh/mapstructure"
	"golang.org/x/oauth2"
	"golang.org/x/oauth2/clientcredentials"
)

// DefaultBaseURL contains url for petfinder API
const DefaultBaseURL = "https://api.petfinder.com/v2"

var existingClient *Client
var once sync.Once
var clientErr error

// Client struct is used to hold http.Client
type Client struct {
	*http.Client
}

type Breed struct {
	Name string `json:"name"`
}

// url is a private function to determine what url to use
// It will use first the environment variable "PF_BASE_URL" or the constant "DefaultBaseURL"
func url() string {
	url := os.Getenv("PF_BASE_URL")
	if url != "" {
		return url
	}
	return DefaultBaseURL
}

func (c Client) httpGet(url string) ([]byte, error) {
	req, err := http.NewRequest("GET", url, nil)
	if err != nil {
		return nil, err
	}
	req.Header.Set("x-api-sdk", "petfinder-go-sdk (https://github.com/petfinder-com/petfinder-go-sdk)")
	response, err := c.Do(req)
	if err != nil {
		fmt.Println("Cannot get response: ", err)
		return nil, err
	}
	defer response.Body.Close()
	body, err := ioutil.ReadAll(response.Body)
	if err != nil {
		fmt.Println("Cannot read response: ", err)
		return nil, err
	}
	return body, nil
}

// sendRequest is a private function accepting a path as a variable
// It combines url + path to create the request and sends the request
func (c Client) sendGetRequest(path string) ([]byte, error) {
	url := fmt.Sprintf("%s%s", url(), path)

	body, err := c.httpGet(url)

	return body, err
}

// NewClient accepts client id and secret client id issued by Petfinder
// It returns a struct callled Client that contains a pointer to http.Client
func GetClient() (*Client, error) {
	once.Do(func() {
		// Pull Client ID key and Client Secret Key from environment variables
		clientID := os.Getenv("PF_CLIENT_ID")
		clientSecret := os.Getenv("PF_CLIENT_SECRET")

		// Create pfclient Object
		pfclient, err := newClient(clientID, clientSecret)
		if err != nil {
			fmt.Println("Could not create client")
			clientErr = err
			return
		}
		fmt.Println("Created Client")
		existingClient = pfclient
	})
	return existingClient, clientErr
}

func newClient(accessToken string, secretAccessToken string) (*Client, error) {
	url := url()

	conf := &clientcredentials.Config{
		ClientID:     accessToken,
		ClientSecret: secretAccessToken,
		Scopes:       []string{},
		TokenURL:     url + "/oauth2/token/",
	}

	client := conf.Client(oauth2.NoContext)

	return &Client{client}, nil
}

// GetAllTypes function is a method of Client
// It returns a struct of animals types and error
func (c Client) GetAllTypes() ([]AnimalType, error) {
	body, err := c.sendGetRequest("/types")
	if err != nil {
		fmt.Println("Error in GetAllTypes:", err)
		return nil, err
	}

	var response struct {
		Types []AnimalType `json:"types"`
	}
	if err := json.Unmarshal(body, &response); err != nil {
		fmt.Println("Error unmarshaling GetAllTypes response:", err)
		return nil, err
	}

	return response.Types, nil
}

// GetType returns an AnimalType by its name and an error, if any.
func (c Client) GetType(reqType string) (AnimalType, error) {
	body, err := c.sendGetRequest("/types/" + reqType)
	if err != nil {
		fmt.Println("Error in GetType:", err)
		return AnimalType{}, err
	}

	var response struct {
		Type AnimalType `json:"type"`
	}
	if err := json.Unmarshal(body, &response); err != nil {
		fmt.Println("Error unmarshaling GetType response:", err)
		return AnimalType{}, err
	}

	return response.Type, nil
}

func (c Client) getBreedsForType(redType string) ([]Breed, error) {
	body, err := c.sendGetRequest("/types/" + redType + "/breeds")

	// Handle error from the request.
	if err != nil {
		return nil, err
	}

	var breeds []Breed
	var message interface{}
	err = json.Unmarshal(body, &message)
	if err != nil {
		return nil, err
	}

	messageMap := message.(map[string]interface{})
	breedsMap := messageMap["breeds"].([]interface{})

	err = mapstructure.Decode(breedsMap, &breeds)
	if err != nil {
		return nil, err
	}

	return breeds, nil
}

// GetAnimalById returns an Animal by its ID and an error, if any.
func (c Client) GetAnimalById(animalID string) (Animal, error) {
	body, err := c.sendGetRequest("/animals/" + animalID)
	if err != nil {
		fmt.Println("Error in GetAnimalById:", err)
		return Animal{}, err
	}

	var response struct {
		Animal Animal `json:"animal"`
	}
	if err := json.Unmarshal(body, &response); err != nil {
		fmt.Println("Error unmarshaling GetAnimalById response:", err)
		return Animal{}, err
	}

	return response.Animal, nil
}

// GetAnimals returns an AnimalResponse and an error, if any.
func (c Client) GetAnimals(params SearchParams) (AnimalResponse, error) {
	queryString := params.CreateQueryString()
	fmt.Println("GetAnimals query string:", queryString)

	url := fmt.Sprintf("/animals%s", queryString)
	body, err := c.sendGetRequest(url)
	if err != nil {
		fmt.Println("Error sending request in GetAnimals:", err)
		return AnimalResponse{}, err
	}

	var animals AnimalResponse
	if err := json.Unmarshal(body, &animals); err != nil {
		fmt.Println("Error unmarshaling GetAnimals response:", err)
		return AnimalResponse{}, err
	}

	return animals, nil
}

// GetOrganizations returns an OrganizationResponse and an error, if any.
func (c Client) GetOrganizations(params SearchParams) (OrganizationResponse, error) {
	queryString := params.CreateQueryString()
	fmt.Println("GetOrganizations query string:", queryString)

	url := fmt.Sprintf("/organizations%s", queryString)
	body, err := c.sendGetRequest(url)
	if err != nil {
		fmt.Println("Error sending request in GetOrganizations:", err)
		return OrganizationResponse{}, err
	}

	var orgs OrganizationResponse
	if err := json.Unmarshal(body, &orgs); err != nil {
		fmt.Println("Error unmarshaling GetOrganizations response:", err)
		return OrganizationResponse{}, err
	}

	return orgs, nil
}

// GetOrganizationsByID returns an Organization by its ID and an error, if any.
func (c Client) GetOrganizationsByID(organizationID string) (Organization, error) {
	body, err := c.sendGetRequest("/organizations/" + organizationID)
	if err != nil {
		fmt.Println("Error in GetOrganizationsByID:", err)
		return Organization{}, err
	}

	var response struct {
		Organization Organization `json:"organization"`
	}
	if err := json.Unmarshal(body, &response); err != nil {
		fmt.Println("Error unmarshaling GetOrganizationsByID response:", err)
		return Organization{}, err
	}

	return response.Organization, nil
}
